using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServiceFactura
    {
        Task<byte[]> GenerarFacturaPdfAsync(int idPedido);
        Task EnviarFacturaPorCorreoAsync(string correoDestino, int idPedido);

        byte[] GenerarEjemploPdf();

    }
}
