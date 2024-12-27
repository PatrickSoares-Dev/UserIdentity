namespace UserIdentity.Models.DTOs.User
{
    public class UserInfoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string LoginUsuario { get; set; }
        public string Email { get; set; }
        public string Setor { get; set; }
    }
}