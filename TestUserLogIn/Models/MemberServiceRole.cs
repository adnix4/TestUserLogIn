namespace TestUserLogIn.Models
{
    public class MemberServiceRole
    {
        public int MemberID { get; set; }
        public MemberInfo? Member { get; set; }

        public int ServiceRoleID { get; set; }
        public ServiceRoles? ServiceRole { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public ICollection<MemberServiceRole>? MemberServiceRoles { get; set; }
    }
}
