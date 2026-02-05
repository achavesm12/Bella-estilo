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
    public class UsuarioProfile : Profile
    {
       public UsuarioProfile ()
        {
            CreateMap<UsuarioDTO, Usuario>().ReverseMap();

            CreateMap<UsuarioRegistroDTO, Usuario>().ReverseMap();
        }
    }
}
