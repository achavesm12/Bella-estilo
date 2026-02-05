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
    public class ServiceProducto : IServiceProducto
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryProducto _repository;
        private readonly ILogger<ServiceProducto> _logger;

        public ServiceProducto(IMapper mapper, IRepositoryProducto repositoy, ILogger<ServiceProducto> logger)
        {
            _mapper = mapper;
            _repository = repositoy;
            _logger = logger;
        }

        public async Task<ICollection<ProductoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            var collection = _mapper.Map<ICollection<ProductoDTO>>(list);
            return collection;
        }

        public async Task<ProductoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapper = _mapper.Map<ProductoDTO>(@object);
            return objectMapper;
        }

        public async Task<int> AddAsync(ProductoRegistroDTO dto)
        {
            var producto = _mapper.Map<Producto>(dto);
            return await _repository.AddAsync(producto, dto.IdsEtiquetas, dto.Imagenes);
        }

        public async Task UpdateAsync(ProductoRegistroDTO dto)
        {
            var producto = _mapper.Map<Producto>(dto);
            await _repository.UpdateAsync(producto, dto.IdsEtiquetas, dto.Imagenes, dto.ImagenesAEliminar);

        }


        public async Task<List<ImagenProductoDTO>> GetImagenesProductoAsync(int idProducto)
        {
            var imagenes = await _repository.GetImagenesByProductoIdAsync(idProducto);
            return _mapper.Map<List<ImagenProductoDTO>>(imagenes);
        }

    }
}
