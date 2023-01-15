using System;
using System.Collections.Generic;
using EntityFrameworkCore.UseRowNumberForPaging;
using Microsoft.EntityFrameworkCore;

namespace ChatTracking.Models;

public partial class JabberContext : DbContext
{
    public JabberContext()
    {
    }

    public JabberContext(DbContextOptions<JabberContext> options)
        : base(options)
    {
    }

    public virtual DbSet<OfMessageArchive> OfMessageArchives { get; set; }

    public virtual DbSet<OfVcard> OfVcards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=RICHARD-PC\\SQLEXPRESS;Initial Catalog=jabber;Integrated Security=True;TrustServerCertificate=True");
    //=> optionsBuilder.UseSqlServer("Data Source=DBSERVER;Initial Catalog=jabber;Integrated Security=True;TrustServerCertificate=True", builder => builder.UseRowNumberForPaging());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OfMessageArchive>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ofMessageArchive");

            entity.HasIndex(e => e.ConversationId, "ofMessageArchive_con_idx");

            entity.Property(e => e.Body)
                .HasColumnType("ntext")
                .HasColumnName("body");
            entity.Property(e => e.ConversationId).HasColumnName("conversationID");
            entity.Property(e => e.FromJid)
                .HasMaxLength(1024)
                .HasColumnName("fromJID");
            entity.Property(e => e.SentDate).HasColumnName("sentDate");
            entity.Property(e => e.ToJid)
                .HasMaxLength(1024)
                .HasColumnName("toJID");
        });

        modelBuilder.Entity<OfVcard>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("ofVCard_pk");

            entity.ToTable("ofVCard");

            entity.Property(e => e.Username)
                .HasMaxLength(64)
                .HasColumnName("username");
            entity.Property(e => e.Vcard)
                //.HasColumnType("ntext")
                .HasColumnName("vcard");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
