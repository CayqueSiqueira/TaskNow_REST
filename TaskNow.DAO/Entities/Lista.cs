namespace TaskNow.DAO.Entities;

public class Lista
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }

    public Quadro? Quadro { get; set; }
    public List<Cartao> Cartoes { get; set; } = [];
}