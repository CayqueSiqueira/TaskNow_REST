namespace TaskNow.DTO.Entities;

public class QuadroDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string DonoId { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}