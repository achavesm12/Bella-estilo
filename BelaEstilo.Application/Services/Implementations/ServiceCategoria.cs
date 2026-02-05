using AutoMapper;
using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Implementations
{
    public class ServiceCategoria : IServiceCategoria
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryCategoria _repository;

        public ServiceCategoria(IMapper mapper, IRepositoryCategoria repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<ICollection<CategoriaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            var collection = _mapper.Map<ICollection<CategoriaDTO>>(list);
            return collection;
        }
    }
}
