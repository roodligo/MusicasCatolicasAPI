using Microsoft.EntityFrameworkCore;
using MusicasCatolicasAPI.Models;

namespace MusicasCatolicasAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Musica> Musicas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<SubCategoria> SubCategorias { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=MusicData.db")
                  .EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Musica>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Musica>()
                .HasOne(p => p.SubCategoria)
                .WithMany()
                .HasForeignKey(p => p.SubCategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Musica>()
                .HasOne(p => p.SubCategoria)
                .WithMany()
                .HasForeignKey(p => p.SubCategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
