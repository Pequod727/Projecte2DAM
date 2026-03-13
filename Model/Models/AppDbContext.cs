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

    public virtual DbSet<Nivell> Nivells { get; set; }

    public virtual DbSet<Objectiu> Objectius { get; set; }

    public virtual DbSet<Personatge> Personatges { get; set; }

    public virtual DbSet<TaulaEstat> TaulaEstats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var properties = new Dictionary<string, string>();

        var lines = File.ReadAllLines("app.properties");

        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    properties[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        string server = properties.GetValueOrDefault("db.server");
        string database = properties.GetValueOrDefault("db.database");
        string user = properties.GetValueOrDefault("db.user");
        string password = properties.GetValueOrDefault("db.password");
        string version = properties.GetValueOrDefault("db.version");

        string connectionString = $"server={server};database={database};uid={user};password={password}";
        optionsBuilder.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse(version));
    }

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
            entity.Property(e => e.Aplicar)
                .HasComment("TRUE per afegir estat, FALSE per curar")
                .HasColumnName("aplicar");
            entity.Property(e => e.DuracioTorns)
                .HasPrecision(1)
                .HasDefaultValueSql("'0'")
                .HasComment("0 és instantani")
                .HasColumnName("duracio_torns");
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
                .HasColumnName("duracio_torns");
            entity.Property(e => e.Estadistica)
                .HasMaxLength(20)
                .HasColumnName("estadistica");
            entity.Property(e => e.Quantitat)
                .HasPrecision(3)
                .HasColumnName("quantitat");

            entity.HasOne(d => d.IdEfecteNavigation).WithOne(p => p.Modificador)
                .HasForeignKey<Modificador>(d => d.IdEfecte)
                .HasConstraintName("fk_modificador_efecte");
        });

        modelBuilder.Entity<Nivell>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("nivell");

            entity.HasIndex(e => e.IdEnemic1, "fk_nivell_enemic1");

            entity.HasIndex(e => e.IdEnemic2, "fk_nivell_enemic2");

            entity.HasIndex(e => e.IdEnemic3, "fk_nivell_enemic3");

            entity.HasIndex(e => e.IdEnemic4, "fk_nivell_enemic4");

            entity.HasIndex(e => e.Id, "id").IsUnique();

            entity.HasIndex(e => e.Ordre, "ordre").IsUnique();

            entity.Property(e => e.Id)
                .HasPrecision(3)
                .HasColumnName("id");
            entity.Property(e => e.IdEnemic1)
                .HasPrecision(3)
                .HasColumnName("id_enemic_1");
            entity.Property(e => e.IdEnemic2)
                .HasPrecision(3)
                .HasColumnName("id_enemic_2");
            entity.Property(e => e.IdEnemic3)
                .HasPrecision(3)
                .HasColumnName("id_enemic_3");
            entity.Property(e => e.IdEnemic4)
                .HasPrecision(3)
                .HasColumnName("id_enemic_4");
            entity.Property(e => e.Ordre)
                .HasPrecision(2)
                .HasComment("Ordre d'aparició (max 99)")
                .HasColumnName("ordre");

            entity.HasOne(d => d.IdEnemic1Navigation).WithMany(p => p.NivellIdEnemic1Navigations)
                .HasForeignKey(d => d.IdEnemic1)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_nivell_enemic1");

            entity.HasOne(d => d.IdEnemic2Navigation).WithMany(p => p.NivellIdEnemic2Navigations)
                .HasForeignKey(d => d.IdEnemic2)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_nivell_enemic2");

            entity.HasOne(d => d.IdEnemic3Navigation).WithMany(p => p.NivellIdEnemic3Navigations)
                .HasForeignKey(d => d.IdEnemic3)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_nivell_enemic3");

            entity.HasOne(d => d.IdEnemic4Navigation).WithMany(p => p.NivellIdEnemic4Navigations)
                .HasForeignKey(d => d.IdEnemic4)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_nivell_enemic4");

            entity.HasMany(d => d.IdItems).WithMany(p => p.IdNivells)
                .UsingEntity<Dictionary<string, object>>(
                    "NivellItem",
                    r => r.HasOne<Item>().WithMany()
                        .HasForeignKey("IdItem")
                        .HasConstraintName("fk_ni_item"),
                    l => l.HasOne<Nivell>().WithMany()
                        .HasForeignKey("IdNivell")
                        .HasConstraintName("fk_ni_nivell"),
                    j =>
                    {
                        j.HasKey("IdNivell", "IdItem")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("nivell_item");
                        j.HasIndex(new[] { "IdItem" }, "fk_ni_item");
                        j.IndexerProperty<decimal>("IdNivell")
                            .HasPrecision(3)
                            .HasColumnName("id_nivell");
                        j.IndexerProperty<decimal>("IdItem")
                            .HasPrecision(3)
                            .HasColumnName("id_item");
                    });
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
                .HasComment("Tu, Enemic, Equip aliat, Equip enemic")
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
