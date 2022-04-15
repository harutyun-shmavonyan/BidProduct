using System;
using BidProduct.API;
using BidProduct.SL.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BidProduct.UnitTests.Abstract;

public abstract class TestBase
{
    protected IServiceCollection ServiceCollection = new ServiceCollection();
    protected IConfiguration Configuration = new ConfigurationBuilder()
                                            .AddEnvironmentVariables()
                                            .Build();

    protected IServiceProvider ServiceProvider => ServiceCollection.BuildServiceProvider();

    [SetUp]
    public void Initialize()
    {
        ServiceCollection.AddAdditionalMapperProfile(new ViewModelsMappingProfile());
        ServiceCollection.AddApplicationServices(Configuration);
    }
}