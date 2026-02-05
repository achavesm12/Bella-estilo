using AutoMapper;
using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Implementations
{
    public class ServiceEtiqueta : IServiceEtiqueta
    {
        private readonly IRepositoryEtiqueta _repository;
        private readonly IMapper _mapper;

        public ServiceEtiqueta(IRepositoryEtiqueta repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<EtiquetaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            var collection = _mapper.Map<ICollection<EtiquetaDTO>>(list);
            return collection;
        }

        public async Task<ICollection<EtiquetaDTO>> FindByIdsAsync(List<int> ids)
        {
            var etiquetas = await _repository.FindByIdsAsync(ids);
            var etiquetasDTO = _mapper.Map<ICollection<EtiquetaDTO>>(etiquetas);
            return etiquetasDTO;
        }

    }
}
