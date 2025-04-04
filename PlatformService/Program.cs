using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddDbContext<AppDbContext>( option => 
// {
//     option.UseMySql( builder.Configuration.GetConnectionString("DefaultConnection"),
//     new MySqlServerVersion(new Version(8, 0, 34)));
// });

if (builder.Environment.IsProduction())
{

    Console.WriteLine("---> Using MySQL Db");
    Console.WriteLine($"---> MySQL Db:${builder.Configuration.GetConnectionString("PlatformsConn")}");
    builder.Services.AddDbContext<AppDbContext>(option =>
        option.UseMySql(
            builder.Configuration.GetConnectionString("PlatformsConn"),
            new MySqlServerVersion(new Version(8, 0, 34)))
    );
}
else
{
    Console.WriteLine("---> Using In-Memory Db");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("InMem"));
}

// Registering PlatformRepo as a scoped service.
// A new instance will be created per HTTP request, ensuring safe database operations.
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

// Registers HttpCommandDataClient as the implementation for ICommandDataClient,
// automatically providing an HttpClient instance via HttpClientFactory.
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true; // Ignore SSL errors
        return handler;
    });
Console.WriteLine($"--->  CommandService Endpoint {builder.Configuration["CommandService"]}");

builder.Services.AddControllers();

// Register AutoMapper for Dependency Injections
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
// if(app.Environment.IsProduction())
// {
//         Console.WriteLine("---> Using Mysql Db");
//         builder.Services.AddDbContext<AppDbContext>( option => 
//         {
//             option.UseMySql( builder.Configuration.GetConnectionString("DefaultConnection"),
//             new MySqlServerVersion(new Version(8, 0, 34)));
//         }); 
        
// }
// else
// {
//     Console.WriteLine("---> Using inMem Db");
//     builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
// }

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();

