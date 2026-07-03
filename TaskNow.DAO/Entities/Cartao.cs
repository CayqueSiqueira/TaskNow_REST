namespace TaskNow.DAO.Entities;

public class Cartao
{
    public int Id { get; set; }
    public int ListaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Ordem { get; set; }
    public DateTime? Prazo { get; set; }
    public string? ResponsavelId { get; set; }

    public Lista? Lista { get; set; }
    public ApplicationUser? Responsavel { get; set; }
    public List<CartaoEtiqueta> Etiquetas { get; set; } = [];
    public List<Comentario> Comentarios { get; set; } = [];
}