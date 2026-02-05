using AutoMapper;
using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using BelaEstilo.Application.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Implementations;
using BelaEstilo.Application.DTOs.Dashboard;

namespace BelaEstilo.Application.Services.Implementations
{
    public class ServicePedido : IServicePedido
    {

        private readonly IMapper _mapper;
        private readonly IRepositoryPedido _repository;
        private readonly ILogger<ServicePedido> _logger;

        public ServicePedido(IMapper mapper, IRepositoryPedido repositoy, ILogger<ServicePedido> logger)
        {
            _mapper = mapper;
            _repository = repositoy;
            _logger = logger;
        }

        public async Task<ICollection<PedidoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            var collection = _mapper.Map<ICollection<PedidoDTO>>(list);
            return collection;
        }

        public async Task<PedidoDTO> FindByIdAsync(int id)
        {
            var pedido = await _repository.FindByIdAsync(id);
            if (pedido == null)
                return null;

            // Mapeo básico del pedido
            var pedidoDto = PedidoMapper.MapToDTO(pedido);

            // Traer productos personalizados del pedido
            var personalizados = await _repository.GetPedidoPersonalizadosByPedidoIdAsync(id);

            // Traer criterios de esos productos personalizados
            var criterios = await _repository.GetCriteriosByPedidoIdAsync(id);

            // Agrupar criterios por IdPedidoPersonalizado
            var criteriosPorPersonalizado = criterios
                .GroupBy(c => c.IdPedidoPersonalizado)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Mapear productos personalizados con sus criterios y calcular el total dinámicamente
            pedidoDto.PedidoPersonalizado = personalizados.Select(pp =>
            {
                var criteriosAsociados = criteriosPorPersonalizado.ContainsKey(pp.IdPedidoPersonalizado)
                    ? criteriosPorPersonalizado[pp.IdPedidoPersonalizado]
                    : new List<PedidoPersonalizadoCriterio>();

                return new PedidoPersonalizadoDTO
                {
                    IdPedidoPersonalizado = pp.IdPedidoPersonalizado,
                    IdPedido = pp.IdPedido,
                    IdProducto = pp.IdProducto,
                    NombreProductoPersonalizado = pp.NombreProductoPersonalizado,
                    CostoBase = pp.CostoBase,
                    Criterios = criteriosAsociados.Select(c => new PedidoPersonalizadoCriterioDTO
                    {
                        Id = c.Id,
                        IdPedidoPersonalizado = c.IdPedidoPersonalizado,
                        NombreCriterio = c.NombreCriterio,
                        OpcionSeleccionada = c.OpcionSeleccionada,
                        CostoExtra = c.CostoExtra
                    }).ToList(),
                    TotalProductoPersonalizado = pp.CostoBase + criteriosAsociados.Sum(c => c.CostoExtra)
                };
            }).ToList();

            // Subtotal de productos normales (usando el DTO)
            var subtotalProductosNormales = pedidoDto.PedidoProducto.Sum(p => p.PrecioUnitario * p.Cantidad);

            // Subtotal de productos personalizados
            var subtotalPersonalizados = pedidoDto.PedidoPersonalizado?.Sum(p => p.TotalProductoPersonalizado) ?? 0;

            // Totales
            pedidoDto.Subtotal = subtotalProductosNormales + subtotalPersonalizados;
            pedidoDto.IVA = pedidoDto.Subtotal * 0.13m;
            pedidoDto.Total = pedidoDto.Subtotal + pedidoDto.IVA;


            return pedidoDto;
        }

        public async Task<int> AddAsync(PedidoDTO dto)
        {
            var entity = new Pedido
            {
                IdUsuario = dto.IdUsuario,
                FechaPedido = dto.FechaPedido,
                Estado = dto.Estado,
                DireccionEnvio = dto.DireccionEnvio,
                MetodoPago = dto.MetodoPago
            };

            return await _repository.AddAsync(entity);
        }
        public async Task UpdateAsync(int id, PedidoDTO dto)
        {
            var entity = _mapper.Map<Pedido>(dto);
            entity.IdPedido = id;

            await _repository.UpdateAsync(entity);
        }

        public async Task<List<VentasPorDiaDTO>> ObtenerVentasPorDiaAsync(DateTime desde, DateTime hasta)
        {
            var pedidos = await _repository.GetPedidosPorRangoFechaAsync(desde, hasta);

            var agrupado = pedidos
                .GroupBy(p => p.FechaPedido.Date)
                .Select(g => new VentasPorDiaDTO
                {
                    Fecha = g.Key.ToString("yyyy-MM-dd"),
                    Total = g.Sum(p => p.Total ?? 0m)
                })
                .OrderBy(dto => dto.Fecha)
                .ToList();

            return agrupado;
        }


    }
}
