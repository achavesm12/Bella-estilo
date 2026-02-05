using BelaEstilo.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServiceUsuario
    {
        Task<ICollection<UsuarioDTO>> ListAsync();

        Task<UsuarioDTO> FindByIdAsync(int id);
        Task<UsuarioDTO> FindByNombreAsync(string nombre);

        Task<int> AddAsync(UsuarioRegistroDTO dto);

        Task UpdateAsync(int id, UsuarioDTO dto);

        Task<UsuarioDTO?> AutenticarUsuarioAsync(string correo, string contrasenna);

        Task<UsuarioDTO?> FindByEmailAsync(string correo);


    }
}
