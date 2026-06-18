using FileSyncBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FileSyncBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Blob> Blobs => Set<Blob>();
    public DbSet<Commit> Commits => Set<Commit>();
    public DbSet<CommitFile> CommitFiles => Set<CommitFile>();
    public DbSet<SyncSnapshot> SyncSnapshots => Set<SyncSnapshot>();
    public DbSet<Conflict> Conflicts => Set<Conflict>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Составной первичный ключ для CommitFile
        modelBuilder.Entity<CommitFile>()
            .HasKey(cf => new { cf.CommitId, cf.Path });
        
        // Индексы для производительности
        modelBuilder.Entity<CommitFile>()
            .HasIndex(cf => cf.BlobHash);
        
        modelBuilder.Entity<Commit>()
            .HasIndex(c => c.Timestamp);
        
        modelBuilder.Entity<Commit>()
            .HasIndex(c => c.Author);
        
        modelBuilder.Entity<SyncSnapshot>()
            .HasIndex(s => s.UserId)
            .IsUnique();
        
        modelBuilder.Entity<Conflict>()
            .HasIndex(c => new { c.Path, c.CreatedAt });
        
        // Связи
        modelBuilder.Entity<Commit>()
            .HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<CommitFile>()
            .HasOne(cf => cf.Commit)
            .WithMany(c => c.Files)
            .HasForeignKey(cf => cf.CommitId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CommitFile>()
            .HasOne(cf => cf.Blob)
            .WithMany()
            .HasForeignKey(cf => cf.BlobHash)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<SyncSnapshot>()
            .HasOne(s => s.ServerCommit)
            .WithMany()
            .HasForeignKey(s => s.ServerCommitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}