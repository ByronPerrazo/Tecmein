
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class FormatoNumeroClienteService : IFormatoNumeroClienteService
    {
        private readonly IGenericRepository<FormatoNumeroCliente> _repositorio;
        private readonly IGenericRepository<Empresa> _repositorioEmpresa;

        public FormatoNumeroClienteService(IGenericRepository<FormatoNumeroCliente> repositorio, IGenericRepository<Empresa> repositorioEmpresa)
        {
            _repositorio = repositorio;
            _repositorioEmpresa = repositorioEmpresa;
        }

        public async Task<FormatoNumeroCliente> Guardar(FormatoNumeroCliente entidad)
        {
            try
            {
                FormatoNumeroCliente formato_encontrado = await _repositorio.Obtener(f => f.SecFormatoNumeroCliente == entidad.SecFormatoNumeroCliente);

                if (formato_encontrado == null)
                {
                    // Si es nulo, es una nueva configuración. Asumimos que solo hay una empresa.
                    var primeraEmpresa = await _repositorioEmpresa.Obtener(e => e.EstaActivo == 1);
                    if (primeraEmpresa == null)
                    {
                        throw new TaskCanceledException("No se encontró una empresa activa para asociar el formato.");
                    }
                    entidad.SecEmpresa = primeraEmpresa.Secuencial;
                    formato_encontrado = await _repositorio.Crear(entidad);
                }
                else
                {
                    // Si ya existe, se actualiza.
                    formato_encontrado.UsaFormato = entidad.UsaFormato;
                    formato_encontrado.Formato = entidad.Formato;
                    formato_encontrado.NumeroInicio = entidad.NumeroInicio;
                    await _repositorio.Editar(formato_encontrado);
                }

                return formato_encontrado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FormatoNumeroCliente> Obtener()
        {
            try
            {
                IQueryable<FormatoNumeroCliente> query = await _repositorio.Consultar();
                return await query.OrderByDescending(f => f.SecFormatoNumeroCliente).FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
