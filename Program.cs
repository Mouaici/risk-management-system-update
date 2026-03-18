using System;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Hello, World!");

// TESTING THE RULES

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RiskManagementDbContext>(options =>

    options.UseMySql(

        builder.Configuration.GetConnectionString("DefaultConnection"),

        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))

    ));

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.Run();