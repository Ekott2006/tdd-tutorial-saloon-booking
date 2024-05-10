using Domain.Data;
using Domain.Repository;
using Domain.Services;
using Domain.Test.Unit;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

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


// Add services to the container.
builder.Services.AddRazorPages();

WebApplication? app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    DataContext context = scope.ServiceProvider.GetRequiredService<DataContext>();
    SeedDatabase seedDatabase = scope.ServiceProvider.GetRequiredService<SeedDatabase>();
    seedDatabase.Seed().Wait();
    if (!context.Employees.Any()) throw new Exception("Unable to Seed Database");
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();