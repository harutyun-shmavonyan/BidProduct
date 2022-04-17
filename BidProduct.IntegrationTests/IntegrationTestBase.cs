using System;
using System.Threading.Tasks;
using BidProduct.DAL.CacheConverters;
using BidProduct.DAL.Caches;
using BidProduct.DAL.DB;
using BidProduct.SL;
using BidProduct.SL.Abstract;
using BidProduct.SL.Extensions;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using BidProduct.SL.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BidProduct.IntegrationTests;

public abstract class IntegrationTestBase
{
    protected IServiceCollection ServiceCollection = new ServiceCollection();
    protected IConfiguration Configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.Test.json", optional: false)
        .Build();

    protected IServiceProvider ServiceProvider => ServiceCollection.BuildServiceProvider();
    protected IInternalMediator Mediator => ServiceProvider.GetRequiredService<IInternalMediator>();
    protected DbContext DbContext => ServiceProvider.GetRequiredService<BidProductDbContext>();

    [SetUp]
    public void Initialize()
    {
        ServiceCollection.AddApplicationServices(Configuration);
        ServiceCollection.AddUserIdProvider<TestUserIdProvider>();
        ServiceCollection.AddRepositories();
        ServiceCollection.AddCache();
        ServiceCollection.AddEfCore(Configuration.GetConnectionString("BidProductTestConnectionString"));
        ServiceCollection.AddValidators();
        ServiceCollection.AddLogging<ElasticSearchLogger>();

        ServiceCollection.AddDateTimeService<FixedDateTimeService>();

        ServiceCollection.ForRequest<GetProductQuery, GetProductQueryResponse>()
            .AddValidation()
            .AddValidator<GetProductQueryValidator>()
            .AddCaching<GetProductQuery, GetProductQueryResponse>()
            .WithKey<string>()
            .WithConverter<ProductCacheConverter>()
            .WithValue<object>()
            .WithConverter<ProductCacheConverter>()
            .WithExpiration(TimeSpan.FromHours(1))
            .Build<InMemoryExpirableCache<GetProductQuery, GetProductQueryResponse>>(CacheLifetime.Singleton);

        ServiceCollection.ForRequest<CreateProductCommand, CreateProductCommandResponse>()
            .AddValidation()
            .AddValidator<CreateProductCommandValidator>();

        using var scope = ServiceCollection.BuildServiceProvider().CreateScope();
        scope.ServiceProvider.GetRequiredService<BidProductDbContext>().Database.Migrate();
    }

    protected async Task TruncateAsync<T>() where T : class
    {
        var entityType = DbContext.Model.FindEntityType(typeof(T));
        var schema = entityType.GetSchema();
        var tableName = entityType.GetTableName();

        var sqlCommand = @$"EXEC sp_msforeachtable ""ALTER TABLE ? NOCHECK CONSTRAINT all"" 
                                  TRUNCATE TABLE {schema}.{tableName}
                                  EXEC sp_msforeachtable ""ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all""";

        await DbContext.Database.ExecuteSqlRawAsync(sqlCommand);
    }
}