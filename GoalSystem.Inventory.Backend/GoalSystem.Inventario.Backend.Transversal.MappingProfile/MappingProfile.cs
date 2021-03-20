using AutoMapper;
using GoalSystem.Inventario.Backend.Application.ViewModels.Cliente;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;

namespace GoalSystem.Inventario.Backend.Transversal.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InventarioItem, InventarioItemViewModel>()
                .ForMember(d => d.isExpired, opt => opt.Ignore())                
                .ReverseMap();
        }
    }
}
