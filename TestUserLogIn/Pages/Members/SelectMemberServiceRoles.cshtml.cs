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
    public class SelectMemberServiceRolesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public SelectMemberServiceRolesModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public List<InvolvementAreas> AllInvolvements { get; set; } = new();
        [BindProperty]
        public List<int> SelectedInvolvementIds { get; set; } = new();
        public async Task<IActionResult> OnGetAsync()
        {
            AllInvolvements = await _context.InvolvementAreas
                .Where(i => i.IsActive)
                .OrderBy(i => i.AreaOfInvolvement)
                .ToListAsync();
            // Load member’s current selections
            var user = await _userManager.GetUserAsync(User);
            if (user?.MemberID != null)
            {
                SelectedInvolvementIds = await _context.MemberServiceRoles
                    .Where(mi => mi.MemberID == user.MemberID)
                    .Select(mi => mi.InvolvementAreaID)
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

            // Remove existing involvements
            var existing = _context.MemberServiceRoles.Where(mi => mi.MemberID == memberId);
            _context.MemberServiceRoles.RemoveRange(existing);

            // Add new selections
            foreach (var involvementId in SelectedInvolvementIds)
            {
                _context.MemberServiceRoles.Add(new MemberServiceRole
                {
                    MemberID = memberId,
                    InvolvementAreaID = involvementId,
                    CreatedDate = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("/Members/MemberInfo");
        }
    }
}
