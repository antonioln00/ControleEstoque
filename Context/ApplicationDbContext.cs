using Microsoft.EntityFrameworkCore;
using ControleEstoque.Entities;

namespace ControleEstoque.Context;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estoque>().HasKey(e => e.Id);
        modelBuilder.Entity<Fornecedor>().HasKey(e => e.Id);
        modelBuilder.Entity<Produto>().HasKey(e => e.Id);

        modelBuilder.Entity<Estoque>().Property(e => e.Quantidade).HasPrecision(18, 5);
        modelBuilder.Entity<Produto>().Property(e => e.Preco).HasPrecision(18, 5);

        modelBuilder
            .Entity<Produto>() // na entidade produto
            .HasOne(e=> e.Fornecedor) // tenho um fornecedor
            .WithMany(e => e.Produtos) // para muitos produtos
            .HasForeignKey(e => e.FornecedorId) // referenciado pela chave estrangeira FornecedorId
            .IsRequired();

        modelBuilder
            .Entity<Estoque>() // na tabela estoque
            .HasOne(e => e.Produto) // tem um produto
            .WithOne() // para um estoque
            .HasForeignKey<Estoque>(e => e.ProdutoId) // referenciado pela chave estrangeira ProdutoId
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer("DefaultConnection");
        base.OnConfiguring(optionsBuilder);
    }
}
