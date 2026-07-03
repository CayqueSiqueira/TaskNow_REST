namespace TaskNow.DTO.Requests.Cartao;

public class CartaoEditarRequestDTO
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime? Prazo { get; set; }
    public string? ResponsavelId { get; set; }
}