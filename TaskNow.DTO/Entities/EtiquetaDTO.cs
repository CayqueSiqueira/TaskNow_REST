namespace TaskNow.DTO.Entities;

public class EtiquetaDTO
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
}