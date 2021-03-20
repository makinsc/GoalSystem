using System;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.Models
{
    public class AuditoriaModelBase
    {
        public DateTime FrechaCreacion { get; set; }
        public DateTime FrechaModificacion { get; set; }
        public string UsusarioCreacion { get; set; }
        public string UsusarioModificacion { get; set; }
    }
}
