using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TestUserLogIn.Data;
using TestUserLogIn.Models;

namespace TestUserLogIn.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class InvolvementAreasModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InvolvementAreasModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InvolvementAreas NewInvolvementArea { get; set; }

        public List<InvolvementAreas> InvolvementAreasList { get; set; } = new();

        public void OnGet()
        {
            InvolvementAreasList = _context.InvolvementAreas.ToList();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                {
                // If the model state is invalid, return to the page with validation errors
                InvolvementAreasList = _context.InvolvementAreas.ToList();
                return Page();
            }
            
            _context.InvolvementAreas.Add(NewInvolvementArea);
            _context.SaveChanges();
            return RedirectToPage(); // Redirect to the same page to refresh the list
            
            
        }
    }
}
