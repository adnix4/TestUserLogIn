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
    public class InterestAreasModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InterestAreasModel> _logger;

        public InterestAreasModel(ApplicationDbContext context, ILogger<InterestAreasModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InterestAreas NewInterestArea { get; set; } = new();

        [BindProperty]
        public InterestAreas? EditInterestArea { get; set; }

        [BindProperty]
        public int? EditInterestAreaId { get; set; }

        public List<InterestAreas> InterestAreasList { get; set; } = new();
        public void OnGet()
        {
            LoadAreas();
        }

        // Add new interest area
        public IActionResult OnPost()
        {
            FixModelStateBinding("NewInterestArea", "InterestArea", NewInterestArea?.InterestArea);
            ModelState.Remove("EditInterestArea.InterestArea");

            if (!ModelStateCheck("OnPost NewInterestArea"))
                return Page();

            try
            {
                var existing = _context.InterestsAreas
                    .FirstOrDefault(ia => EF.Functions.Like(ia.InterestArea, NewInterestArea.InterestArea));
                if (existing != null)
                {
                    if (existing.IsActive)
                    {
                        ModelState.AddModelError("NewInterestArea.InterestArea",
                            "An active Interest Area with this name already exists.");
                        return Page();
                    }

                    // Reactivate inactive area
                    existing.IsActive = true;
                    existing.Description = NewInterestArea.Description;
                    existing.UpdatedDate = DateTime.UtcNow;
                    _context.SaveChanges();

                    _logger.LogInformation("Reactivated Interest Area: {InterestArea}", existing.InterestArea);
                    return RedirectToPage();

                }

                // Add new area
                NewInterestArea.CreatedDate = DateTime.UtcNow;
                NewInterestArea.IsActive = true;

                _context.InterestsAreas.Add(NewInterestArea);
                _context.SaveChanges();

                _logger.LogInformation("Added new Interest Area: {InterestArea}", NewInterestArea.InterestArea);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new Interest Area");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the Interest Area.");
                LoadAreas();
                return Page();
            }
        }

        // Load edit form
        public IActionResult OnPostEdit(int id)
        {
            var existingArea = _context.InterestsAreas.Find(id);

            if (existingArea == null)
            {
                _logger.LogWarning("Interest Area with ID {Id} not found for editing", id);
                return NotFound();
            }

            EditInterestAreaId = id;
            EditInterestArea = existingArea;

            LoadAreas();
            return Page();
        }

        // Helper: Load areas
        private void LoadAreas()
        {
            InterestAreasList = _context.InterestsAreas
                .OrderBy(ia => ia.InterestArea)
                .ToList();
        }

        // ModelState debugger
        private bool ModelStateCheck(string contextName = "")
        {
            if (ModelState.IsValid) return true;

            _logger.LogWarning("ModelState is invalid in {Context}. Submitted values: {@Values}",
                contextName,
                Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString()));


            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    _logger.LogWarning("ModelState error in {Context}: Field '{Field}' - Error: {Error}",
                        contextName, error.Field, error.Error);
                }
                return false;
            }


        }
    }
        
    }
