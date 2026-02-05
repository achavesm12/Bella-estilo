using BelaEstilo.Infraestructure.Data;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Implementations
{
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly BelaEstiloContext _context;

        public RepositoryUsuario(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Usuario>> ListAsync()
        {
            var collection = await _context.Set<Usuario>()
            .OrderBy(x => x.Nombre)
            .AsNoTracking()
            .ToListAsync();

            return collection;
        }

        public async Task<Usuario> FindByIdAsync(int id)
        {
            return await _context.Usuario
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<int> AddAsync(Usuario entity)
        {
            var collection = _context.Usuario;

            collection.Add(entity);

            await _context.SaveChangesAsync();

            return entity.IdUsuario;
        }

        public async Task UpdateAsync(int id, Usuario entity)
        {
            var existeUsuario = await _context.Usuario.FindAsync(id);

            if (existeUsuario is null)
            {
                throw new Exception("Usuario no encontrado");
            }
            existeUsuario.Nombre = entity.Nombre;
            existeUsuario.Correo = entity.Correo;
            existeUsuario.Rol = entity.Rol;

            _context.Usuario.Update(existeUsuario);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByCorreoAsync(string correo)
        {
            //Anyasync pregunta si existe al menos un regisgtro en la bd 
            return await _context.Usuario.AnyAsync(u => u.Correo == correo);
        }

        public async Task<bool> ExistsByCorreoAsync(string correo, int? excluirId = null)
        {
            return await _context.Usuario.AnyAsync(u => u.Correo == correo && (!excluirId.HasValue || u.IdUsuario != excluirId.Value));
        }

        public async Task<Usuario?> ObtenerPorCorreoYContrasenaAsync(string correo, string contrasenna)
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
                return null;

            try
            {
                // Validar que la contraseña guardada sea un hash bcrypt válido
                if (string.IsNullOrEmpty(usuario.Contrasenna) || !usuario.Contrasenna.StartsWith("$2"))
                    return null;

                bool isValid = BCrypt.Net.BCrypt.Verify(contrasenna, usuario.Contrasenna);
                if (!isValid)
                    return null;

                return usuario;
            }
            catch (Exception)
            {
                // En caso de cualquier error, devolvemos null para no bloquear el login
                return null;
            }
        }

        public async Task<Usuario> GetByNombreAsync(string nombre)
        {
            return await _context.Usuario.FirstOrDefaultAsync(u => u.Nombre == nombre);
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Correo == correo);
        }



    }
}

