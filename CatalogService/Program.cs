using Autofac;
using Autofac.Extensions.DependencyInjection;
using CatalogService.Data;
using Consul;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogConnection")));

builder.Services.AddControllers();

// Configure Autofac container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register DbContext
    containerBuilder.RegisterType<CatalogContext>()
                    .As<DbContext>()
                    .InstancePerLifetimeScope();

    // Register other dependencies here if needed in the future
});

var app = builder.Build();

var consulClient = new ConsulClient(config =>
{
    config.Address = new Uri("http://localhost:8500");
});

var registration = new AgentServiceRegistration()
{
    ID = app.Environment.ApplicationName,
    Name = app.Environment.ApplicationName,
    Address = "localhost",
    Port = app.Environment.IsDevelopment() ? 5001 : 80
};

consulClient.Agent.ServiceRegister(registration).Wait();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();