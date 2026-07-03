namespace TaskNow.DAO.Entities;

public class Comentario
{
    public int Id { get; set; }
    public int CartaoId { get; set; }
    public string AutorId { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Cartao? Cartao { get; set; }
    public ApplicationUser? Autor { get; set; }
}