using AutoMapper;
using VertrauApi.Models;

namespace VertrauApi.Mappings;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<CreateUsuarioDto, Usuario>();
        CreateMap<UpdateUsuarioDto, Usuario>();
        CreateMap<Usuario, UsuarioResponseDto>();
    }
}
