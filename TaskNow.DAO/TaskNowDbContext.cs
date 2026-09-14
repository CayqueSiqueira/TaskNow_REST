using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAO.Entities;

namespace TaskNow.DAO;

public class TaskNowDbContext(DbContextOptions<TaskNowDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Quadro> Quadros => Set<Quadro>();
    public DbSet<MembroQuadro> MembrosQuadro => Set<MembroQuadro>();
    public DbSet<Lista> Listas => Set<Lista>();
    public DbSet<Cartao> Cartoes => Set<Cartao>();
    public DbSet<Etiqueta> Etiquetas => Set<Etiqueta>();
    public DbSet<CartaoEtiqueta> CartoesEtiquetas => Set<CartaoEtiqueta>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<AtividadeCartao> AtividadesCartao => Set<AtividadeCartao>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Quadro>(entity =>
        {
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(500);
            entity.HasOne(x => x.Dono).WithMany().HasForeignKey(x => x.DonoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Lista>(entity =>
        {
            entity.Property(x => x.Nome).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => new { x.QuadroId, x.Ordem });
            entity.HasOne(x => x.Quadro).WithMany(x => x.Listas).HasForeignKey(x => x.QuadroId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Cartao>(entity =>
        {
            entity.Property(x => x.Titulo).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(2000);
            entity.HasIndex(x => new { x.ListaId, x.Ordem });
            entity.HasOne(x => x.Lista).WithMany(x => x.Cartoes).HasForeignKey(x => x.ListaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Responsavel).WithMany().HasForeignKey(x => x.ResponsavelId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Etiqueta>(entity =>
        {
            entity.Property(x => x.Nome).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Cor).HasMaxLength(7).IsRequired();
            entity.HasOne(x => x.Quadro).WithMany(x => x.Etiquetas).HasForeignKey(x => x.QuadroId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CartaoEtiqueta>(entity =>
        {
            entity.HasKey(x => new { x.CartaoId, x.EtiquetaId });
            entity.HasOne(x => x.Cartao).WithMany(x => x.Etiquetas).HasForeignKey(x => x.CartaoId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Etiqueta).WithMany(x => x.Cartoes).HasForeignKey(x => x.EtiquetaId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Comentario>(entity =>
        {
            entity.Property(x => x.Texto).HasMaxLength(2000).IsRequired();
            entity.HasOne(x => x.Cartao).WithMany(x => x.Comentarios).HasForeignKey(x => x.CartaoId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Autor).WithMany().HasForeignKey(x => x.AutorId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MembroQuadro>(entity =>
        {
            entity.HasIndex(x => new { x.QuadroId, x.UsuarioId }).IsUnique();
            entity.HasOne(x => x.Quadro).WithMany(x => x.Membros).HasForeignKey(x => x.QuadroId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AtividadeCartao>(entity =>
        {
            entity.Property(x => x.Tipo).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(500).IsRequired();
            // Se o cartao for deletado, apaga o historico em cascata
            entity.HasOne(x => x.Cartao).WithMany().HasForeignKey(x => x.CartaoId).OnDelete(DeleteBehavior.Cascade);
            // Se o usuario for deletado, deixa o historico orfao (null) para nao apagar os rastros do cartao
            entity.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}