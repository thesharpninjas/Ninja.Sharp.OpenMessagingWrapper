using Microsoft.Extensions.Configuration;
using Ninja.Sharp.OpenMessagingMiddleware.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddUserSecrets(typeof(Program).Assembly);

string topic = "MS00536.AS.AckINPSPRE";
builder.Services
    .AddArtemisServices(builder.Configuration)
    .AddProducer(topic)
    .AddConsumer<LoggerMqConsumer>(topic)
    .Build();

topic = "hello-world";
builder.Services
    .AddKafkaServices(builder.Configuration)
    .AddProducer(topic)
    .AddConsumer<LoggerMqConsumer>(topic)
    .Build();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
