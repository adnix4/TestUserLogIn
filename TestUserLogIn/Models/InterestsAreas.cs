using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestUserLogIn.Models
{
    public class InterestsAreas
    {
        [Key]
        public int InterestAreaID { get; set; }
        [Required(ErrorMessage = "Interest Area is required")]
        [Display(Name = "Interest Area")]
        public required string InterestArea { get; set; }
        [Display(Name = "Description")]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public ICollection<MemberInterest>? MemberInterests { get; set; }
    }
}
