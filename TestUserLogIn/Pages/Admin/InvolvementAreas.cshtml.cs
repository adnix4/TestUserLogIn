using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TestUserLogIn.Data;
using TestUserLogIn.Models;

namespace TestUserLogIn.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class InvolvementAreasModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvolvementAreasModel> _logger;

        public InvolvementAreasModel(ApplicationDbContext context, ILogger<InvolvementAreasModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InvolvementAreas NewInvolvementArea { get; set; } = new();

        [BindProperty]
        public InvolvementAreas? EditInvolvementArea { get; set; }

        [BindProperty]
        public int? EditInvolvementAreaId { get; set; }

        public List<InvolvementAreas> InvolvementAreasList { get; set; } = new();

        public void OnGet()
        {
            LoadActiveAreas();
        }

        // Add new involvement area
        public IActionResult OnPost()
        {
            Console.WriteLine($"Form keys received: {string.Join(", ", Request.Form.Keys)}");
            if (NewInvolvementArea?.AreaOfInvolvement != null)
            {
                ModelState.Remove("AreaOfInvolvement");
                ModelState.SetModelValue("NewInvolvementArea.AreaOfInvolvement",
                    new ValueProviderResult(
                    NewInvolvementArea.AreaOfInvolvement));
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine($"ModelState is invalid.{ModelState}");
                Console.WriteLine($"NewInvolvementArea.AreaOfInvolvement: {NewInvolvementArea?.AreaOfInvolvement}");
                Console.WriteLine($"NewInvolvementArea.Description: {NewInvolvementArea?.Description}");
                foreach (var modelState in ModelState)
                {
                    Console.WriteLine($"Key: {modelState.Key}, IsValid: {modelState.Value.ValidationState}");
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }

                LoadActiveAreas();
                return Page();
            }

            try
            {
                // Case-insensitive lookup
                var areaName = NewInvolvementArea.AreaOfInvolvement;
                var existing = _context.InvolvementAreas
                    .FirstOrDefault(ia => EF.Functions.Like(ia.AreaOfInvolvement, areaName));

                if (existing != null)
                {
                    if (existing.IsActive)
                    {
                        ModelState.AddModelError("NewInvolvementArea.AreaOfInvolvement",
                            "This involvement area already exists.");
                        LoadActiveAreas();
                        return Page();
                    }

                    // Reactivate inactive area
                    existing.IsActive = true;
                    existing.Description = NewInvolvementArea.Description; // optional
                    existing.UpdatedDate = DateTime.UtcNow;

                    _context.SaveChanges();
                    _logger.LogInformation("Reactivated involvement area {Area}", existing.AreaOfInvolvement);

                    return RedirectToPage();
                }

                // Create new
                NewInvolvementArea.CreatedDate = DateTime.UtcNow;
                NewInvolvementArea.IsActive = true;

                _context.InvolvementAreas.Add(NewInvolvementArea);
                _context.SaveChanges();

                _logger.LogInformation("Added new involvement area {Area}", NewInvolvementArea.AreaOfInvolvement);

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving new involvement area");
                ModelState.AddModelError(string.Empty, "Error saving the involvement area.");
                LoadActiveAreas();
                return Page();
            }
        }

        // Load edit form
        public IActionResult OnPostEdit(int id)
        {
            var existingArea = _context.InvolvementAreas.Find(id);
            if (existingArea == null) return NotFound();

            EditInvolvementAreaId = id;
            EditInvolvementArea = existingArea;

            LoadActiveAreas();
            return Page();
        }

        // Save edit
        public IActionResult OnPostSaveEdit()
        {
            if (EditInvolvementArea == null) return BadRequest();

            if (!ModelState.IsValid)
            {
                LoadActiveAreas();
                return Page();
            }

            var existingArea = _context.InvolvementAreas.Find(EditInvolvementArea.InvolvementAreaID);
            if (existingArea == null) return NotFound();

            // Prevent duplicate names
            var duplicate = _context.InvolvementAreas
                .FirstOrDefault(ia =>
                    ia.InvolvementAreaID != EditInvolvementArea.InvolvementAreaID &&
                    ia.AreaOfInvolvement.Equals(EditInvolvementArea.AreaOfInvolvement, StringComparison.OrdinalIgnoreCase) &&
                    ia.IsActive);

            if (duplicate != null)
            {
                ModelState.AddModelError("EditInvolvementArea.AreaOfInvolvement",
                    "Another active involvement area already has this name.");
                LoadActiveAreas();
                return Page();
            }

            existingArea.AreaOfInvolvement = EditInvolvementArea.AreaOfInvolvement;
            existingArea.Description = EditInvolvementArea.Description;
            existingArea.UpdatedDate = DateTime.UtcNow;

            _context.SaveChanges();

            _logger.LogInformation("Edited involvement area {Area}", existingArea.AreaOfInvolvement);

            return RedirectToPage();
        }

        public IActionResult OnPostCancelEdit()
        {
            return RedirectToPage();
        }

        // Soft delete
        public IActionResult OnPostDelete(int id)
        {
            var involvementArea = _context.InvolvementAreas.Find(id);
            if (involvementArea != null)
            {
                involvementArea.IsActive = false;
                involvementArea.UpdatedDate = DateTime.UtcNow;
                _context.SaveChanges();

                _logger.LogInformation("Soft deleted involvement area {Area}", involvementArea.AreaOfInvolvement);
            }

            return RedirectToPage();
        }
        public IActionResult OnPostReactivate(int id)
        {
            var involvementArea = _context.InvolvementAreas.Find(id);
            if (involvementArea != null && !involvementArea.IsActive)
            {
                involvementArea.IsActive = true;
                involvementArea.UpdatedDate = DateTime.UtcNow;
                _context.SaveChanges();
            }
            return RedirectToPage();
        }

        // Helper: Load only active areas
        private void LoadActiveAreas()
        {
            InvolvementAreasList = _context.InvolvementAreas
                .Where(ia => ia.IsActive)
                .OrderBy(ia => ia.AreaOfInvolvement)
                .ToList();
        }
    }
}
