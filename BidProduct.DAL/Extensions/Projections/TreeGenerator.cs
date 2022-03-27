using System.Linq.Expressions;

namespace BidProduct.DAL.Extensions.Projections
{
    public static class TreeGenerator<T>
    {
        public static Expression<Func<T, T>> Generate(ICollection<Expression<Func<T, object>>> selections)
        {
            var parameterExpression = selections.First().Parameters[0];

            if(parameterExpression.Name == null) throw new ArgumentNullException(nameof(parameterExpression.Name));

            var chains = selections.Select(CreateChain).ToList();

            var rootNode = new Node
            {
                IsDecoupledCollection = false,
                PropertyName = parameterExpression.Name,
                Type = parameterExpression.Type,
            };

            foreach (var chain in chains)
            {
                var previousNode = rootNode;
                foreach (var node in chain)
                {
                    var tempNode = node;
                    if (previousNode != null && !previousNode.Children.Contains(node))
                    {
                        previousNode.Children.Add(tempNode);
                    }
                    else
                    {
                        previousNode?.Children.TryGetValue(node, out tempNode);
                    }
                    previousNode = tempNode;
                }
            }

            var expression = CreateSelectExpression(parameterExpression, rootNode, parameterExpression.Name);
            return Expression.Lambda<Func<T, T>>(expression, parameterExpression);
        }

        private static Expression CreateSelectExpression(Expression parameterExpression, Node root, string path)
        {
            var propertyExpression = path.Split('.').Skip(1).Aggregate(parameterExpression, Expression.PropertyOrField);

            if (!root.Children.Any())
            {
                return propertyExpression;
            }

            if (!root.IsDecoupledCollection && !IsCollection(root.Type!))
            {
                var newExpression = Expression.New(root.Type!);
                var bindingExpressions = root.Children.Select(c =>
                {
                    if (c.MemberInfo == null) 
                        throw new ArgumentException(nameof(c.MemberInfo));
                    
                    var expression = CreateSelectExpression(parameterExpression, c, path + "." + c.PropertyName);
                    return Expression.Bind(c.MemberInfo, expression);
                }).ToList();

                return Expression.MemberInit(newExpression, bindingExpressions);
            }

            if (root.IsDecoupledCollection || IsCollection(root.Type!))
            {
                var innerType = root.Type!.GetGenericArguments().First();

                var selectMethodInfo = typeof(Enumerable).GetMethods()
                    .Single(m => m.Name == "Select" && 
                                m.IsGenericMethod &&
                                m.GetParameters().ElementAt(1).ParameterType.GetGenericArguments().Length == 2);

                selectMethodInfo = selectMethodInfo.MakeGenericMethod(innerType, innerType);

                var newParameter = Expression.Parameter(innerType, innerType.Name.ToLower());
                var newExpression = Expression.New(innerType);

                var bindingExpressions = root.Children.Select(c =>
                {
                    if (c.MemberInfo == null)
                        throw new ArgumentException(nameof(c.MemberInfo));

                    return Expression.Bind(c.MemberInfo, 
                        CreateSelectExpression(newParameter, c, root.PropertyName + "." + c.PropertyName));
                }).ToList();

                var childrenExpression = Expression.MemberInit(newExpression, bindingExpressions);

                var selectLambda = Expression.Lambda(childrenExpression, newParameter);
                var selectExpression = Expression.Call(null, selectMethodInfo, propertyExpression, selectLambda);

                var toListInfo = typeof(Enumerable).GetMethods().Single(m => m.Name == "ToList");
                toListInfo = toListInfo.MakeGenericMethod(innerType);

                var toListExpression = Expression.Call(null, toListInfo, selectExpression);

                return toListExpression;
            }

            return Expression.Empty();
        }

        private static ICollection<Node> CreateChain(Expression<Func<T, object>> selection)
        {
            var l = new Stack<Node>();
            var expression = selection.Body;

            if (expression is UnaryExpression unaryExpression)
            {
                expression = unaryExpression.Operand;
            }

            while (expression != null)
            {
                var isCollection = false;

                if (expression is MethodCallExpression methodCallExpression)
                {
                    expression = methodCallExpression.Arguments.First();
                    isCollection = true;
                }

                if (expression is MemberExpression memberExpression)
                {
                    l.Push(new Node
                    {
                        MemberInfo = memberExpression.Member,
                        Type = memberExpression.Type,
                        PropertyName = memberExpression.Member.Name,
                        IsDecoupledCollection = isCollection,
                    });

                    expression = memberExpression.Expression;
                }
                else
                {
                    expression = null;
                }
            }

            return l.ToList();
        }

        private static bool IsCollection(Type type)
        {
            var res = type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return res;
        }
    }
}
