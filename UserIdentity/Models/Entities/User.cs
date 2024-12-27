namespace ConferenciaTelecall.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string LoginUsuario { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public string Salt { get; set; }
        public int SetorId { get; set; }
        public bool Ativo { get; set; }
        
        public Department Setor { get; set; }
    }
}