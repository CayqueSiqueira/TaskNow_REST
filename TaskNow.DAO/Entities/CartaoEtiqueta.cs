namespace TaskNow.DAO.Entities;

public class CartaoEtiqueta
{
    public int CartaoId { get; set; }
    public int EtiquetaId { get; set; }

    public Cartao? Cartao { get; set; }
    public Etiqueta? Etiqueta { get; set; }
}