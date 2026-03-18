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

    public virtual DbSet<Efecte> Efectes { get; set; }

    public virtual DbSet<Habilitat> Habilitats { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Nivell> Nivells { get; set; }

    public virtual DbSet<Objectiu> Objectius { get; set; }

    public virtual DbSet<Personatge> Personatges { get; set; }

    public virtual DbSet<PersonatgeItem> PersonatgeItems { get; set; }

    public virtual DbSet<TaulaEstadistique> TaulaEstadistiques { get; set; }

    public virtual DbSet<TaulaEstat> TaulaEstats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var properties = new Dictionary<string, string>();

        var lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.properties"));

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

            entity.HasIndex(e => e.IdObjectiu, "id_objectiu");

            entity.Property(e => e.Id)
                .HasPrecision(3)
                .HasColumnName("id");
            entity.Property(e => e.Descripcio)
                .HasMaxLength(255)
                .HasColumnName("descripcio");
            entity.Property(e => e.Icona)
                .HasMaxLength(255)
                .HasColumnName("icona");
            entity.Property(e => e.IdObjectiu)
                .HasPrecision(1)
                .HasColumnName("id_objectiu");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");

            entity.HasOne(d => d.IdObjectiuNavigation).WithMany(p => p.Accios)
                .HasForeignKey(d => d.IdObjectiu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accio_ibfk_1");
        });

        modelBuilder.Entity<Efecte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("efecte");

            entity.HasIndex(e => e.IdAccio, "id_accio");

            entity.HasIndex(e => e.IdEstadistica, "id_estadistica");

            entity.HasIndex(e => e.IdEstat, "id_estat");

            entity.Property(e => e.Id)
                .HasPrecision(3)
                .HasColumnName("id");
            entity.Property(e => e.Duracio)
                .HasPrecision(2)
                .HasDefaultValueSql("'0'")
                .HasColumnName("duracio");
            entity.Property(e => e.EsAfegir).HasColumnName("esAfegir");
            entity.Property(e => e.IdAccio)
                .HasPrecision(3)
                .HasColumnName("id_accio");
            entity.Property(e => e.IdEstadistica)
                .HasPrecision(2)
                .HasColumnName("id_estadistica");
            entity.Property(e => e.IdEstat)
                .HasPrecision(2)
                .HasColumnName("id_estat");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.Probabilitat)
                .HasPrecision(3)
                .HasDefaultValueSql("'100'")
                .HasColumnName("probabilitat");
            entity.Property(e => e.Quantitat)
                .HasPrecision(3)
                .HasColumnName("quantitat");

            entity.HasOne(d => d.IdAccioNavigation).WithMany(p => p.Efectes)
                .HasForeignKey(d => d.IdAccio)
                .HasConstraintName("efecte_ibfk_1");

            entity.HasOne(d => d.IdEstadisticaNavigation).WithMany(p => p.Efectes)
                .HasForeignKey(d => d.IdEstadistica)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("efecte_ibfk_3");

            entity.HasOne(d => d.IdEstatNavigation).WithMany(p => p.Efectes)
                .HasForeignKey(d => d.IdEstat)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("efecte_ibfk_2");
        });

        modelBuilder.Entity<Habilitat>(entity =>
        {
            entity.HasKey(e => e.IdAccio).HasName("PRIMARY");

            entity.ToTable("habilitat");

            entity.HasIndex(e => e.IdPersonatge, "id_personatge");

            entity.Property(e => e.IdAccio)
                .HasPrecision(3)
                .HasColumnName("id_accio");
            entity.Property(e => e.IdPersonatge)
                .HasPrecision(3)
                .HasColumnName("id_personatge");

            entity.HasOne(d => d.IdAccioNavigation).WithOne(p => p.Habilitat)
                .HasForeignKey<Habilitat>(d => d.IdAccio)
                .HasConstraintName("habilitat_ibfk_1");

            entity.HasOne(d => d.IdPersonatgeNavigation).WithMany(p => p.Habilitats)
                .HasForeignKey(d => d.IdPersonatge)
                .HasConstraintName("habilitat_ibfk_2");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.IdAccio).HasName("PRIMARY");

            entity.ToTable("item");

            entity.Property(e => e.IdAccio)
                .HasPrecision(3)
                .HasColumnName("id_accio");

            entity.HasOne(d => d.IdAccioNavigation).WithOne(p => p.Item)
                .HasForeignKey<Item>(d => d.IdAccio)
                .HasConstraintName("item_ibfk_1");
        });

        modelBuilder.Entity<Nivell>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("nivell");

            entity.HasIndex(e => e.IdEnemic1, "id_enemic_1");

            entity.HasIndex(e => e.IdEnemic2, "id_enemic_2");

            entity.HasIndex(e => e.IdEnemic3, "id_enemic_3");

            entity.HasIndex(e => e.IdEnemic4, "id_enemic_4");

            entity.HasIndex(e => e.Ordre, "ordre").IsUnique();

            entity.Property(e => e.Id)
                .HasPrecision(3)
                .HasColumnName("id");
            entity.Property(e => e.Fons)
                .HasMaxLength(255)
                .HasColumnName("fons");
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
                .HasColumnName("ordre");

            entity.HasOne(d => d.IdEnemic1Navigation).WithMany(p => p.NivellIdEnemic1Navigations)
                .HasForeignKey(d => d.IdEnemic1)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("nivell_ibfk_1");

            entity.HasOne(d => d.IdEnemic2Navigation).WithMany(p => p.NivellIdEnemic2Navigations)
                .HasForeignKey(d => d.IdEnemic2)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("nivell_ibfk_2");

            entity.HasOne(d => d.IdEnemic3Navigation).WithMany(p => p.NivellIdEnemic3Navigations)
                .HasForeignKey(d => d.IdEnemic3)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("nivell_ibfk_3");

            entity.HasOne(d => d.IdEnemic4Navigation).WithMany(p => p.NivellIdEnemic4Navigations)
                .HasForeignKey(d => d.IdEnemic4)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("nivell_ibfk_4");

            entity.HasMany(d => d.IdItems).WithMany(p => p.IdNivells)
                .UsingEntity<Dictionary<string, object>>(
                    "NivellItem",
                    r => r.HasOne<Item>().WithMany()
                        .HasForeignKey("IdItem")
                        .HasConstraintName("nivell_item_ibfk_2"),
                    l => l.HasOne<Nivell>().WithMany()
                        .HasForeignKey("IdNivell")
                        .HasConstraintName("nivell_item_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdNivell", "IdItem")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("nivell_item");
                        j.HasIndex(new[] { "IdItem" }, "id_item");
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

            entity.Property(e => e.Id)
                .HasPrecision(1)
                .HasColumnName("id");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Personatge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("personatge");

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
            entity.Property(e => e.Icona)
                .HasMaxLength(255)
                .HasColumnName("icona");
            entity.Property(e => e.Imatge)
                .HasMaxLength(255)
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

        modelBuilder.Entity<PersonatgeItem>(entity =>
        {
            entity.HasKey(e => new { e.IdPersonatge, e.IdItem })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("personatge_item");

            entity.HasIndex(e => e.IdItem, "id_item");

            entity.Property(e => e.IdPersonatge)
                .HasPrecision(3)
                .HasColumnName("id_personatge");
            entity.Property(e => e.IdItem)
                .HasPrecision(3)
                .HasColumnName("id_item");
            entity.Property(e => e.QuantitatStock)
                .HasPrecision(2)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantitat_stock");

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.PersonatgeItems)
                .HasForeignKey(d => d.IdItem)
                .HasConstraintName("personatge_item_ibfk_2");

            entity.HasOne(d => d.IdPersonatgeNavigation).WithMany(p => p.PersonatgeItems)
                .HasForeignKey(d => d.IdPersonatge)
                .HasConstraintName("personatge_item_ibfk_1");
        });

        modelBuilder.Entity<TaulaEstadistique>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("taula_estadistiques");

            entity.Property(e => e.Id)
                .HasPrecision(2)
                .HasColumnName("id");
            entity.Property(e => e.Icona)
                .HasMaxLength(255)
                .HasColumnName("icona");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<TaulaEstat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("taula_estats");

            entity.Property(e => e.Id)
                .HasPrecision(2)
                .HasColumnName("id");
            entity.Property(e => e.Descripcio)
                .HasMaxLength(255)
                .HasColumnName("descripcio");
            entity.Property(e => e.Icona)
                .HasMaxLength(255)
                .HasColumnName("icona");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
