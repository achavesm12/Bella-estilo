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
    public class PedidoPersonalizadoProfile : Profile
    {
        public PedidoPersonalizadoProfile()
        {
            CreateMap<PedidoPersonalizadoDTO, PedidoPersonalizado>().ReverseMap();
            CreateMap<PedidoPersonalizadoCriterioDTO, PedidoPersonalizadoCriterio>().ReverseMap();
        }
    }
}
