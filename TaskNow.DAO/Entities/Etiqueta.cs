namespace TaskNow.DAO.Entities;

public class Etiqueta
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;

    public Quadro? Quadro { get; set; }
    public List<CartaoEtiqueta> Cartoes { get; set; } = [];
}