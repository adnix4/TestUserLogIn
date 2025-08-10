using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestUserLogIn.Models
{
    public class InvolvementAreas
    {
        [Key]
        public int InvolvementAreaID { get; set; }
        [Required(ErrorMessage = "Area of Involvement is required")]
        [Display(Name = "Area of Involvement")]
        public required string AreaOfInvolvement { get; set; }
        [Display(Name = "Description")]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public ICollection<MemberInvolvement>? MemberInvolvements { get; set; }
    }
}
