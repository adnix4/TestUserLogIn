namespace TestUserLogIn.Models
{
    public class MemberInterest
    {
        public int MemberID { get; set; }
        public MemberInfo? Member { get; set; }

        public int InterestAreaID { get; set; }
        public InterestAreas? InterestArea { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public ICollection<MemberInterest>? MemberInterests { get; set; }
    }
}
