using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem
{
    public class InventarioItem: AuditoriaModelBase
    {   
        [Key]
        public Guid Id { get; set; }
        public string Nombre { get; set; }        
        public DateTime FechaCaducidad { get; set; }        
        public int Unidades { get; set; }
        public bool IsNotificacionExpiradaEnviada { get; set; } = false;
    }
}
