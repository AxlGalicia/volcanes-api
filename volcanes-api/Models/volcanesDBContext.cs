using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace volcanes_api.Models
{
    public partial class volcanesDBContext : DbContext
    {
        public volcanesDBContext()
        {
        }

        public volcanesDBContext(DbContextOptions<volcanesDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Volcan> Volcans { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Volcan>(entity =>
            {
                entity.ToTable("volcan");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Altura).HasColumnName("altura");

                entity.Property(e => e.Descripcion)
                    .HasColumnType("text")
                    .HasColumnName("descripcion");

                entity.Property(e => e.Ecosistema)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ecosistema");

                entity.Property(e => e.Imagen)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("imagen");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("nombre");

                entity.Property(e => e.Ubicacion)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ubicacion");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
