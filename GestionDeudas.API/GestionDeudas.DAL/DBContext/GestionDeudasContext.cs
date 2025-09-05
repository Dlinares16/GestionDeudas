using System;
using System.Collections.Generic;
using GestionDeudas.Model;
using Microsoft.EntityFrameworkCore;

namespace GestionDeudas.DAL.DBContext;

public partial class GestionDeudasContext : DbContext
{
    public GestionDeudasContext()
    {
    }

    public GestionDeudasContext(DbContextOptions<GestionDeudasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcceptedFriend> AcceptedFriends { get; set; }

    public virtual DbSet<Debt> Debts { get; set; }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBalance> UserBalances { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AcceptedFriend>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("accepted_friends");

            entity.Property(e => e.FriendEmail)
                .HasMaxLength(100)
                .HasColumnName("friend_email");
            entity.Property(e => e.FriendFirstName)
                .HasMaxLength(50)
                .HasColumnName("friend_first_name");
            entity.Property(e => e.FriendId).HasColumnName("friend_id");
            entity.Property(e => e.FriendLastName)
                .HasMaxLength(50)
                .HasColumnName("friend_last_name");
            entity.Property(e => e.FriendsSince)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("friends_since");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .HasColumnName("user_email");
            entity.Property(e => e.UserFirstName)
                .HasMaxLength(50)
                .HasColumnName("user_first_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserLastName)
                .HasMaxLength(50)
                .HasColumnName("user_last_name");
        });

        modelBuilder.Entity<Debt>(entity =>
        {
            entity.HasKey(e => e.DebtId).HasName("debts_pkey");

            entity.ToTable("debts");

            entity.HasIndex(e => e.CreditorId, "idx_debts_creditor");

            entity.HasIndex(e => e.DebtorId, "idx_debts_debtor");

            entity.HasIndex(e => e.Status, "idx_debts_status");

            entity.Property(e => e.DebtId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("debt_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreditorId).HasColumnName("creditor_id");
            entity.Property(e => e.DebtorId).HasColumnName("debtor_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Creditor).WithMany(p => p.DebtCreditors)
                .HasForeignKey(d => d.CreditorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("debts_creditor_id_fkey");

            entity.HasOne(d => d.Debtor).WithMany(p => p.DebtDebtors)
                .HasForeignKey(d => d.DebtorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("debts_debtor_id_fkey");
        });

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.VerificationId).HasName("email_verifications_pkey");

            entity.ToTable("email_verifications");

            entity.HasIndex(e => e.VerificationToken, "idx_verifications_token");

            entity.Property(e => e.VerificationId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("verification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expires_at");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("is_used");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerificationToken)
                .HasMaxLength(255)
                .HasColumnName("verification_token");

            entity.HasOne(d => d.User).WithMany(p => p.EmailVerifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("email_verifications_user_id_fkey");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.FriendshipId).HasName("friendships_pkey");

            entity.ToTable("friendships");

            entity.HasIndex(e => e.AddresseeId, "idx_friendships_addressee");

            entity.HasIndex(e => e.RequesterId, "idx_friendships_requester");

            entity.HasIndex(e => e.Status, "idx_friendships_status");

            entity.HasIndex(e => new { e.RequesterId, e.AddresseeId }, "unique_friendship").IsUnique();

            entity.Property(e => e.FriendshipId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("friendship_id");
            entity.Property(e => e.AddresseeId).HasColumnName("addressee_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.RequesterId).HasColumnName("requester_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Addressee).WithMany(p => p.FriendshipAddressees)
                .HasForeignKey(d => d.AddresseeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("friendships_addressee_id_fkey");

            entity.HasOne(d => d.Requester).WithMany(p => p.FriendshipRequesters)
                .HasForeignKey(d => d.RequesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("friendships_requester_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.HasIndex(e => e.DebtId, "idx_payments_debt");

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DebtId).HasColumnName("debt_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");

            entity.HasOne(d => d.Debt).WithMany(p => p.Payments)
                .HasForeignKey(d => d.DebtId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_debt_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.IsActive, "idx_users_active");

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerified)
                .HasDefaultValue(false)
                .HasColumnName("email_verified");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserBalance>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("user_balance");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.NetBalance).HasColumnName("net_balance");
            entity.Property(e => e.TotalIOwe).HasColumnName("total_i_owe");
            entity.Property(e => e.TotalOwedToMe).HasColumnName("total_owed_to_me");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("user_sessions_pkey");

            entity.ToTable("user_sessions");

            entity.HasIndex(e => e.IsActive, "idx_sessions_active");

            entity.HasIndex(e => e.UserId, "idx_sessions_user");

            entity.Property(e => e.SessionId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("session_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expires_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(500)
                .HasColumnName("refresh_token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_sessions_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
