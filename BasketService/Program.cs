using Autofac;
using Autofac.Extensions.DependencyInjection;
using BasketService.Data;
using BasketService.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container and use Autofac as the DI container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Configure Entity Framework DbContext
builder.Services.AddDbContext<BasketContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BasketConnection")));

// Add other services like controllers
builder.Services.AddControllers();

// Configure Autofac container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register DbContext
    containerBuilder.RegisterType<BasketContext>()
                    .As<DbContext>()
                    .InstancePerLifetimeScope();

    // Additional dependencies can be registered here
});

var app = builder.Build();

// Start RabbitMQ Consumer
var rabbitMqConsumer = new RabbitMqConsumer();
var consumerThread = new Thread(rabbitMqConsumer.StartConsumer);
consumerThread.Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();