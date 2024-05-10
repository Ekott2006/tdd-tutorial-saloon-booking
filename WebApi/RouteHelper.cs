using Domain.DTOs;
using Domain.Model;
using Domain.Repository;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi;

public class RouteHelper
{
    public static async Task<IResult> GetServices(SalonServiceRepository repository)
    {
        return TypedResults.Ok(await repository.Get());
    }

    public static async Task<IResult> GetEmployee(EmployeeRepository repository)
    {
        return TypedResults.Ok(await repository.Get());
    }

    public static async Task<IResult> GetBookingTime([AsParameters] GetEmployeeTimeRequest request,
        GetEmployeeTimeService service, SalonServiceRepository repository)
    {
        SalonServiceResponse? salonService = await repository.Get(request.ServiceId);
        if (salonService is null) return TypedResults.NotFound();
        return TypedResults.Ok(await service.GetEmployeeTime(salonService.Duration, request.EmployeeId));
    }

    public static async Task<IResult> CreateBooking(CreateBookingService service,
        CreateBookingServiceRequest request, SalonServiceRepository repository)
    {
        System.Console.WriteLine($"Request Firsname {request.FirstName}, Lastname: {request.LastName}, Datetime: {request.StartDate}, ServiceId: {request.ServiceId}");

        SalonServiceResponse? salonService = await repository.Get(request.ServiceId);
        if (salonService is null) return TypedResults.NotFound();
        System.Console.WriteLine($"Salon Service ID: {salonService.Id}, Duration: {salonService.Duration}");
        Period period = new(request.StartDate, request.StartDate + salonService.Duration);
        System.Console.WriteLine("Period: StartDate: {0}, EndDate: {1}, Duration: {2}", period.StartDateTime, period.EndDateTime, period.Duration);

        CreateBookingServiceParameter parameter = new(request.FirstName, request.LastName, period, request.EmployeeId);
        return await service.Create(parameter) ? TypedResults.Created() : TypedResults.NotFound();
    }
}