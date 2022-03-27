using BidProduct.API;
using BidProduct.API.ExceptionHandlers;
using BidProduct.API.Middlewares;
using BidProduct.Common.Abstract;
using BidProduct.DAL.CacheConverters;
using BidProduct.DAL.Caches;
using BidProduct.DAL.DB;
using BidProduct.SL.Extensions;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using BidProduct.SL.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddTransient<FirstExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, DbUpdateExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, ValidationFailedExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, InvalidOperationExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, NotFoundExceptionHandler>();
services.AddTransient<ExceptionHandlerBase, LastExceptionHandler>();

services.AddAdditionalMapperProfile(new ViewModelsMappingProfile());

services.AddApplicationServices(builder.Configuration);
services.AddRepositories();
services.AddCache();
services.AddEfCore(builder.Configuration.GetConnectionString("BidProductConnectionString"));
services.AddValidators();
services.AddMessageLogging();

services.ForRequest<GetProductQuery, GetProductQueryResponse>()
    .AddValidation()
    .AddValidator<GetProductQueryValidator>()
    .AddCaching<GetProductQuery, GetProductQueryResponse>()
    .WithKey<long>()
    .WithConverter<ProductCacheConverter>()
    .WithValue<GetProductQueryResponse>()
    .WithConverter<ProductCacheConverter>()
    .Build<InMemoryCache<GetProductQuery, GetProductQueryResponse, long, GetProductQueryResponse>>(CacheLifetime.Singleton);

services.ForRequest<CreateProductCommand, CreateProductCommandResponse>()
    .AddValidation()
    .AddValidator<CreateProductCommandValidator>();

services.AddControllers();
services.AddMvc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    scope.ServiceProvider.GetRequiredService<BidProductDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<AutoMapper.IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();