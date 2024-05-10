using System.Text.Json;
using Domain.DTOs;
using Domain.Model;
using Domain.Repository;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Website.Dtos;

namespace Website.Pages.Booking;

public class IndexModel(
    SalonServiceRepository repository,
    CreateBookingService service,
    EmployeeRepository employeeRepository,
    GetEmployeeTimeService getEmployeeTimeService,
    ILogger<IndexModel> logger) : PageModel
{
    [BindProperty] public CreateBookingRequest Request { get; set; }
    [TempData] public bool IsValid { get; set; }
    [BindProperty(SupportsGet = true)] public Guid Id { get; set; }

    public SalonServiceResponse SalonService { get; set; }
    public IEnumerable<SelectListItem> Employees { get; set; }

    private async Task<bool> SetCustomProperty()
    {
        SalonServiceResponse? salonService = await repository.Get(Id);
        if (salonService is null) return false;
        SalonService = salonService;
        return true;
    }

    public async Task<IActionResult> OnGet()
    {
        if (!await SetCustomProperty()) return RedirectToPage("/Index");
        Employees = (await employeeRepository.Get()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
        return Page();
    }

    public async Task<IActionResult> OnGetValidTimes(Guid? employeeId = null)
    {
        if (!await SetCustomProperty()) return NotFound();
        return Content(
            JsonSerializer.Serialize(await getEmployeeTimeService.GetEmployeeTime(SalonService.Duration, employeeId)));
    }


    public async Task<IActionResult> OnPost()
    {
        if (!await SetCustomProperty()) return RedirectToPage("/Index");
        DateTime dateTime = Request.StartDate.ToDateTime(Request.StartTime);

        logger.LogInformation("Request Firstname {}, Lastname: {}, Datetime: {}, EmployeeId: {}  ServiceId: {}", Request.FirstName, Request.LastName,  dateTime, Request.EmployeeId, Id);

        if (!ModelState.IsValid)
        {
            Employees = (await employeeRepository.Get()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
            return Page();
        }

        CreateBookingServiceParameter parameter = new(Request.FirstName, Request.LastName,
            new Period(dateTime, dateTime + SalonService.Duration), Request.EmployeeId);

        IsValid = await service.Create(parameter);
        return RedirectToPage("Confirmation");
    }
}