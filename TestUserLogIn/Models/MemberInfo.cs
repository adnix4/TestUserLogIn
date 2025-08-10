using System;
using System.ComponentModel.DataAnnotations;
using TestUserLogIn.Data;

namespace TestUserLogIn.Models
{
    public class MemberInfo
    {
        [Key]
        public int MemberID { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        public required string LastName { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Sex { get; set; }

        public string? Comments { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public required ApplicationUser ApplicationUser { get; set; }
        public ICollection<MemberInterest>? MemberInterests { get; set; }
        public ICollection<MemberInvolvement>? MemberInvolvements { get; set; }
        public ICollection<MemberServiceRole>? MemberServiceRoles { get; set; }

    }
}
