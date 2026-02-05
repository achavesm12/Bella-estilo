using AutoMapper;
using BelaEstilo.Application.DTOs;
using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Profiles
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            CreateMap<ProductoDTO, Producto>()
              .ForMember(dest => dest.PedidoProducto, opt => opt.Ignore())
              .ForMember(dest => dest.ImagenProducto, opt => opt.Ignore())
              .ForMember(dest => dest.Resena, opt => opt.Ignore())
              .ForMember(dest => dest.IdEtiqueta, opt => opt.Ignore())
              .ForMember(dest => dest.IdPromocion, opt => opt.Ignore())
              .ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore())
              .ReverseMap();

            CreateMap<ProductoRegistroDTO, Producto>()
                .ForMember(dest => dest.PedidoProducto, opt => opt.Ignore())
                .ForMember(dest => dest.ImagenProducto, opt => opt.Ignore())
                .ForMember(dest => dest.IdEtiqueta, opt => opt.Ignore())
                .ForMember(dest => dest.IdPromocion, opt => opt.Ignore())
                .ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore())
                .ReverseMap();

            //Mapeo para imágenes
            CreateMap<ImagenProducto, ImagenProductoDTO>()
                .ForMember(dest => dest.Imagen, opt => opt.MapFrom(src => src.Imagen))
                .ReverseMap();
        }
    }
}
