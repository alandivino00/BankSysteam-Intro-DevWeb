using BankSysteam.Api.data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            // 1. Define o esquema de segurança (o botão "Authorize")
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"Cabeçalho de autorização JWT usando o esquema Bearer. 
                      \r\n\r\n Digite 'Bearer' [espaço] e então seu token na caixa de texto abaixo.
                      \r\n\r\n Exemplo: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            // 2. Define que esse esquema deve ser aplicado globalmente
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
        });

        builder.Services.AddDbContext<BankContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("BankDatabase")));

        var jwtSettings = builder.Configuration.GetSection("JwtSettings"); 
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"]
            };
        });

        builder.Services.AddAuthorization();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //builder.Services.AddOpenApi();

        builder.Services.AddScoped<BankSysteam.Api.Repositories.IContaRepository, BankSysteam.Api.Repositories.ContaRepository>();
        builder.Services.AddScoped<BankSysteam.Api.Service.IContaService, BankSysteam.Api.Service.ContaService>();

        var app = builder.Build();

        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            //app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();


        app.MapControllers();

        app.MapGet("/health", async (BankContext context) =>
        {
            bool canConnect;
            try
            {
                canConnect = await context.Database.CanConnectAsync();
            }
            catch 
            {
                canConnect = false;
            }

            if (canConnect)
                return Results.Ok("Meu banco esta funcionando.");

            return Results.Problem("Meu banco falhou");
        });

        app.Run();
    }
}