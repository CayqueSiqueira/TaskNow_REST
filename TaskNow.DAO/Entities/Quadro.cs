namespace TaskNow.DAO.Entities;

public class Quadro
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string DonoId { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ApplicationUser? Dono { get; set; }
    public List<Lista> Listas { get; set; } = [];
    public List<MembroQuadro> Membros { get; set; } = [];
    public List<Etiqueta> Etiquetas { get; set; } = [];
}