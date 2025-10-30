using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TestUserLogIn.Data;
using TestUserLogIn.Models;

namespace TestUserLogIn.Pages
{
    [Authorize]
    public class MemberInfoModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MemberInfoModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public MemberInfoModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            ILogger<MemberInfoModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public MemberInfo MemberDetails { get; set; } = new();

        public IList<string> Roles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Load user roles
            Roles = await _userManager.GetRolesAsync(user);

            // Load existing MemberInfo if it exists
            var memberInfo = await _context.MemberInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ApplicationUser.Id == user.Id);

            if (memberInfo != null)
            {
                MemberDetails = memberInfo;
            }
            else
            {
                MemberDetails = await _context.MemberInfos
                    .FirstOrDefaultAsync(m => m.MemberID == user.MemberID) ?? new MemberInfo
                {
                    FirstName = "",
                    LastName = "",
                    Email = user.Email,
                    ApplicationUser = user,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (!ModelState.IsValid)
            {
                Roles = await _userManager.GetRolesAsync(user);
                return Page();
            }

            try
            {
                var existing = await _context.MemberInfos
                    .FirstOrDefaultAsync(m => m.ApplicationUser.Id == user.Id);

                if (existing == null)
                {
                    // New profile
                    MemberDetails.ApplicationUser = user;
                    MemberDetails.CreatedDate = DateTime.UtcNow;
                    MemberDetails.IsActive = true;

                    _context.MemberInfos.Add(MemberDetails);
                    _logger.LogInformation("Created new MemberInfo for {User}", user.UserName);
                }
                else
                {
                    // Update profile
                    existing.FirstName = MemberDetails.FirstName;
                    existing.LastName = MemberDetails.LastName;
                    existing.Address = MemberDetails.Address;
                    existing.City = MemberDetails.City;
                    existing.State = MemberDetails.State;
                    existing.Zip = MemberDetails.Zip;
                    existing.CellPhoneNumber = MemberDetails.CellPhoneNumber;
                    existing.HomePhoneNumber = MemberDetails.HomePhoneNumber;                    
                    existing.Email = MemberDetails.Email;
                    existing.PreferredContactEmail = MemberDetails.PreferredContactEmail;
                    existing.BirthDate = MemberDetails.BirthDate;
                    existing.Sex = MemberDetails.Sex;
                    existing.Comments = MemberDetails.Comments;
                    existing.PrefersPhone = MemberDetails.PrefersPhone;
                    existing.PrefersEmail = MemberDetails.PrefersEmail;
                    existing.PrefersText = MemberDetails.PrefersText;
                    existing.UpdatedDate = DateTime.UtcNow;

                    _context.MemberInfos.Update(existing);
                    _logger.LogInformation("Updated MemberInfo for {User}", user.UserName);
                }

                await _context.SaveChangesAsync();

                // Refresh claims so FirstName shows up in the nav bar.
                await _signInManager.RefreshSignInAsync(user);

                _logger.LogInformation("MemberInfo saved successfully for {User}", user.UserName);

                return RedirectToPage("/Members/SelectInterests"); //  redirect to SelectedIntersts
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving MemberInfo");
                ModelState.AddModelError(string.Empty, "There was a problem saving your information.");
                return Page();
            }
        }
    }
}

