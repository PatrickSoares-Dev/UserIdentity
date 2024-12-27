namespace UserIdentity.Models.DTOs.User
{
    public class CompleteUserCreationDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string LoginUsuario { get; set; }
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
        public int SistemaId { get; set; }
    }
}