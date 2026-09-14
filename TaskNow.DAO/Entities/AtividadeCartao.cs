using System;

namespace TaskNow.DAO.Entities;

public class AtividadeCartao
{
    public int Id { get; set; }
    public int CartaoId { get; set; }
    public string? UsuarioId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Cartao? Cartao { get; set; }
    public ApplicationUser? Usuario { get; set; }
}
