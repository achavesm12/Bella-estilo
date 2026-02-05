using BelaEstilo.Application.DTOs;
using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Mappers
{
    public static class ProductoPersonalizacionOpcionMapper
    {
        public static ProductoPersonalizacionOpcionDTO ToDTO(this ProductoPersonalizacionOpcion e)
        => new()
        {
            IdOpcion = e.IdOpcion,
            IdProducto = e.IdProducto,
            NombreCriterio = e.NombreCriterio,
            CostoExtra = e.CostoExtra,
            Activo = e.Activo
        };

        public static ProductoPersonalizacionOpcion ToEntity(this ProductoPersonalizacionOpcionDTO d)
            => new()
            {
                IdOpcion = d.IdOpcion,
                IdProducto = d.IdProducto,
                NombreCriterio = d.NombreCriterio,
                CostoExtra = d.CostoExtra,
                Activo = d.Activo
            };
    }
}
