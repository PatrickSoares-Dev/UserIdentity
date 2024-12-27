namespace UserIdentity.Models.DTOs.User
{
    public class UserUpdateDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string LoginUsuario { get; set; }
        public string Email { get; set; }
        public int SetorId { get; set; }
    }
}