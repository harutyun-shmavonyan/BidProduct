﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BidProduct.DAL;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.DB;
using BidProduct.DAL.FilterExecutors;
using BidProduct.SL.Mapping;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Proxies;
using BidProduct.SL.Proxies.Cache;
using BidProduct.SL.Utils;
using Mapper = BidProduct.SL.Mapping.Mapper;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Abstract.Validation;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.SL.Extensions
{
    public static class ServicesExtensions
    {
        private static readonly ICollection<Profile> Profiles = new List<Profile>();

        // ReSharper disable once UnusedParameter.Global
        public static void AddAdditionalMapperProfile(this IServiceCollection services, Profile profile) =>
            Profiles.Add(profile);

        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IScopeIdProvider>(_ => new ScopeIdProvider(Guid.NewGuid().ToString()));

            services.AddTransient<IInternalMediator, InternalMediator>();
            services.AddMediatR(typeof(InternalMediator));

            services.AddSingleton<Common.Abstract.IMapper, Mapper>();
            services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ModelsMappingProfile());
                mc.AddProfiles(Profiles);
            }).CreateMapper());

            services.ForRequest<GetProductQuery, GetProductQueryResponse>().AddCaching<GetProductQuery, GetProductQueryResponse>(services).WithKey<string>();

            services.Configure<InternalMessageLoggingConfiguration>(_ => configuration.GetSection("InternalMessageLoggingConfiguration"));
        }

        public static void AddCache(this IServiceCollection services)
        {
            AddCacheConverters(services);
            AddCacheDecorators(services);
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IIncludeFilterExecutor, IncludeFilterExecutor>();
            services.AddScoped<IProjectionFilterExecutor, ProjectionFilterExecutor>();
        }

        public static void AddEfCore(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BidProductDbContext>(options => options.UseSqlServer(connectionString));
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.Decorate(typeof(IRequestHandler<,>), typeof(InternalRequestHandlerValidatorProxy<,>));
        }

        public static void AddMessageLogging(this IServiceCollection services)
        {
            services.AddTransient<ILogger, ConsoleLogger>();
            services.Decorate(typeof(IRequestHandler<,>), typeof(InternalMessageLoggerProxy<,>));
        }

        private static void AddCacheDecorators(IServiceCollection services)
        {
        }

        private static void AddCacheConverters(IServiceCollection services)
        {
        }

        public static RequestPipelineBuilder<TRequest> ForRequest<TRequest, TResponse>(this IServiceCollection services) where TResponse : class where TRequest : IInternalRequest<TResponse>
        {
            return new RequestPipelineBuilder<TRequest>(services);
        }
    }

    public class RequestPipelineBuilder<TRequest>
    {
        protected readonly IServiceCollection Services;

        public RequestPipelineBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public ValidationLayerBuilder<TRequest> AddValidation(IServiceCollection services)
        {
            return new ValidationLayerBuilder<TRequest>(services);
        }

        public CacheKeyBuilder<TRequest1, TResponse> AddCaching<TRequest1, TResponse>(IServiceCollection services) where TResponse : class where TRequest1 : IInternalRequest<TResponse>
        {
            return new CacheKeyBuilder<TRequest1, TResponse>(services);
        }
    }

    public class CacheKeyBuilder<TRequest, TResponse> where TRequest : IInternalRequest<TResponse> where TResponse : class
    {
        private readonly IServiceCollection _services;

        public CacheKeyBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public CacheKeyConverterBuilder<TRequest, TResponse, TKey> WithKey<TKey>()
        {
            return new CacheKeyConverterBuilder<TRequest, TResponse, TKey>(_services);
        }
    }

    public class CacheKeyConverterBuilder<TRequest, TResponse, TKey> where TRequest : IInternalRequest<TResponse> where TResponse : class
    {
        private readonly IServiceCollection _services;

        public CacheKeyConverterBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public CacheValueBuilder<TRequest, TResponse, TKey> WithConverter<TConverter>() where TConverter : class, ICacheKeyConverter<TRequest, TKey>
        {
            _services.AddTransient<ICacheKeyConverter<TRequest, TKey>, TConverter>();
            return new CacheValueBuilder<TRequest, TResponse, TKey>(_services);
        }
    }

    public class CacheValueBuilder<TRequest, TResponse, TKey> where TRequest : IInternalRequest<TResponse> where TResponse : class
    {
        private readonly IServiceCollection _services;

        public CacheValueBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public CacheValueConverterBuilder<TRequest, TResponse, TKey, TValue> WithValueConverter<TValue>()
        {
            return new CacheValueConverterBuilder<TRequest, TResponse, TKey, TValue>(_services);
        }
    }

    public class CacheValueConverterBuilder<TRequest, TResponse, TKey, TValue> where TRequest : IInternalRequest<TResponse> where TResponse : class
    {
        private readonly IServiceCollection _services;

        public CacheValueConverterBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public CacheDecoratorBuilder<TRequest, TResponse, TKey, TValue> WithConverter<TConverter>() where TConverter : class, ICacheValueConverter<TResponse, TValue>
        {
            _services.AddTransient<ICacheValueConverter<TResponse, TValue>, TConverter>();
            return new CacheDecoratorBuilder<TRequest, TResponse, TKey, TValue>(_services);
        }
    }

    public class CacheDecoratorBuilder<TRequest, TResponse, TKey, TValue> where TRequest : IInternalRequest<TResponse> where TResponse : class
    {
        private readonly IServiceCollection _services;

        public CacheDecoratorBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public CacheBuilder<TRequest, TResponse, TKey, TValue> Build<TCache>(CacheLifetime lifetime) where TCache : class, IExpirableKeyValueCache<TRequest, TResponse, TKey, TValue>
        {
            _services.Decorate<IRequestHandler<TRequest, TResponse>, InternalRequestHandlerReadThroughCacheProxy<TRequest, TResponse, TKey, TResponse>>();

            switch (lifetime)
            {
                case CacheLifetime.Singleton:
                    _services.AddSingleton<IExpirableKeyValueCache<TRequest, TResponse, TKey, TValue>, TCache>();
                    break;
                case CacheLifetime.Scoped:
                    _services.AddScoped<IExpirableKeyValueCache<TRequest, TResponse, TKey, TValue>, TCache>();
                    break;
                case CacheLifetime.Transient:
                    _services.AddTransient<IExpirableKeyValueCache<TRequest, TResponse, TKey, TValue>, TCache>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }

            return new CacheBuilder<TRequest, TResponse, TKey, TValue>(_services);
        }
    }

    public enum CacheLifetime
    {
        Singleton,
        Scoped,
        Transient
    }

    public class CacheBuilder<TRequest, TResponse, TKey, TValue>
    {
        private readonly IServiceCollection _services;

        public CacheBuilder(IServiceCollection services)
        {
            _services = services;
        }
    }

    public class ValidationLayerBuilder<TRequest> : RequestPipelineBuilder<TRequest>
    {
        public ValidationLayerBuilder(IServiceCollection services) : base(services)
        {
        }

        public ValidationLayerBuilder<TRequest> AddValidator<TValidator>() where TValidator : class, IValidator<TRequest>
        {
            Services.AddTransient<IValidator<TRequest>, TValidator>();
            return this;
        }
    }
}
