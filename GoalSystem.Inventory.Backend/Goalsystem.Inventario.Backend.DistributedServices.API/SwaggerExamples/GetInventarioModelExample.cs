using GoalSystem.Inventario.Backend.Application.ViewModels.Cliente;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.IO;

namespace GoalSystem.Inventario.Backend.API.SwaggerExamples
{
    /// <summary>
    /// Representa un Modelo de ejemplo de elemento de inventario .
    /// </summary>
    public class GetInventarioModelExample : IExamplesProvider<IEnumerable<InventarioItemViewModel>>
    {
        private const string path = "./jsonData/GetInventarioItemResponse.json";

        /// <summary>
        /// Obtiene ejemplo de lista de elementos de inventario.
        /// </summary>
        /// <returns>Lista de ejemplo de elementos de inventario</returns>
        public IEnumerable<InventarioItemViewModel> GetExamples()
        {
            return JsonConvert.DeserializeObject<IEnumerable<InventarioItemViewModel>>(File.ReadAllText(path));
        }
    }
}
