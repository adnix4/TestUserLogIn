using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TestUserLogIn.Data;
using TestUserLogIn.Models;

namespace TestUserLogIn.Pages.Members
{
    [Authorize(Roles = "RegisteredUser")]
    public class SelectMemberInvolvementModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
