using AutoMapper;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.API.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Quadro, QuadroDTO>().ReverseMap();
        CreateMap<Lista, ListaDTO>().ReverseMap();
        CreateMap<Cartao, CartaoDTO>().ReverseMap();
        CreateMap<Etiqueta, EtiquetaDTO>().ReverseMap();
        CreateMap<Comentario, ComentarioDTO>().ReverseMap();
        CreateMap<MembroQuadro, MembroQuadroDTO>().ReverseMap();
    }
}