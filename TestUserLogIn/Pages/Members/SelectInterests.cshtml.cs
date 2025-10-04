using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TestUserLogIn.Data;
using TestUserLogIn.Models;

namespace TestUserLogIn.Pages.Members
{
    [Authorize]
    public class SelectInterestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SelectInterestsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<InterestAreas> AllInterests { get; set; } = new();
        [BindProperty]
        public List<int> SelectedInterestIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            AllInterests = await _context.InterestAreas
                .Where(i => i.IsActive)
                .OrderBy(i => i.InterestArea)
                .ToListAsync();

            // Load member’s current selections
            var user = await _userManager.GetUserAsync(User);
            if (user?.MemberID != null)
            {
                SelectedInterestIds = await _context.MemberInterests
                    .Where(mi => mi.MemberID == user.MemberID)
                    .Select(mi => mi.InterestAreaID)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.MemberID == null)
                return Unauthorized();

            int memberId = user.MemberID.Value;

            // Remove existing interests
            var existing = _context.MemberInterests.Where(mi => mi.MemberID == memberId);
            _context.MemberInterests.RemoveRange(existing);

            // Add new selections
            foreach (var interestId in SelectedInterestIds)
            {
                _context.MemberInterests.Add(new MemberInterest
                {
                    MemberID = memberId,
                    InterestAreaID = interestId,
                    CreatedDate = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            // Redirect or confirmation message
            return RedirectToPage("/Members/MemberInfo");
        }
    }
}

