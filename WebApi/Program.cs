using Domain;
using Domain.Data;
using Domain.Repository;
using Domain.Services;
using Domain.Test.Unit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<SalonServiceRepository>();
builder.Services.AddScoped<ShiftRepository>();

builder.Services.AddScoped<CreateBookingService>();
builder.Services.AddScoped<EmployeeHelper>();
builder.Services.AddScoped<GetEmployeeTimeService>();
builder.Services.AddScoped<SeedDatabase>();

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite($"Data Source=my_application_booking.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (IServiceScope scope = app.Services.CreateScope())
{
    DataContext context = scope.ServiceProvider.GetRequiredService<DataContext>();
    SeedDatabase seedDatabase = scope.ServiceProvider.GetRequiredService<SeedDatabase>();
    seedDatabase.Seed().Wait();
    if (!context.Employees.Any()) throw new Exception("Unable to Seed Database");
}


app.UseHttpsRedirection();

app.MapGet("/services", RouteHelper.GetServices);
app.MapGet("/employees", RouteHelper.GetEmployee);
app.MapGet("/bookingTime", RouteHelper.GetBookingTime);
app.MapPost("/booking", RouteHelper.CreateBooking);

app.Run();
