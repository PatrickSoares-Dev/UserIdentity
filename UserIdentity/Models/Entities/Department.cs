namespace ConferenciaTelecall.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public ICollection<User> Users { get; set; }
    }
}