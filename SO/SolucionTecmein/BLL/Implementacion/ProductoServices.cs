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
    public class ProductoServices : IProductoServices
    {
        private readonly IGenericRepository<Producto> _repositorio;
        private readonly IStorageServices _storageServies;
        private readonly IUtilidadesServices _utilidadesServices;
        private readonly IEmpresaStorageServices _empresaStorageServices;
        public ProductoServices(IGenericRepository<Producto> repositorio,
                                IStorageServices storageServies,
                                IUtilidadesServices utilidadesServices,
                                IEmpresaStorageServices empresaStorageServices){
            _repositorio = repositorio;
            _storageServies = storageServies;
            _utilidadesServices = utilidadesServices;
            _empresaStorageServices = empresaStorageServices;
        }

        public async Task<Producto> Crear(Producto entidad, Stream? imagen = null, string nombreImagen = "")
        {
            try
            {
                var producto
                    = await
                        _repositorio
                        .Obtener(x =>
                                 x.Nombre == entidad.Nombre);

                if (producto != null)
                    throw new TaskCanceledException("Nombre Producto Ya Registrado");

                entidad.NombreImagen = string.IsNullOrEmpty(nombreImagen) ? $"{entidad.Nombre}_img" : nombreImagen;

                var empresaStorage = await _empresaStorageServices.Consultar();
                var almacenamientoEmpresa 
                    = empresaStorage
                      .FirstOrDefault(x => x.SecEmpresa == 1)
                        ?? throw new TaskCanceledException($"Error Empresa No ha definido un FTP");

                if (imagen != null)
                {
                    entidad.UrlImagen = await _storageServies
                                                .SubirStorage(imagen,
                                                              almacenamientoEmpresa.CarpetaUsuario,
                                                              nombreImagen);
                }

                var productoGenerado = await _repositorio.Crear(entidad);

                if (productoGenerado.Secuencial == 0)
                    throw new TaskCanceledException($"Error Usuario {entidad.Nombre} No se pudo Generar");

                var userAdquirido = await _repositorio.Consultar(x => x.Secuencial == productoGenerado.Secuencial);

                    productoGenerado 
                        = userAdquirido
                          .Include(x => x.SecTipoProductoNavigation)
                          .First();

                return productoGenerado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Producto> Editar(Producto entidad, Stream? Foto = null, string? NombreFoto = "")
        {
            try
            {
                var producto
                    = await _repositorio
                            .Consultar();

                if (producto
                    .Any(x => x.Nombre == entidad.Nombre &&
                              x.Secuencial != entidad.Secuencial))
                    throw new TaskCanceledException("Nombre Producto Ya Registrado");

               

                var productoProcesado = producto.First(x => x.Secuencial == entidad.Secuencial);
                    productoProcesado.Nombre = string.IsNullOrEmpty(entidad.Nombre) ? productoProcesado.Nombre : entidad.Nombre;
                    productoProcesado.Descripcion = string.IsNullOrEmpty(entidad.Descripcion) ? productoProcesado.Descripcion : entidad.Descripcion;
                    productoProcesado.SecTipoProducto = entidad.SecTipoProducto == 0 ? productoProcesado.SecTipoProducto : entidad.SecTipoProducto;
                    productoProcesado.Marca = entidad.Marca;
                    productoProcesado.Sistema = entidad.Sistema;
                    productoProcesado.Capacidad = entidad.Capacidad;
                    productoProcesado.Motor = entidad.Motor;
                    productoProcesado.Stock = entidad.Stock;
                    productoProcesado.Precio = entidad.Precio;
                    productoProcesado.EstaActivo = entidad.EstaActivo;

                var empresaStorage = await _empresaStorageServices.Consultar();

                var almacenamientoEmpresa 
                    = empresaStorage
                      .FirstOrDefault(x => x.SecEmpresa == 1) 
                        ?? throw new TaskCanceledException($"Error Empresa No ha definido un FTP");

                if (Foto != null)
                {
                    productoProcesado.UrlImagen 
                        = await _storageServies
                                .SubirStorage(Foto,
                                              almacenamientoEmpresa.CarpetaUsuario,
                                              NombreFoto);
                }

                var usuarioGenerado = await _repositorio.Editar(productoProcesado);

                var usuarioModificado
                      = await _repositorio
                             .Consultar(x => x.Secuencial == productoProcesado.Secuencial);
                productoProcesado = usuarioModificado.Include(x => x.SecTipoProductoNavigation).First();

                return productoProcesado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var seElimino = false;
                var producto
                    = await _repositorio
                             .Consultar(x => x.Secuencial == secuencial);

                var prod = producto.FirstOrDefault();
                if (prod != null)
                {
                    var usuarioGenerado = await _repositorio.Eliminar(prod);
                    seElimino = true;
                }
                return seElimino;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<List<Producto>> Lista()
        {
            var query = await _repositorio.Consultar();
            return query.Include(x => x.SecTipoProductoNavigation).ToList();
        }

        public async Task<Producto> OtenerPorSecuencial(int secuencial)
         => await _repositorio.Obtener(x =>
                                       x.Secuencial.Equals(secuencial));
    }
}
