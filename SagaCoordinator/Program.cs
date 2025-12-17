using Contracts.Commands;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using SagaCoordinator;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddRebus(
    configure => configure
        .Transport(t => 
            t.UseRabbitMq(builder.Configuration["Rabbit:ConnectionString"], "saga_queue"))
        .Routing(r => r.TypeBased()
            .Map<ReserveItemsCommand>("inventory_queue")
            .Map<CancelReservationCommand>("inventory_queue")
            .Map<CancelOrderCommand>("order_queue")
            .Map<OrderPayingCommand>("order_queue")
            .Map<OrderSucceededCommand>("order_queue")
            .Map<PayOrderCommand>("payment_queue"))
        .Sagas(s => s.StoreInPostgres(
            builder.Configuration.GetConnectionString("RebusSql"),
            dataTableName: "RebusSagaData",
            indexTableName: "RebusSagaIndex",
            automaticallyCreateTables: true))
        .Options(o =>
        {
            
            o.LogPipeline();
            o.SetNumberOfWorkers(1);
            o.SetMaxParallelism(1);
            
        })
       ,
    isDefaultBus: true
);

builder.Services.AutoRegisterHandlersFromAssemblyOf<OrderSaga>();
var app = builder.Build();
app.MapGet("/", () => "Saga Coordinator is running");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
