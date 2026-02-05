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
    public class ServicePromocion : IServicePromocion
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryPromocion _repository;
        private readonly ILogger<ServicePromocion> _logger;

        public ServicePromocion(IMapper mapper, IRepositoryPromocion repositoy, ILogger<ServicePromocion> logger)
        {
            _mapper = mapper;
            _repository = repositoy;
            _logger = logger;
        }

        public async Task<ICollection<PromocionDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            var collection = _mapper.Map<ICollection<PromocionDTO>>(list);
            return collection;
        }

        public async Task<PromocionDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapper = _mapper.Map<PromocionDTO>(@object);
            return objectMapper;
        }

        public async Task<int> AddAsync(PromocionRegistroDTO dto)
        {
            var promocion = new Promocion
            {
                Nombre = dto.Nombre,
                Descuento = dto.Descuento / 100m,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin
            };

            if (dto.TipoPromocion == "Categoria" && dto.IdCategoria != null)
            {
                promocion.IdCategoria = dto.IdCategoria
                    .Select(id => new Categoria { IdCategoria = id })
                    .ToList();
            }
            else if (dto.TipoPromocion == "Producto" && dto.IdProducto != null)
            {
                promocion.IdProducto = dto.IdProducto
                    .Select(id => new Producto { IdProducto = id })
                    .ToList();
            }

            return await _repository.AddAsync(promocion);
        }
        public async Task UpdateAsync(PromocionRegistroDTO dto)
        {
            var promocion = new Promocion
            {
                IdPromocion = dto.IdPromocion,
                Nombre = dto.Nombre,
                Descuento = dto.Descuento,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                IdCategoria = new List<Categoria>(),
                IdProducto = new List<Producto>()
            };

            if (dto.TipoPromocion == "Categoria" && dto.IdCategoria != null)
            {
                promocion.IdCategoria = dto.IdCategoria
                    .Select(id => new Categoria { IdCategoria = id })
                    .ToList();
            }
            else if (dto.TipoPromocion == "Producto" && dto.IdProducto != null)
            {
                promocion.IdProducto = dto.IdProducto
                    .Select(id => new Producto { IdProducto = id })
                    .ToList();
            }

            await _repository.UpdateAsync(promocion);
        }


        public async Task<List<Promocion>> ListEntityAsync()
        {
            return (await _repository.ListAsync()).ToList(); 
        }

    }
}
