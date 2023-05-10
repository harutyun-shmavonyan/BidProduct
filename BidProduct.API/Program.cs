using System;
using BidProduct.API;
using BidProduct.API.Controllers;
using BidProduct.API.ExceptionHandlers;
using BidProduct.API.Middlewares;
using BidProduct.API.User;
using BidProduct.DAL.CacheConverters;
using BidProduct.DAL.Caches;
using BidProduct.DAL.DB;
using BidProduct.SL;
using BidProduct.SL.Extensions;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using BidProduct.SL.Validators;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
    {
        options.RequireHttpsMetadata = true;

        options.TokenRetriever = request => 
        request.Cookies["access_token"];
        options.Authority = builder.Configuration["IdentityServer:BaseUrl"];
        options.SupportedTokens = SupportedTokens.Reference;

        options.ApiName = builder.Configuration["IdentityServer:ApiName"];
        options.ApiSecret = builder.Configuration["IdentityServer:ApiSecret"];
    });

services.AddHttpContextAccessor();

services.AddTransient<FirstExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, DbUpdateExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, ValidationFailedExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, InvalidOperationExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, NotFoundExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, LastExceptionHandler>();

services.AddAdditionalMapperProfile(new ViewModelsMappingProfile());

services.AddApplicationServices(builder.Configuration);
services.AddUserIdProvider<UserIdProvider>();
var logger = services.AddElasticSearchLogging(builder.Configuration, builder.Environment);
services.AddRepositories();
services.AddCache();
services.AddEfCore(builder.Configuration.GetConnectionString("BidProductConnectionString"));
services.AddValidators();
services.AddLogging<ElasticSearchLogger>();

services.ForRequest<GetProductQuery, GetProductQueryResponse>()
    .AddValidation()
    .AddValidator<GetProductQueryValidator>()
    .AddCaching<GetProductQuery, GetProductQueryResponse>()
    .WithKey<string>()
    .WithConverter<ProductCacheConverter>()
    .WithValue<object>()
    .WithConverter<ProductCacheConverter>()
    .WithExpiration(TimeSpan.FromHours(1))
    .Build<InMemoryExpirableCache<GetProductQuery, GetProductQueryResponse>>(CacheLifetime.Singleton);

services.ForRequest<CreateProductCommand, CreateProductCommandResponse>()
    .AddValidation()
    .AddValidator<CreateProductCommandValidator>();

services.AddControllers();
services.AddMvc();

services.AddHttpClient<BFFController>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["IdentityServer:BaseUrl"]);
    options.Timeout = TimeSpan.FromSeconds(5);
});

builder.Host.UseSerilog(logger);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    scope.ServiceProvider.GetRequiredService<BidProductDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<AutoMapper.IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();