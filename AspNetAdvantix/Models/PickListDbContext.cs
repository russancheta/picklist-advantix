using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AspNetAdvantix.Models
{
    public partial class PickListDbContext : DbContext
    {
        public PickListDbContext()
        {
        }

        public PickListDbContext(DbContextOptions<PickListDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PickListH> PickListH { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=10.0.100.30;Initial Catalog=TESTDBPICKLIST;Persist Security Info=False;User ID=sa;Password=B1Admin;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<PickListH>(entity =>
            {
                entity.HasKey(e => e.DocEntry)
                    .HasName("KPICK_LIST_H_PR");

                entity.ToTable("@PICK_LIST_H");

                entity.Property(e => e.DocEntry).ValueGeneratedNever();

                entity.Property(e => e.Canceled)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Creator).HasMaxLength(8);

                entity.Property(e => e.DataSource)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Handwrtten)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.Instance).HasDefaultValueSql("((0))");

                entity.Property(e => e.Object).HasMaxLength(20);

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.RequestStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('W')");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('O')");

                entity.Property(e => e.Transfered)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.UObjType)
                    .HasColumnName("U_ObjType")
                    .HasMaxLength(15);

                entity.Property(e => e.USoDate)
                    .HasColumnName("U_SO_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.USoDocNum).HasColumnName("U_SO_DocNum");

                entity.Property(e => e.USoItemCode)
                    .HasColumnName("U_SO_ItemCode")
                    .HasMaxLength(30);

                entity.Property(e => e.USoPlno).HasColumnName("U_SO_PLNo");

                entity.Property(e => e.USoQty)
                    .HasColumnName("U_SO_Qty")
                    .HasColumnType("numeric(19, 6)");

                entity.Property(e => e.USoRemarks)
                    .HasColumnName("U_SO_Remarks")
                    .HasMaxLength(100);

                entity.Property(e => e.USoUserName)
                    .HasColumnName("U_SO_UserName")
                    .HasMaxLength(30);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });
        }
    }
}
