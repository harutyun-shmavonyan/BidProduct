﻿using System;
using System.Collections.Generic;
using AutoMapper;
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
using BidProduct.SL.Proxies;
using BidProduct.SL.Proxies.Cache;
using BidProduct.SL.Utils;
using Mapper = BidProduct.SL.Mapping.Mapper;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.CacheConverters;
using BidProduct.SL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Abstract.Validation;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using BidProduct.SL.Services;
using Serilog;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Elasticsearch;
using BidProduct.Common.Abstract;
using BidProduct.DAL.Repositories;
using BidProduct.DAL.BasicRepositories;
using BidProduct.DAL.Models;
using BidProduct.SL.CQRS.QueryHandlers;
using BidProduct.DAL.Caches;

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
            services.AddScoped<ITraceIdProvider>(_ => new TraceIdProvider(Guid.NewGuid().ToString()));

            services.AddTransient<IDateTimeService, StandardDateTimeService>();

            services.AddTransient<IInternalMediator, InternalMediator>();
            services.AddMediatR(typeof(InternalMediator));

            services.AddSingleton<Common.Abstract.IMapper, Mapper>();
            services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ModelsMappingProfile());
                mc.AddProfiles(Profiles);
            }).CreateMapper());

            services.Configure<InternalMessageLoggingConfiguration>(
                configuration.GetSection("InternalMessageLoggingConfiguration"));
        }

        public static void AddUserIdProvider<TUserIdProvider>(this IServiceCollection service) where TUserIdProvider : class, IUserIdProvider
        {
            service.AddScoped<IUserIdProvider, TUserIdProvider>();
        }

        public static Serilog.ILogger AddElasticSearchLogging(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            Log.Logger = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment.EnvironmentName))
                 .Enrich.WithProperty("Environment", environment)
                 .ReadFrom.Configuration(configuration) 
                 .CreateLogger();

            return Log.Logger;

            static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
            {
                return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"BidProduct_{environment}"
                };
            }
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

            services.AddScoped(typeof(IQueryableRepository<,>), typeof(QueryableRepository<,>));
            services.AddScoped(typeof(ICreatableRepository<,>), typeof(CreatableRepository<,>));
            services.AddScoped(typeof(IUpdateableRepository<,>), typeof(UpdateableRepository<,>));
            services.AddScoped(typeof(IUpsertableRepository<,>), typeof(UpsertableRepository<,>));
            services.AddScoped(typeof(IRemovableRepository<,>), typeof(RemovableRepository<,>));

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IRequestHandler<GetByFilterQuery<Product, long, GetProductQueryResponse>, ICollection<GetProductQueryResponse>>,
                    GetByFilterQueryHandler<Product, long, GetProductQueryResponse>>();
        }

        public static void AddDateTimeService<TDateTimeService>(this IServiceCollection services)
            where TDateTimeService : class, IDateTimeService
        {
            services.AddTransient<IDateTimeService, TDateTimeService>();
        }

        public static void AddEfCore(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BidProductDbContext>(options => options.UseSqlServer(connectionString));
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.Decorate(typeof(IRequestHandler<,>), typeof(InternalRequestHandlerValidatorProxy<,>));
        }

        public static void AddLogging<TLogger>(this IServiceCollection services) where TLogger : class, Abstract.ILogger
        {
            services.AddTransient<Abstract.ILogger, TLogger>();
            services.Decorate(typeof(IRequestHandler<,>), typeof(InternalMessageLoggerProxy<,>));
        }

        private static void AddCacheDecorators(IServiceCollection services)
        {
            services.Decorate<IRequestHandler<GetProductQuery, GetProductQueryResponse>,
                InternalRequestHandlerReadThroughCacheProxy<GetProductQuery, GetProductQueryResponse, string, object>>();
        }

        private static void AddCacheConverters(IServiceCollection services)
        {
            services.AddSingleton<ProductCacheConverter>();
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

        public ValidationLayerBuilder<TRequest> AddValidation()
        {
            return new ValidationLayerBuilder<TRequest>(Services);
        }

        public CacheKeyBuilder<TInternalRequest, TResponse> AddCaching<TInternalRequest, TResponse>()
            where TResponse : class where TInternalRequest : IInternalRequest<TResponse>
        {
            return new CacheKeyBuilder<TInternalRequest, TResponse>(Services);
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

        public CacheValueConverterBuilder<TRequest, TResponse, TKey, TValue> WithValue<TValue>()
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

        public CacheDecoratorBuilder<TRequest, TResponse, TKey, TValue> WithExpiration(TimeSpan expiration)
        {
            _services.AddSingleton<IExpirationPolicy<TRequest, TResponse>>(
                new ExpirationPolicy<TRequest, TResponse>(expiration));

            return this;
        }

        public void Build<TCache>(CacheLifetime lifetime) where TCache : class, IKeyValueCache<TRequest, TResponse, TKey, TValue>
        {
            switch (lifetime)
            {
                case CacheLifetime.Singleton:
                    _services.AddSingleton<IKeyValueCache<TRequest, TResponse, TKey, TValue>, TCache>();
                    break;
                case CacheLifetime.Scoped:
                    _services.AddScoped<IKeyValueCache<TRequest, TResponse, TKey, TValue>, TCache>();
                    break;
                case CacheLifetime.Transient:
                    _services.AddTransient<IKeyValueCache<TRequest, TResponse, TKey, TValue>, TCache>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }
        }
    }

    public enum CacheLifetime
    {
        Singleton,
        Scoped,
        Transient
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
