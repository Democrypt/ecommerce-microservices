using Consul;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json");

builder.Services.AddOcelot();

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

await app.UseOcelot();

app.Run();
