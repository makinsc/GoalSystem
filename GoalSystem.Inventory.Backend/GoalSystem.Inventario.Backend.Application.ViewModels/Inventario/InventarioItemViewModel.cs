using System;

namespace GoalSystem.Inventario.Backend.Application.ViewModels.Cliente
{
    public class InventarioItemViewModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string FechaCaducidad { get; set; }
        public int Unidades { get; set; }
        public bool isExpired => DateTime.UtcNow > DateTime.Parse(FechaCaducidad);

    }
}
