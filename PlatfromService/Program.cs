using Microsoft.EntityFrameworkCore;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddDbContext<AppDbContext>( option => 
// {
//     option.UseMySql( builder.Configuration.GetConnectionString("DefaultConnection"),
//     new MySqlServerVersion(new Version(8, 0, 34)));
// });
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));

// Registering PlatformRepo as a scoped service.
// A new instance will be created per HTTP request, ensuring safe database operations.
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();

