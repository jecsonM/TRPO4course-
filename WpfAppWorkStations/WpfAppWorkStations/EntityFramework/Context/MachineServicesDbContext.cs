using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WpfAppWorkStations.EntityFramework;

namespace WpfAppWorkStations.EntityFramework.Context;

public partial class MachineServicesDbContext : DbContext
{
    public MachineServicesDbContext()
    {
    }

    public MachineServicesDbContext(DbContextOptions<MachineServicesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Machine> Machines { get; set; }

    public virtual DbSet<Machineservice> Machineservices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderstate> Orderstates { get; set; }

    public virtual DbSet<Relevantcost> Relevantcosts { get; set; }

    public virtual DbSet<Relevantorderstate> Relevantorderstates { get; set; }

    public virtual DbSet<Relevantrequeststate> Relevantrequeststates { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Requeststate> Requeststates { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Serviceprovision> Serviceprovisions { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MachineServicesDB;Username=MachineServiceUser;Password=MachineServiceUserMaster;SSL Mode=Prefer;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("clients_pkey");

            entity.ToTable("clients", "MachineServiceDBScheme");

            entity.Property(e => e.ClientId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("client_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(256)
                .HasColumnName("company_name");
            entity.Property(e => e.ContactPersonFullname)
                .HasMaxLength(256)
                .HasColumnName("contact_person_fullname");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(15)
                .HasColumnName("contact_phone");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("inn");
            entity.Property(e => e.Kpp)
                .HasMaxLength(9)
                .HasColumnName("kpp");
            entity.Property(e => e.MainAddress)
                .HasMaxLength(256)
                .HasColumnName("main_address");
            entity.Property(e => e.Notes)
                .HasMaxLength(256)
                .HasColumnName("notes");
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.MachineId).HasName("machines_pkey");

            entity.ToTable("machines", "MachineServiceDBScheme");

            entity.Property(e => e.MachineId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("machine_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.MastersComment)
                .HasMaxLength(256)
                .HasColumnName("masters_comment");
            entity.Property(e => e.Model)
                .HasMaxLength(256)
                .HasColumnName("model");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(50)
                .HasColumnName("serial_number");

            entity.HasOne(d => d.Client).WithMany(p => p.Machines)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("fk_machines_clients");
        });

        modelBuilder.Entity<Machineservice>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("machineservices_pkey");

            entity.ToTable("machineservices", "MachineServiceDBScheme");

            entity.Property(e => e.ServiceId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("service_id");
            entity.Property(e => e.CreatorsId).HasColumnName("creators_id");
            entity.Property(e => e.MachineServiceName)
                .HasMaxLength(256)
                .HasColumnName("machine_service_name");

            entity.HasOne(d => d.Creators).WithMany(p => p.Machineservices)
                .HasForeignKey(d => d.CreatorsId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_machineservices_staff");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders", "MachineServiceDBScheme");

            entity.Property(e => e.OrderId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("order_id");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.RequestId).HasColumnName("request_id");

            entity.HasOne(d => d.Request).WithMany(p => p.Orders)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("fk_orders_requests");

            entity.HasMany(d => d.Machines).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "Machinesinorder",
                    r => r.HasOne<Machine>().WithMany()
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_machinesinorder_machines"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_machinesinorder_orders"),
                    j =>
                    {
                        j.HasKey("OrderId", "MachineId").HasName("pk_orders_machines");
                        j.ToTable("machinesinorder", "MachineServiceDBScheme");
                        j.IndexerProperty<int>("OrderId").HasColumnName("order_id");
                        j.IndexerProperty<int>("MachineId").HasColumnName("machine_id");
                    });
        });

        modelBuilder.Entity<Orderstate>(entity =>
        {
            entity.HasKey(e => e.OrderStateId).HasName("orderstates_pkey");

            entity.ToTable("orderstates", "MachineServiceDBScheme");

            entity.Property(e => e.OrderStateId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("order_state_id");
            entity.Property(e => e.OrderStateName)
                .HasMaxLength(30)
                .HasColumnName("order_state_name");
        });

        modelBuilder.Entity<Relevantcost>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("relevantcosts", "MachineServiceDBScheme");

            entity.Property(e => e.CreatorsId).HasColumnName("creators_id");
            entity.Property(e => e.RelevantCost1)
                .HasColumnType("money")
                .HasColumnName("relevant_cost");
            entity.Property(e => e.Relevantcostid)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("relevantcostid");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.SetDate).HasColumnName("set_date");

            entity.HasOne(d => d.Creators).WithMany()
                .HasForeignKey(d => d.CreatorsId)
                .HasConstraintName("fk_relevantcosts_staff");

            entity.HasOne(d => d.Service).WithMany()
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_relevantcosts_machineservices");
        });

