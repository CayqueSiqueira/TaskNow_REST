namespace TaskNow.DAO.Entities;

public class MembroQuadro
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public PapelMembro Papel { get; set; }

    public Quadro? Quadro { get; set; }
    public ApplicationUser? Usuario { get; set; }
}