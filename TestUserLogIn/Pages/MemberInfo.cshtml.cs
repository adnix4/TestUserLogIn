using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TestUserLogIn.Data;

namespace TestUserLogIn.Pages
{
    [Authorize]
    public class MemberInfoModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public MemberInfoModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Required]
        public string UserName { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                UserName = user.UserName;
                Roles = await _userManager.GetRolesAsync(user);
            }
            else
            {
                UserName = "Unknown User";
            }
        }
        
    }
}
