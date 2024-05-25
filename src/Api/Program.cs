using Infra.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ConsumerService>();
builder.Services.AddSingleton<ProducerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", async ([FromServices] ProducerService producer) =>
{
    await producer.ProduceAsync("my-topic", "Hello, Kafka!");
    return "Message produced";
})
.WithOpenApi();

app.Run();