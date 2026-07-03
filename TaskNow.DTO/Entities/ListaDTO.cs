namespace TaskNow.DTO.Entities;

public class ListaDTO
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public List<CartaoDTO> Cartoes { get; set; } = [];
}