namespace TaskNow.DTO.Requests.Cartao;

public class CartaoCriarRequestDTO
{
    public int ListaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime? Prazo { get; set; }
    public string? ResponsavelId { get; set; }
}