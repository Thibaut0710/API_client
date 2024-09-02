using API_Client.Context;
using API_Client.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ClientsContext>(options =>
options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new MariaDbServerVersion(new Version(10, 11, 6)),
            optionsBuilder => optionsBuilder.EnableRetryOnFailure()
        )
    );

builder.Services.AddControllers();
builder.Services.AddScoped<ClientService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddSingleton<RabbitMQConsumer>();
builder.WebHost.UseUrls("http://0.0.0.0:5203");
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Services.GetRequiredService<RabbitMQConsumer>();
app.Run();
