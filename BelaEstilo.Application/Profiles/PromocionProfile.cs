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
    public class PromocionProfile: Profile
    {
        public PromocionProfile()
        {
            CreateMap<PromocionDTO, Promocion>().ReverseMap();
        }
    }
}
