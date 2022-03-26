using System.Linq.Expressions;

namespace BidProduct.DAL.Abstract.Filtering
{
    public class OrderedProperty<T>
    {
        public OrderingForm OrderingForm { get; }
        public Expression<Func<T, object>> Property { get; }

        public OrderedProperty(OrderingForm orderingForm, Expression<Func<T, object>> property)
        {
            OrderingForm = orderingForm;
            Property = property;
        }
    }
}