using AutoMapper;
using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Mappers;
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
    public class ServicePedidoPersonalizado : IServicePedidoPersonalizado
    {
        private const decimal _IVA = 0.13m;
        private readonly IMapper _mapper;
        private readonly IRepositoryPedido _repositoryPedido;
        private readonly IRepositoryPedidoPersonalizado _repositoryPersonalizado;

        public ServicePedidoPersonalizado(IRepositoryPedido repositoryPedido,
            IRepositoryPedidoPersonalizado repositoryPersonalizado, IMapper mapper)
        {
            _repositoryPedido = repositoryPedido;
            _repositoryPersonalizado = repositoryPersonalizado;
            _mapper = mapper;
        }

        // Para persistir en BD (usa esto en el Checkout, no en "Agregar al carrito")
        public async Task<int> AddPersonalizadoAsync(PedidoDTO pedidoDto, PedidoPersonalizadoDTO personalizadoDTO)
        {
            var criteriosSeleccionados = (personalizadoDTO.Criterios ?? new List<PedidoPersonalizadoCriterioDTO>())
                                         .Any(c => c.Seleccionado)
                ? personalizadoDTO.Criterios.Where(c => c.Seleccionado)
                : (personalizadoDTO.Criterios ?? Enumerable.Empty<PedidoPersonalizadoCriterioDTO>());

            var extras = criteriosSeleccionados.Sum(c => c.CostoExtra);
            var subtotal = personalizadoDTO.CostoBase + extras;
            var iva = Math.Round(subtotal * _IVA, 2);
            var total = subtotal + iva;

            pedidoDto.Total = total;
            pedidoDto.FechaPedido = DateTime.Now;
            if (string.IsNullOrWhiteSpace(pedidoDto.Estado)) pedidoDto.Estado = "Pendiente";

            var pedido = _mapper.Map<Pedido>(pedidoDto);
            var idPedido = await _repositoryPedido.AddAsync(pedido);

            //var idPedido = await _repositoryPedido.AddAsync(pedidoDto.ToEntity());

            personalizadoDTO.IdPedido = idPedido;
            personalizadoDTO.TotalProductoPersonalizado = total;
            var idPedidoPersonalizado = await _repositoryPersonalizado
                .AddPedidoPersonalizadoAsync(personalizadoDTO.ToEntity());

            foreach (var c in criteriosSeleccionados)
            {
                c.IdPedidoPersonalizado = idPedidoPersonalizado;
                await _repositoryPersonalizado.AddCriterioAsync(c.ToEntity());
            }

            return idPedido;
        }

        // Para la pantalla de personalización (solo lee)
        public async Task<PedidoPersonalizadoDTO> GetPedidoBaseAsync(int idProducto)
        {
            var producto = await _repositoryPersonalizado.GetProductoBaseAsync(idProducto)
                          ?? throw new InvalidOperationException("Producto base no encontrado.");

            var dto = PedidoPersonalizadoMapper.ToPersonalizadoDTO(producto);

            var opciones = await _repositoryPersonalizado.GetOpcionesPorProductoAsync(idProducto);
            dto.Criterios = opciones.Select(o => new PedidoPersonalizadoCriterioDTO
            {
                Id = o.IdOpcion,
                NombreCriterio = o.NombreCriterio,
                OpcionSeleccionada = "Sí",
                CostoExtra = o.CostoExtra,
                Seleccionado = false
            }).ToList();

            return dto;
        }

        // Crea el Pedido “vacío” para luego ir agregando líneas personalizadas
        public async Task<int> CrearPedidoVacioAsync(PedidoDTO pedidoDto)
        {
            pedidoDto.FechaPedido = DateTime.Now;
            if (string.IsNullOrWhiteSpace(pedidoDto.Estado))
                pedidoDto.Estado = "Pendiente";
            pedidoDto.Total ??= 0m; // inicia en 0; se irá acumulando

            var pedido = _mapper.Map<Pedido>(pedidoDto);
            var idPedido = await _repositoryPedido.AddAsync(pedido);

            return idPedido;
        }

        public async Task AddPersonalizadoEnPedidoExistenteAsync(int idPedido, PedidoPersonalizadoDTO dto)
        {
            var extras = (dto.Criterios ?? Enumerable.Empty<PedidoPersonalizadoCriterioDTO>()).Where(c => c.Seleccionado).Sum(c => c.CostoExtra);
            if (!(dto.Criterios?.Any(c => c.Seleccionado) ?? false))
                extras = (dto.Criterios ?? Enumerable.Empty<PedidoPersonalizadoCriterioDTO>()).Sum(c => c.CostoExtra);

            var subtotal = dto.CostoBase + extras;
            var iva = Math.Round(subtotal * _IVA, 2);
            var totalLinea = subtotal + iva;

            dto.IdPedido = idPedido;
            dto.TotalProductoPersonalizado = totalLinea;

            // 🔄 Mapear el personalizado con AutoMapper
            var personalizadoEntity = _mapper.Map<PedidoPersonalizado>(dto);
            var idPedidoPersonalizado = await _repositoryPersonalizado.AddPedidoPersonalizadoAsync(personalizadoEntity);

            // 🔄 Mapear y guardar los criterios
            var criteriosAInsertar = (dto.Criterios ?? Enumerable.Empty<PedidoPersonalizadoCriterioDTO>())
                                     .Where(c => !dto.Criterios.Any(x => x.Seleccionado) || c.Seleccionado);

            foreach (var c in criteriosAInsertar)
            {
                c.IdPedidoPersonalizado = idPedidoPersonalizado;
                var criterioEntity = _mapper.Map<PedidoPersonalizadoCriterio>(c);
                await _repositoryPersonalizado.AddCriterioAsync(criterioEntity);
            }

            // 🔄 Sumar total al pedido
            var pedidoEntity = await _repositoryPedido.FindByIdAsync(idPedido);
            pedidoEntity.Total = (pedidoEntity.Total ?? 0m) + totalLinea;
            await _repositoryPedido.UpdateAsync(pedidoEntity);
        }
    }
}
