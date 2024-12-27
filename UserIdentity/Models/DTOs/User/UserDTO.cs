namespace UserIdentity.Models.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string LoginUsuario { get; set; }
        public int RoleId { get; set; }
        public string RoleNome { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentNome { get; set; }
        public int SistemaId { get; set; }
        public string SistemaNome { get; set; }
    }
}