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
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        public string? LastName { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }

        public string? CellPhoneNumber { get; set; }

        public string? HomePhoneNumber { get; set; }

        public string? WorkPhoneNumber { get; set; }

        public string? Email { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Sex { get; set; }

        public string? Comments { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string ApplicationUserID { get; set; } = string.Empty;

        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<MemberInterest>? MemberInterests { get; set; }
        public ICollection<MemberInvolvement>? MemberInvolvements { get; set; }
        public ICollection<MemberServiceRole>? MemberServiceRoles { get; set; }

    }
}
