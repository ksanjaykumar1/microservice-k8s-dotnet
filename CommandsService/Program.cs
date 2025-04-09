using CommandService.Data;
using CommandsService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Registers AppDbContext with an in-memory database named "InMem" for testing or development purposes
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));

// Registers the CommandRepo implementation for ICommandRepo with scoped lifetime,
// meaning a new instance is created per HTTP request
builder.Services.AddScoped<ICommandRepo, CommandRepo>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registers AutoMapper and automatically scans all loaded assemblies 
// for classes that inherit from AutoMapper.Profile to configure object mappings
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
