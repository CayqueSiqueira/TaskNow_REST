namespace TaskNow.DTO.Entities;

public class CartaoDTO
{
    public int Id { get; set; }
    public int ListaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Ordem { get; set; }
    public DateTime? Prazo { get; set; }
    public string? ResponsavelId { get; set; }
    public string? ResponsavelNome { get; set; }
    public string? ResponsavelEmail { get; set; }
    public List<EtiquetaDTO> Etiquetas { get; set; } = [];
}