        modelBuilder.Entity<Relevantorderstate>(entity =>
        {
            entity.HasKey(e => e.RelevantOrderStateId).HasName("relevantorderstates_pkey");

            entity.ToTable("relevantorderstates", "MachineServiceDBScheme");

            entity.Property(e => e.RelevantOrderStateId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("relevant_order_state_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderStateId).HasColumnName("order_state_id");
            entity.Property(e => e.SetDate).HasColumnName("set_date");

            entity.HasOne(d => d.Order).WithMany(p => p.Relevantorderstates)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_relevantorderstates_orders");

            entity.HasOne(d => d.OrderState).WithMany(p => p.Relevantorderstates)
                .HasForeignKey(d => d.OrderStateId)
                .HasConstraintName("fk_relevantorderstates_orderstates");
        });

        modelBuilder.Entity<Relevantrequeststate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("relevantrequeststates", "MachineServiceDBScheme");

            entity.Property(e => e.RelevantRequestStateId)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("relevant_request_state_id");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.RequestStateId).HasColumnName("request_state_id");
            entity.Property(e => e.SetDate).HasColumnName("set_date");

            entity.HasOne(d => d.Request).WithMany()
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("fk_relevantrequeststates_requests");

            entity.HasOne(d => d.RequestState).WithMany()
                .HasForeignKey(d => d.RequestStateId)
                .HasConstraintName("fk_relevantrequeststates_requeststates");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("requests_pkey");

            entity.ToTable("requests", "MachineServiceDBScheme");

            entity.Property(e => e.RequestId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("request_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.ServiceAddress)
                .HasMaxLength(256)
                .HasColumnName("service_address");

            entity.HasOne(d => d.Client).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("fk_requests_clients");
        });

        modelBuilder.Entity<Requeststate>(entity =>
        {
            entity.HasKey(e => e.RequestStateId).HasName("requeststates_pkey");

            entity.ToTable("requeststates", "MachineServiceDBScheme");

            entity.Property(e => e.RequestStateId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("request_state_id");
            entity.Property(e => e.RequestStateName)
                .HasMaxLength(30)
                .HasColumnName("request_state_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles", "MachineServiceDBScheme");

            entity.Property(e => e.RoleId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Serviceprovision>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ServiceId }).HasName("pk_orders_machineservices");

            entity.ToTable("serviceprovisions", "MachineServiceDBScheme");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.MastersId).HasColumnName("masters_id");

            entity.HasOne(d => d.Masters).WithMany(p => p.Serviceprovisions)
                .HasForeignKey(d => d.MastersId)
                .HasConstraintName("fk_serviceprovisions_staff");

            entity.HasOne(d => d.Order).WithMany(p => p.Serviceprovisions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_serviceprovisions_orders");

            entity.HasOne(d => d.Service).WithMany(p => p.Serviceprovisions)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("fk_serviceprovisions_machineservices");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("staff_pkey");

            entity.ToTable("staff", "MachineServiceDBScheme");

            entity.Property(e => e.StaffId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("staff_id");
            entity.Property(e => e.Login)
                .HasMaxLength(256)
                .HasColumnName("login");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Staff)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_staff_roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
