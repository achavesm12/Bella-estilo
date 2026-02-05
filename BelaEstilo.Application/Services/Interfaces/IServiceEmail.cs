using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServiceEmail
    {
        Task EnviarFacturaPorCorreoAsync(string destinatario, string asunto, string mensaje,
            byte[] adjuntoPdf, string nombreAdjunto);

    }
}
