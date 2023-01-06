using Microsoft.Extensions.Hosting;

namespace Barbecue;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSignalR();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMemoryCache();
        builder.Services.AddCors(options =>
        {
            options
                .AddDefaultPolicy(b => 
                    b.AllowAnyHeader().AllowCredentials()
                        .SetIsOriginAllowed(f => true));
        });

        builder.Host.UseOrleans(siloBuilder =>
        {
            siloBuilder
                .UseLocalhostClustering()
                .AddMemoryGrainStorage("PubSubStore")
                .AddMemoryStreams("barbecue");
        });
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors();
        app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new[] { "index.html" } });
        app.UseStaticFiles();
        app.UseHttpsRedirection();

        app.UseExceptionMiddleware();
        app.UseAuthenticationMiddleware();
        app.UseAuthorization();

        app.MapHub<GameHub>("/game");
        app.MapControllers();

        app.Run();
    }
}