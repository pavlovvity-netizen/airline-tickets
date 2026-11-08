using Airline_Tickets.Data;
using Airline_Tickets.Repositories;
using Airline_Tickets.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AirlineDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AirlineDatabase")));

builder.Services.AddScoped<SeatRepository>();

builder.Services.AddScoped<ReservationRepository>();

builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<BookingService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
