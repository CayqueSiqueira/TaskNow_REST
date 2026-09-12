using AutoMapper;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.API.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Quadro, QuadroDTO>().ReverseMap()
            .ForMember(dest => dest.Listas, opt => opt.Ignore())
            .ForMember(dest => dest.Membros, opt => opt.Ignore())
            .ForMember(dest => dest.Etiquetas, opt => opt.Ignore())
            .ForMember(dest => dest.Dono, opt => opt.Ignore());

        CreateMap<Lista, ListaDTO>().ReverseMap()
            .ForMember(dest => dest.Quadro, opt => opt.Ignore())
            .ForMember(dest => dest.Cartoes, opt => opt.Ignore());

        CreateMap<Cartao, CartaoDTO>()
            .ForMember(dest => dest.Etiquetas, opt => opt.MapFrom(src =>
                src.Etiquetas
                    .Where(x => x.Etiqueta != null)
                    .Select(x => x.Etiqueta!)));
        CreateMap<CartaoDTO, Cartao>()
            .ForMember(dest => dest.Lista, opt => opt.Ignore())
            .ForMember(dest => dest.Responsavel, opt => opt.Ignore())
            .ForMember(dest => dest.Etiquetas, opt => opt.Ignore())
            .ForMember(dest => dest.Comentarios, opt => opt.Ignore());

        CreateMap<Etiqueta, EtiquetaDTO>().ReverseMap();

        CreateMap<Comentario, ComentarioDTO>()
            .ForMember(dest => dest.AutorNome, opt => opt.MapFrom(src =>
                src.Autor == null ? string.Empty : src.Autor.UserName ?? src.Autor.Email ?? string.Empty));
        CreateMap<ComentarioDTO, Comentario>()
            .ForMember(dest => dest.Cartao, opt => opt.Ignore())
            .ForMember(dest => dest.Autor, opt => opt.Ignore());

        CreateMap<AtividadeCartao, AtividadeCartaoDTO>()
            .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(src =>
                src.Usuario == null ? string.Empty : src.Usuario.UserName ?? src.Usuario.Email ?? string.Empty));
        CreateMap<AtividadeCartaoDTO, AtividadeCartao>()
            .ForMember(dest => dest.Cartao, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());

        CreateMap<MembroQuadro, MembroQuadroDTO>()
            .ForMember(dest => dest.Papel, opt => opt.MapFrom(src => src.Papel.ToString()));
        CreateMap<MembroQuadroDTO, MembroQuadro>()
            .ForMember(dest => dest.Papel, opt => opt.MapFrom(src => Enum.Parse<PapelMembro>(src.Papel)))
            .ForMember(dest => dest.Quadro, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());
    }
}
