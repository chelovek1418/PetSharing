using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PetSharing.Data.Entities;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetSharing.Data.Contexts
{
    public class PetSharingDbContext : IdentityDbContext<User>
    {
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PetProfile> PetProfiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public PetSharingDbContext(DbContextOptions<PetSharingDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>().HasKey(sc => new { sc.UserId, sc.PetId });
            modelBuilder.Entity<Subscription>()
                .HasOne(sc => sc.User)
                .WithMany(s => s.Subscriptions)
                .HasForeignKey(sc => sc.UserId);
            modelBuilder.Entity<Subscription>()
                .HasOne(sc => sc.PetProfile)
                .WithMany(s => s.Folowers)
                .HasForeignKey(sc => sc.PetId);
            modelBuilder.Entity<User>().HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
            modelBuilder.Entity<PetProfile>().HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
            modelBuilder.Entity<Post>().HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
            modelBuilder.Entity<Comment>().HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
            modelBuilder.Entity<Message>().HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Deleted && entry.Entity.GetType()!=typeof(IdentityUserRole<string>))
                {
                    entry.State = EntityState.Modified;
                    entry.CurrentValues["IsDeleted"] = true;
                    //foreach (var navigationEntry in entry.Navigations.Where(n => !n.Metadata.IsDependentToPrincipal()).Where(x=>x.CurrentValue!=null))
                    //{
                    //    if (navigationEntry is CollectionEntry collectionEntry)
                    //    {
                    //        foreach (var dependentEntry in collectionEntry.CurrentValue)
                    //        {
                    //            HandleDependent(Entry(dependentEntry));
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var dependentEntry = navigationEntry.CurrentValue;
                    //        if (dependentEntry != null)
                    //        {
                    //            HandleDependent(Entry(dependentEntry));
                    //        }
                    //    }
                    //}
                }
            }
        }
        //private void HandleDependent(EntityEntry entry)
        //{
        //    entry.CurrentValues["IsDeleted"] = true;
        //}
    }
}
