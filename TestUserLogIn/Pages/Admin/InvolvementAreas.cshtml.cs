using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TestUserLogIn.Data;
using TestUserLogIn.Models;

namespace TestUserLogIn.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class InvolvementAreasModel(ApplicationDbContext context) : PageModel
    {
        private readonly ApplicationDbContext _context = context;

        [BindProperty]
        public InvolvementAreas NewInvolvementArea { get; set; }

        public List<InvolvementAreas> InvolvementAreasList { get; set; } = new();

        public void OnGet()
        {
            InvolvementAreasList = _context.InvolvementAreas
                .Where(ia => ia.IsActive)
                .ToList();
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

        [BindProperty]
        public InvolvementAreas? EditInvolvementArea { get; set; }

        [BindProperty]
        public int? EditInvolvementAreaId { get; set; }
        public IActionResult OnPostEdit(int id)
        {
            EditInvolvementAreaId = id;
            var existingArea = _context.InvolvementAreas.Find(id);
            if (existingArea == null) return NotFound();

            EditInvolvementArea = existingArea;
            InvolvementAreasList = _context.InvolvementAreas
                .Where(ia => ia.IsActive)
                .ToList();

            return Page();
        }

        public IActionResult OnPostSaveEdit()
        {
            if (EditInvolvementArea == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                InvolvementAreasList = _context.InvolvementAreas
                    .Where(ia => ia.IsActive)
                    .ToList();
                return Page();
            }
            var existingArea = _context.InvolvementAreas.Find(EditInvolvementArea.InvolvementAreaID);
            if (existingArea == null) return NotFound();

            existingArea.AreaOfInvolvement = EditInvolvementArea.AreaOfInvolvement;
            existingArea.Description = EditInvolvementArea.Description;
            existingArea.UpdatedDate = DateTime.UtcNow;

            _context.SaveChanges();
            return RedirectToPage(); // Redirect to the same page to refresh the list
        }

        public IActionResult OnPostCancelEdit()
        {
            return RedirectToPage(); // Simply redirect to the same page to cancel editing
        }

        public IActionResult OnPostDelete(int id)
        {
            var involvementArea = _context.InvolvementAreas.Find(id);
            if (involvementArea != null)
            {
                involvementArea.IsActive = false; // Soft delete by setting IsActive to false
                involvementArea.UpdatedDate = DateTime.UtcNow;
                _context.SaveChanges();
            }
            return RedirectToPage(); // Redirect to the same page to refresh the list
        }
    }
}
