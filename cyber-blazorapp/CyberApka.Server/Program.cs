using CyberApka.Server.Data.Database;
using CyberApka.Server.Features.Auth.Commands;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<CyberDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddFastEndpoints();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<CreateUser.Handler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWasm",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowWasm");
app.UseFastEndpoints();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
