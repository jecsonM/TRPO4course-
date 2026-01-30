using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MachineServices.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Machine> Machines { get; set; }

    public virtual DbSet<MachineService> MachineServices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderState> OrderStates { get; set; }

    public virtual DbSet<RelevantCost> RelevantCosts { get; set; }

    public virtual DbSet<RelevantOrderState> RelevantOrderStates { get; set; }

    public virtual DbSet<RelevantRequestState> RelevantRequestStates { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestState> RequestStates { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServiceProvision> ServiceProvisions { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ADCLG1;Database= Moiseev_MachineService;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__BF21A424E4D0F590");

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CompanyName)
                .IsUnicode(false)
                .HasColumnName("company_name");
            entity.Property(e => e.ContactPersonFullname)
                .IsUnicode(false)
                .HasColumnName("contact_person_fullname");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("contact_phone");
            entity.Property(e => e.Email)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("INN");
            entity.Property(e => e.Kpp)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("KPP");
            entity.Property(e => e.MainAddress)
                .IsUnicode(false)
                .HasColumnName("main_address");
            entity.Property(e => e.Notes)
                .IsUnicode(false)
                .HasColumnName("notes");
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.MachineId).HasName("PK__Machines__7B75BEA9092B9C8B");

            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.MastersComment)
                .IsUnicode(false)
                .HasColumnName("masters_comment");
            entity.Property(e => e.Model)
                .IsUnicode(false)
                .HasColumnName("model");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("serial_number");

            entity.HasOne(d => d.Client).WithMany(p => p.Machines)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Machines_Clients");
        });

        modelBuilder.Entity<MachineService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__MachineS__3E0DB8AFC5D718C4");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.CreatorsId).HasColumnName("creators_id");
            entity.Property(e => e.MachineServiceName)
                .IsUnicode(false)
                .HasColumnName("machine_service_name");

            entity.HasOne(d => d.Creators).WithMany(p => p.MachineServices)
                .HasForeignKey(d => d.CreatorsId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_MachineServices_Staff");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__4659622942D6BB39");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.RequestId).HasColumnName("request_id");

            entity.HasOne(d => d.Request).WithMany(p => p.Orders)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_Orders_Requests");

            entity.HasMany(d => d.Machines).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "MachinesInOrder",
                    r => r.HasOne<Machine>().WithMany()
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MachinesInOrder_Machines"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MachinesInOrder_Orders"),
                    j =>
                    {
                        j.HasKey("OrderId", "MachineId").HasName("PK_Orders_Machines");
                        j.ToTable("MachinesInOrder");
                        j.IndexerProperty<int>("OrderId").HasColumnName("order_id");
                        j.IndexerProperty<int>("MachineId").HasColumnName("machine_id");
                    });
        });

        modelBuilder.Entity<OrderState>(entity =>
        {
            entity.HasKey(e => e.OrderStateId).HasName("PK__OrderSta__C67F8250B1158A50");

            entity.Property(e => e.OrderStateId).HasColumnName("order_state_id");
            entity.Property(e => e.OrderStateName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("order_state_name");
        });

        modelBuilder.Entity<RelevantCost>(entity =>
        {
            entity.HasKey(e => e.RelevantCostId).HasName("PK__Relevant__FE9821FCA998D107");

            entity.Property(e => e.RelevantCostId).HasColumnName("relevantCostId");
            entity.Property(e => e.CreatorsId).HasColumnName("creators_id");
            entity.Property(e => e.RelevantCost1)
                .HasColumnType("money")
                .HasColumnName("relevant_cost");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.SetDate).HasColumnName("set_date");

            entity.HasOne(d => d.Creators).WithMany(p => p.RelevantCosts)
                .HasForeignKey(d => d.CreatorsId)
                .HasConstraintName("FK_RelevantCosts_Staff");

            entity.HasOne(d => d.Service).WithMany(p => p.RelevantCosts)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RelevantCosts_MachineServices");
        });

        modelBuilder.Entity<RelevantOrderState>(entity =>
        {
            entity.HasKey(e => e.RelevantOrderStateId).HasName("PK__Relevant__0C59A551F51CA213");

            entity.Property(e => e.RelevantOrderStateId).HasColumnName("relevant_order_state_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderStateId).HasColumnName("order_state_id");
            entity.Property(e => e.SetDate).HasColumnName("set_date");

            entity.HasOne(d => d.Order).WithMany(p => p.RelevantOrderStates)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_RelevantOrderStates_Orders");

            entity.HasOne(d => d.OrderState).WithMany(p => p.RelevantOrderStates)
                .HasForeignKey(d => d.OrderStateId)
                .HasConstraintName("FK_RelevantOrderStates_OrderStates");
        });

        modelBuilder.Entity<RelevantRequestState>(entity =>
        {
            entity.HasKey(e => e.RelevantRequestStateId).HasName("PK__Relevant__51D9374ABBD57859");

            entity.Property(e => e.RelevantRequestStateId).HasColumnName("relevant_request_state_id");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.RequestStateId).HasColumnName("request_state_id");
            entity.Property(e => e.SetDate).HasColumnName("set_date");

            entity.HasOne(d => d.Request).WithMany(p => p.RelevantRequestStates)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_RelevantRequestStates_Requests");

            entity.HasOne(d => d.RequestState).WithMany(p => p.RelevantRequestStates)
                .HasForeignKey(d => d.RequestStateId)
                .HasConstraintName("FK_RelevantRequestStates_RequestStates");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Requests__18D3B90F559FD4CF");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.ServiceAddress)
                .IsUnicode(false)
                .HasColumnName("service_address");

            entity.HasOne(d => d.Client).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Requests_Clients");
        });

        modelBuilder.Entity<RequestState>(entity =>
        {
            entity.HasKey(e => e.RequestStateId).HasName("PK__RequestS__9C3FC190CD56D25B");

            entity.Property(e => e.RequestStateId).HasColumnName("request_state_id");
            entity.Property(e => e.RequestStateName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("request_state_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC39459020");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<ServiceProvision>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ServiceId }).HasName("PK_Orders_MachineServices");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.MastersId).HasColumnName("masters_id");

            entity.HasOne(d => d.Masters).WithMany(p => p.ServiceProvisions)
                .HasForeignKey(d => d.MastersId)
                .HasConstraintName("FK_ServiceProvisions_Staff");

            entity.HasOne(d => d.Order).WithMany(p => p.ServiceProvisions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_ServiceProvisions_Orders");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceProvisions)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_ServiceProvisions_MachineServices");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__1963DD9C77AC1E92");

            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Login)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Staff)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Staff_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
