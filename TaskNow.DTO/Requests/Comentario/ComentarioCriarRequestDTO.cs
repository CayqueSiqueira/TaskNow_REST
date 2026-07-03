namespace TaskNow.DTO.Requests.Comentario;

public class ComentarioCriarRequestDTO
{
    public int CartaoId { get; set; }
    public string Texto { get; set; } = string.Empty;
}