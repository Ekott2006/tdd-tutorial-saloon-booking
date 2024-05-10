using Domain.DTOs;
using Domain.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages;

public class IndexModel(ILogger<IndexModel> logger, SalonServiceRepository repository) : PageModel
{
    public IEnumerable<SalonServiceResponse> Services { get; set; }

    public async void OnGet()
    {
        Services = await repository.Get();
    }
}