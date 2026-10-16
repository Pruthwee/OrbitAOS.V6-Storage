// ============================================================
// Application/Common/Mappings/MappingProfile.cs
// Clean Architecture - Application Layer
// AutoMapper profile for ViewModel/DTO mappings
// Replaces: manual mapping code in controllers
// ============================================================

using AutoMapper;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Web.ViewModels;

namespace OrbitAOS.V6.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile defining all object-to-object mappings.
/// Centralizes mapping configuration in the Application layer.
/// Replaces manual property assignment in legacy MVC controllers.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ApplicationUser → UserViewModel
        CreateMap<ApplicationUser, UserViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ReverseMap();

        // Add additional mappings here as domain entities are added
        // Example: CreateMap<Product, ProductViewModel>().ReverseMap();
    }
}
