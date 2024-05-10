using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Booking;

public class Confirmation : PageModel
{
    [TempData] public bool IsValid { get; set; }
    public string ConfirmationText { get; set; }

    public IActionResult OnGet()
    {
        if (!IsValid) return RedirectToPage("/Index");
        ConfirmationText = IsValid ? "Your appointment has been confirmed" : "Unable to book appointment";
        return Page();
    }
}