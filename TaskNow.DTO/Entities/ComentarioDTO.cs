namespace TaskNow.DTO.Entities;

public class ComentarioDTO
{
    public int Id { get; set; }
    public int CartaoId { get; set; }
    public string AutorId { get; set; } = string.Empty;
    public string AutorNome { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}