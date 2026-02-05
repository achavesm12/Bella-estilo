using AutoMapper;
using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Implementations
{
    public class ServiceResena : IServiceResena
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryResena _repository;
        private readonly ILogger<ServiceResena> _logger;

        public ServiceResena(IMapper mapper, IRepositoryResena repositoy, ILogger<ServiceResena> logger)
        {
            _mapper = mapper;
            _repository = repositoy;
            _logger = logger;
        }

        public async Task<ICollection<ResenaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            var collection = _mapper.Map<ICollection<ResenaDTO>>(list);
            return collection;
        }

        public async Task<ResenaDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapper = _mapper.Map<ResenaDTO>(@object);
            return objectMapper;
        }

        public async Task<int> AddAsync(ResenaRegistroDTO dto, int idUsuario)
        {
            var resena = new Resena
            {
                Comentario = dto.Comentario,
                Valoracion = dto.Valoracion,
                Fecha = DateTime.Now,
                IdProducto = dto.IdProducto,
                IdUsuario = idUsuario
            };
            return await _repository.AddAsync(resena);
        }

    }
}
