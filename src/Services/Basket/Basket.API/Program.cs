using Basket.API.Basket.DeleteBasket;
using Basket.API.Basket.GetBasket;
using Basket.API.Basket.StoreBasket;
using Basket.API.Data;
using Discount.gRPC;
using FluentValidation;
using HealthChecks.UI.Client;
using Marten;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Distributed;
using UniversalCommon.Behaviors;
using UniversalCommon.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

var basketAsembly = typeof(Program).Assembly;

//add Services
builder.Services.AddCarter(configurator: c =>
{
    c.WithModule<GetBasketEndpoint>();
    c.WithModule<StoreBasketEndpoint>();
    c.WithModule<DeleteBasketEndpoint>();
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(basketAsembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LogginBehavior<,>));
});

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});

//DB
builder.Services.AddMarten(options => {
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
    options.Schema.For<ShoppingCart>().Identity(x => x.UserName);
})
.UseLightweightSessions();

builder.Services.AddValidatorsFromAssembly(basketAsembly);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, BasketCachedRepository>();
// or but not recommended
//builder.Services.AddScoped<IBasketRepository>(provider =>
//{
//    var repository = provider.GetRequiredService<IBasketRepository>();
//    return new BasketCachedRepository(repository, provider.GetRequiredService<IDistributedCache>());
//});

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();


//Configure http pipeline
app.MapCarter();

//
app.UseExceptionHandler(opt => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }
);

app.Run();
