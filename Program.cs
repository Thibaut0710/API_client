using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API_Client.Context;
using API_Client.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration JWT pour l'authentification
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("https://0.0.0.0:7296")  // URL de l'API client
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// Configuration du DbContext pour MySQL
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
builder.WebHost.UseUrls("https://0.0.0.0:7296");
// Configuration de Swagger pour la documentation de l'API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure le pipeline des requetes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Activer CORS
app.UseCors("CorsPolicy");

// Activer l'authentification et l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Démarrer le consommateur RabbitMQ
app.Services.GetRequiredService<RabbitMQConsumer>();

app.Run();
