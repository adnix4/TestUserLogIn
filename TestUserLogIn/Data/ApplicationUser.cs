using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace TestUserLogIn.Data
{
    public class ApplicationUser : IdentityUser
    {
       
        public string? FirstName { get; set; }   
        public string? LastName { get; set; }
        public DateOnly? BirthDate { get; set; }
       
        // Additional properties can be added as needed
    }
}
