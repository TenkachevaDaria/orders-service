using System.Reflection;
using System.Text.Json.Serialization;
using Contracts.Events;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Handlers;
using OrderService.Application.Interfaces;
using OrderService.Application.Services;
using OrderService.Infrastructure.Persistence;
using Serilog;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Serilog.Sinks.SystemConsole.Themes;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile("Serilog.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console(theme: SystemConsoleTheme.Literate)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrdersPublisher, OrdersPublisher>();
builder.Services.AddScoped<IOrderService, OrdersService>();

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDb")));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IOrderDbContext, OrdersDbContext>();

builder.Services.AddRebus(configure => configure
    .Routing(r => r.TypeBased().Map<OrderCreatedEvent>("saga_queue"))
    .Transport(t => t.UseRabbitMq(builder.Configuration["Rabbit:ConnectionString"]!, "order_queue"))
    .Options(o =>
    {
        o.LogPipeline();
        o.SetNumberOfWorkers(1);
        o.SetMaxParallelism(1);
    })
);
builder.Services.AddScoped<IOrderService, OrdersService>();
builder.Services.AutoRegisterHandlersFromAssemblyOf<OrderSucceededHandler>();
builder.Services.AutoRegisterHandlersFromAssemblyOf<CancelOrderHandler>();
builder.Services.AutoRegisterHandlersFromAssemblyOf<OrderPayingHandler>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseRouting();
app.UseHttpsRedirection();
app.Run();

public partial class Program { }

