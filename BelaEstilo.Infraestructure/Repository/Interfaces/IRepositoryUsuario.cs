using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryUsuario
    {

        Task<ICollection<Usuario>> ListAsync();

        Task<Usuario> FindByIdAsync(int id);

        Task<int> AddAsync(Usuario entity);

        Task UpdateAsync(int id, Usuario entity);

        Task<bool> ExistsByCorreoAsync(string correo);

        Task<bool> ExistsByCorreoAsync(string correo, int? excluirId = null);

        Task<Usuario?> ObtenerPorCorreoYContrasenaAsync(string correo, string contrasenna);

        Task<Usuario> GetByNombreAsync(string nombre);

        Task<Usuario?> GetByCorreoAsync(string correo);


    }
}
