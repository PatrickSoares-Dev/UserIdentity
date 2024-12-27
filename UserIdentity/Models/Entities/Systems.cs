using ConferenciaTelecall.Models.Entities;

public class Systems
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string? Url { get; set; } 
    public ICollection<UserSystemRole> UserSystemRoles { get; set; }
}