
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

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
                var secFormato = entidad.SecFormatoNumeroCliente;
                FormatoNumeroCliente formato_encontrado = await _repositorio.Obtener(f => f.SecFormatoNumeroCliente == secFormato);

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
                    formato_encontrado.LongitudNumero = entidad.LongitudNumero;
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
