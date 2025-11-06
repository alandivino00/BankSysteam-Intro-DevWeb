using BankSysteam.Api.data;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddDbContext<BankContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("BankDatabase")));


        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            //app.MapOpenApi();
        }

        app.UseHttpsRedirection();

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