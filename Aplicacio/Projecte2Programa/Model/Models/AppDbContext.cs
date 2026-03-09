using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Model.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accio> Accios { get; set; }

    public virtual DbSet<CanviEstat> CanviEstats { get; set; }

    public virtual DbSet<Efecte> Efectes { get; set; }

    public virtual DbSet<Habilitat> Habilitats { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Modificador> Modificadors { get; set; }

    public virtual DbSet<Objectiu> Objectius { get; set; }

    public virtual DbSet<Personatge> Personatges { get; set; }

    public virtual DbSet<TaulaEstat> TaulaEstats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=pollastre;uid=admin;password=admin", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Accio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("accio");

            entity.HasIndex(e => e.IdPersonatge, "fk_accio_personatge");

            entity.HasIndex(e => e.Id, "id").IsUnique();

            entity.Property(e => e.Id)
                .HasPrecision(3)
                .HasColumnName("id");
            entity.Property(e => e.IdPersonatge)
                .HasPrecision(3)
                .HasColumnName("id_personatge");

            entity.HasOne(d => d.IdPersonatgeNavigation).WithMany(p => p.Accios)
                .HasForeignKey(d => d.IdPersonatge)
                .HasConstraintName("fk_accio_personatge");
        });

        modelBuilder.Entity<CanviEstat>(entity =>
        {
            entity.HasKey(e => e.IdEfecte).HasName("PRIMARY");

            entity.ToTable("canvi_estat");

            entity.HasIndex(e => e.IdEstat, "fk_canviestat_taulaestats");

            entity.HasIndex(e => e.IdEfecte, "id_efecte").IsUnique();

            entity.Property(e => e.IdEfecte)
                .HasPrecision(3)
                .HasColumnName("id_efecte");
            entity.Property(e => e.IdEstat)
                .HasPrecision(2)
                .HasColumnName("id_estat");

            entity.HasOne(d => d.IdEfecteNavigation).WithOne(p => p.CanviEstat)
                .HasForeignKey<CanviEstat>(d => d.IdEfecte)
                .HasConstraintName("fk_canviestat_efecte");

            entity.HasOne(d => d.IdEstatNavigation).WithMany(p => p.CanviEstats)
                .HasForeignKey(d => d.IdEstat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_canviestat_taulaestats");
        });

        modelBuilder.Entity<Efecte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("efecte");

            entity.HasIndex(e => e.IdAccio, "fk_efecte_accio");

            entity.HasIndex(e => e.IdObjectiu, "fk_efecte_objectiu");

            entity.HasIndex(e => e.Id, "id").IsUnique();

            entity.Property(e => e.Id)
                .HasPrecision(3)
                .HasColumnName("id");
            entity.Property(e => e.Descripcio)
                .HasMaxLength(255)
                .HasColumnName("descripcio");
            entity.Property(e => e.IdAccio)
                .HasPrecision(3)
                .HasColumnName("id_accio");
            entity.Property(e => e.IdObjectiu)
                .HasPrecision(1)
                .HasColumnName("id_objectiu");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.Probabilitat)
                .HasPrecision(3)
                .HasDefaultValueSql("'100'")
                .HasColumnName("probabilitat");

            entity.HasOne(d => d.IdAccioNavigation).WithMany(p => p.Efectes)
                .HasForeignKey(d => d.IdAccio)
                .HasConstraintName("fk_efecte_accio");

            entity.HasOne(d => d.IdObjectiuNavigation).WithMany(p => p.Efectes)
                .HasForeignKey(d => d.IdObjectiu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_efecte_objectiu");
        });

        modelBuilder.Entity<Habilitat>(entity =>
        {
            entity.HasKey(e => e.IdAccio).HasName("PRIMARY");

            entity.ToTable("habilitat");

            entity.HasIndex(e => e.IdAccio, "id_accio").IsUnique();

            entity.Property(e => e.IdAccio)
                .HasPrecision(3)
                .HasColumnName("id_accio");

            entity.HasOne(d => d.IdAccioNavigation).WithOne(p => p.Habilitat)
                .HasForeignKey<Habilitat>(d => d.IdAccio)
                .HasConstraintName("fk_habilitat_accio");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.IdAccio).HasName("PRIMARY");

            entity.ToTable("item");

            entity.HasIndex(e => e.IdAccio, "id_accio").IsUnique();

            entity.Property(e => e.IdAccio)
                .HasPrecision(3)
                .HasColumnName("id_accio");

            entity.HasOne(d => d.IdAccioNavigation).WithOne(p => p.Item)
                .HasForeignKey<Item>(d => d.IdAccio)
                .HasConstraintName("fk_item_accio");
        });

        modelBuilder.Entity<Modificador>(entity =>
        {
            entity.HasKey(e => e.IdEfecte).HasName("PRIMARY");

            entity.ToTable("modificador");

            entity.HasIndex(e => e.IdEfecte, "id_efecte").IsUnique();

            entity.Property(e => e.IdEfecte)
                .HasPrecision(3)
                .HasColumnName("id_efecte");
            entity.Property(e => e.DuracioTorns)
                .HasPrecision(1)
                .HasDefaultValueSql("'0'")
                .HasComment("si es zero es instantani")
                .HasColumnName("duracio_torns");
            entity.Property(e => e.Estadistica)
                .HasMaxLength(20)
                .HasComment("nom exacte de la estadistica")
                .HasColumnName("estadistica");
            entity.Property(e => e.Quantitat)
                .HasPrecision(3)
                .HasColumnName("quantitat");

            entity.HasOne(d => d.IdEfecteNavigation).WithOne(p => p.Modificador)
                .HasForeignKey<Modificador>(d => d.IdEfecte)
                .HasConstraintName("fk_modificador_efecte");
        });

        modelBuilder.Entity<Objectiu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("objectiu");

            entity.HasIndex(e => e.Id, "id").IsUnique();

            entity.Property(e => e.Id)
                .HasPrecision(1)
                .HasColumnName("id");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasComment("aliat, yo, enemic, aliats, enemics")
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Personatge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("personatge");

            entity.HasIndex(e => e.Id, "id").IsUnique();

            entity.Property(e => e.Id)
                .HasPrecision(3)
                .HasColumnName("id");
            entity.Property(e => e.Atac)
                .HasPrecision(3)
                .HasColumnName("atac");
            entity.Property(e => e.Defensa)
                .HasPrecision(3)
                .HasColumnName("defensa");
            entity.Property(e => e.Descripcio)
                .HasMaxLength(255)
                .HasColumnName("descripcio");
            entity.Property(e => e.Experiencia)
                .HasPrecision(3)
                .HasColumnName("experiencia");
            entity.Property(e => e.Imatge)
                .HasColumnType("blob")
                .HasColumnName("imatge");
            entity.Property(e => e.Jugable).HasColumnName("jugable");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.Velocitat)
                .HasPrecision(3)
                .HasColumnName("velocitat");
            entity.Property(e => e.Vida)
                .HasPrecision(3)
                .HasColumnName("vida");
        });

        modelBuilder.Entity<TaulaEstat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("taula_estats");

            entity.HasIndex(e => e.Id, "id").IsUnique();

            entity.Property(e => e.Id)
                .HasPrecision(2)
                .HasColumnName("id");
            entity.Property(e => e.Descripcio)
                .HasMaxLength(255)
                .HasColumnName("descripcio");
            entity.Property(e => e.Imatge)
                .HasMaxLength(255)
                .HasColumnName("imatge");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
