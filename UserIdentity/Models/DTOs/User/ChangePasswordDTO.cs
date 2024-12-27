namespace UserIdentity.Models.DTOs.User
{
    public class ChangePasswordDTO
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }

}