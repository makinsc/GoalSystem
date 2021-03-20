using GoalSystem.Inventario.Backend.Domain.Core.Interfaces;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database
{
    [ExcludeFromCodeCoverage]
    public class InventarioContext : DbContext,IUnitOfWork
    {
        public InventarioContext(DbContextOptions<InventarioContext> options) : base(options)
        {  }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventarioItem>()
                .HasIndex(c => new { c.Nombre })
                .IsUnique(false)
                .HasName("Ind_InventarioItem_Nombre_1793F16CC77D03694AF5E1610A43F634");           
        }        
        public DbSet<InventarioItem> InventarioItems { get; set; }
        public override Task<int> SaveChangesAsync( CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow;

            this.ChangeTracker.Entries().Where(e => e.Entity is AuditoriaModelBase).ToList().ForEach((entry) =>
            {
                (entry.Entity as AuditoriaModelBase).FrechaModificacion = currentDate;
                //Como no estoy gestionando usuarios por flata de tiempo introduzco el nombre por defecto.
                (entry.Entity as AuditoriaModelBase).UsusarioModificacion = "SystemModif."; 
            });

            this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Entity is AuditoriaModelBase).ToList().ForEach((entry) =>
            {
                (entry.Entity as AuditoriaModelBase).FrechaCreacion = currentDate;
                (entry.Entity as AuditoriaModelBase).UsusarioCreacion = "System";
            });

            //this.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList().ForEach((entry) =>
            //{                
            //    entry.State = EntityState.Modified;
            //    entityLogicRemove.RemovedDate = currentDate;
            //    entityLogicRemove.RemovedByUser = userId;
            //    entityLogicRemove.IsRemoved = true;
            //});
            
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
