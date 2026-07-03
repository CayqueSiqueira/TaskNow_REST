namespace TaskNow.DTO.Utils;

public class PagedResultDTO<T>
{
    public IReadOnlyList<T> Itens { get; set; } = [];
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int Total { get; set; }
}