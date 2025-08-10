using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TestUserLogIn.Data;

namespace TestUserLogIn.Pages.Members
{
    [Authorize(Roles = "RegisteredUser")]
    public class InterestsModel : PageModel
    {
        
        public void OnGet()
        {
        }
    }
}
