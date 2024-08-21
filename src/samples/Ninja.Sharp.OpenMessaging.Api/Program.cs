using Ninja.Sharp.OpenMessaging.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddUserSecrets(typeof(Program).Assembly);

string artemisTopic = "artemisTopic";
string kafkaTopic = "kafkaTopic";

builder.Services
    .AddArtemisServices(builder.Configuration)
    .AddProducer(artemisTopic)
    .AddConsumer<LoggerMqConsumer>(artemisTopic)
    .Build();


builder.Services
    .AddKafkaServices(builder.Configuration)
    .AddProducer(kafkaTopic)
    .AddConsumer<LoggerMqConsumer>(kafkaTopic)
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
