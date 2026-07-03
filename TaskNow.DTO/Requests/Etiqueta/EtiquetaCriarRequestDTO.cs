namespace TaskNow.DTO.Requests.Etiqueta;

public class EtiquetaCriarRequestDTO
{
    public int QuadroId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
}