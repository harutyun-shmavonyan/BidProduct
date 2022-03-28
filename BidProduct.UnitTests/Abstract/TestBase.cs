using System;
using BidProduct.API;
using BidProduct.SL.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BidProduct.UnitTests;

public abstract class TestBase
{
    protected IServiceCollection ServiceCollection;
    protected IConfiguration Configuration;

    protected IServiceProvider ServiceProvider => ServiceCollection.BuildServiceProvider();

    [SetUp]
    public void Initialize()
    {
        ServiceCollection = new ServiceCollection();
        Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        ServiceCollection.AddAdditionalMapperProfile(new ViewModelsMappingProfile());
        ServiceCollection.AddApplicationServices(Configuration);
    }
}