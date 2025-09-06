using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BdClasses;

public partial class ConstructionContext : DbContext
{
    public ConstructionContext()
    {
    }

    public ConstructionContext(DbContextOptions<ConstructionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GroupingPropertiesForItem> GroupingPropertiesForItems { get; set; }

    public virtual DbSet<GroupingProperty> GroupingProperties { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemMetaDatum> ItemMetaData { get; set; }

    public virtual DbSet<Journal> Journals { get; set; }

    public virtual DbSet<NameItem> NameItems { get; set; }

    public virtual DbSet<Object> Objects { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionsForRole> PermissionsForRoles { get; set; }

    public virtual DbSet<Producer> Producers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesOfUser> RolesOfUsers { get; set; }

    public virtual DbSet<TypeOfItem> TypeOfItems { get; set; }

    public virtual DbSet<TypesOfMetaDatum> TypesOfMetaData { get; set; }

    public virtual DbSet<TypesOfUnit> TypesOfUnits { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Construction;Username=postgres;Password=a");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupingPropertiesForItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GroupingPropertiesForItems_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Item).WithMany(p => p.GroupingPropertiesForItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("itemIdKKeyGProps");

            entity.HasOne(d => d.Prop).WithMany(p => p.GroupingPropertiesForItems)
                .HasForeignKey(d => d.PropId)
                .HasConstraintName("propFKeyGProps");
        });

        modelBuilder.Entity<GroupingProperty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GroupingProperties_pkey");

            entity.HasIndex(e => e.Name, "unGrCon").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Items_pkey");

            entity.HasIndex(e => e.NameId, "fki_nameFKeyItems");

            entity.HasIndex(e => e.TypeOfItemId, "fki_typItFKey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.CountOfUnits).HasDefaultValue(1);
            entity.Property(e => e.ExcpectedCost).HasDefaultValue(1);
            entity.Property(e => e.PricePerUnit).HasDefaultValue(1);

            entity.HasOne(d => d.Name).WithMany(p => p.Items)
                .HasForeignKey(d => d.NameId)
                .HasConstraintName("nameFKeyItems");

            entity.HasOne(d => d.Object).WithMany(p => p.Items)
                .HasForeignKey(d => d.Objectid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("objFKey");

            entity.HasOne(d => d.Producer).WithMany(p => p.Items)
                .HasForeignKey(d => d.ProducerId)
                .HasConstraintName("prFKey");

            entity.HasOne(d => d.TypeOfItem).WithMany(p => p.Items)
                .HasForeignKey(d => d.TypeOfItemId)
                .HasConstraintName("typItFKey");

            entity.HasOne(d => d.TypeUnit).WithMany(p => p.Items)
                .HasForeignKey(d => d.TypeUnitId)
                .HasConstraintName("tuFKey");
        });

        modelBuilder.Entity<ItemMetaDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ItemMetaData_pkey");

            entity.HasIndex(e => e.ItemId, "fki_mtIIDFKey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.DataType).WithMany(p => p.ItemMetaData)
                .HasForeignKey(d => d.DataTypeId)
                .HasConstraintName("itMtFKey");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemMetaData)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("mtIIDFKey");
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Journal_pkey");

            entity.ToTable("Journal");
        });

        modelBuilder.Entity<NameItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("NameItem_pkey");

            entity.ToTable("NameItem");

            entity.HasIndex(e => e.Name, "unName").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<Object>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Objects_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Address).HasColumnType("character varying(255)[]");
            entity.Property(e => e.Description).HasColumnType("character varying(255)[]");
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Permissions_pkey");

            entity.HasIndex(e => e.Name, "perUn").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<PermissionsForRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PermissionsForRoles_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Perm).WithMany(p => p.PermissionsForRoles)
                .HasForeignKey(d => d.PermId)
                .HasConstraintName("rlPerId");

            entity.HasOne(d => d.Role).WithMany(p => p.PermissionsForRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("perRId");
        });

        modelBuilder.Entity<Producer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Producers_pkey");

            entity.HasIndex(e => e.Name, "unNPeod").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Roles_pkey");

            entity.HasIndex(e => e.Name, "roleUn").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<RolesOfUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RolesOfUsers_pkey");

            entity.HasIndex(e => e.UserId, "fki_usFKey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Role).WithMany(p => p.RolesOfUsers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("rlFKey");

            entity.HasOne(d => d.User).WithMany(p => p.RolesOfUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("usFKey");
        });

        modelBuilder.Entity<TypeOfItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TypeOfItem_pkey");

            entity.ToTable("TypeOfItem");

            entity.HasIndex(e => e.Name, "toi").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<TypesOfMetaDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TypesOfMetaData_pkey");

            entity.HasIndex(e => e.Name, "unMtName").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<TypesOfUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TypesOfUnit_pkey");

            entity.ToTable("TypesOfUnit");

            entity.HasIndex(e => e.Name, "tyUn").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.HasIndex(e => e.Name, "usName").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying(255)[]");
            entity.Property(e => e.Password).HasColumnType("character varying(255)[]");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
