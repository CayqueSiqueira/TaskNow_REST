namespace TaskNow.DTO.Entities;

public class MembroQuadroDTO
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string Papel { get; set; } = string.Empty;
}