using AutoMapper;
using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Implementations;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryUsuario _repository;

        public ServiceUsuario(IMapper mapper, IRepositoryUsuario repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<ICollection<UsuarioDTO>> ListAsync()
        {

            var list = await _repository.ListAsync();
            var collection = _mapper.Map<ICollection<UsuarioDTO>>(list);
            return collection;
        }

        public async Task<UsuarioDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);

            if (entity == null)
                throw new Exception("Usuario no encontrado");

            return _mapper.Map<UsuarioDTO>(entity);
        }

        
        public async Task<int> AddAsync(UsuarioRegistroDTO dto)
        {
            //Valida que no exista el usuario
            var existeUsuario = await _repository.ExistsByCorreoAsync(dto.Correo);
            if (existeUsuario)            
                throw new Exception("El correo ya está registrado.");

            //encriptar la contraseña
            //var hasher = new PasswordHasher<Usuario>();
            //dto.Contrasenna = hasher.HashPassword(null, dto.Contrasenna);
            dto.Contrasenna = BCrypt.Net.BCrypt.HashPassword(dto.Contrasenna);


            //mapeo dto a entidad
            var usuario = _mapper.Map<Usuario>(dto);

            return await _repository.AddAsync(usuario);

        }

        public async Task UpdateAsync(int id, UsuarioDTO dto)
        {
            var existeCorreo = await _repository.ExistsByCorreoAsync(dto.Correo, id);

            if (existeCorreo)
                throw new Exception("El correo ya está registrado por otro usuario.");

            var entity = _mapper.Map<Usuario>(dto);
            await _repository.UpdateAsync(id, entity);
        }

        public async Task<UsuarioDTO?> AutenticarUsuarioAsync(string correo, string contrasenna)
        {
            var usuario = await _repository.ObtenerPorCorreoYContrasenaAsync(correo, contrasenna);

            if (usuario == null)
                return null;

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<UsuarioDTO> FindByNombreAsync(string nombre)
        {
            var entity = await _repository.GetByNombreAsync(nombre);
            return entity == null ? null : _mapper.Map<UsuarioDTO>(entity);
        }

        public async Task<UsuarioDTO?> FindByEmailAsync(string correo)
        {
            var entity = await _repository.GetByCorreoAsync(correo);
            return entity == null ? null : _mapper.Map<UsuarioDTO>(entity);
        }


    }
}
