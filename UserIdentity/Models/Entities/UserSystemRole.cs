namespace ConferenciaTelecall.Models.Entities
{
    public class UserSystemRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SystemId { get; set; }
        public int RoleId { get; set; }

        public User User { get; set; }
        public Systems Systems { get; set; } 
        public Role Role { get; set; }
    }
}