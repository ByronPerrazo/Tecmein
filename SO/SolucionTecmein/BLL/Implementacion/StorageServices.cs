using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Firebase.Auth;
using Firebase.Storage;
using System.IO;

namespace BLL.Implementacion
{
    public class StorageServices : IStorageServices
    {
        private readonly IGenericRepository<Empresastorage> _repositorio;
        private readonly string _basePath = @"C:\TecmeinFiles";

        public StorageServices(IGenericRepository<Empresastorage> repositorio)
        {
            _repositorio = repositorio;

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> SubirStorage(Stream? RepositorioExterno, string? CarpetaDestino, string? NombreArchivo)
        {
            if (RepositorioExterno == null || string.IsNullOrEmpty(NombreArchivo))
                return string.Empty;

            // Para garantizar que si la subida a Firebase falla, podamos leer el stream de nuevo en el fallback local
            Stream streamParaSubir = RepositorioExterno;
            MemoryStream? memoryStreamBackup = null;

            try
            {
                var storage = await _repositorio.Obtener(x => x.EstaActivo == 1);
                
                bool useFirebase = storage != null && 
                                   !string.IsNullOrWhiteSpace(storage.ApiKey) && 
                                   !string.IsNullOrWhiteSpace(storage.Ruta) && 
                                   !string.IsNullOrWhiteSpace(storage.Email) && 
                                   !string.IsNullOrWhiteSpace(storage.Clave);

                if (useFirebase)
                {
                    // Si el stream no soporta posicionamiento (CanSeek = false), hacemos una copia previa en memoria.
                    // Si soporta CanSeek, guardamos su posición inicial para poder rebobinarlo si falla.
                    if (!RepositorioExterno.CanSeek)
                    {
                        memoryStreamBackup = new MemoryStream();
                        await RepositorioExterno.CopyToAsync(memoryStreamBackup);
                        memoryStreamBackup.Position = 0;
                        streamParaSubir = memoryStreamBackup;
                    }
                    else
                    {
                        RepositorioExterno.Position = 0;
                    }

                    try
                    {
                        var config = new FirebaseAuthProvider(new FirebaseConfig(storage!.ApiKey));
                        var usuarioStorage = await config.SignInWithEmailAndPasswordAsync(storage.Email, storage.Clave);
                        var cancelToken = new CancellationTokenSource();
                        var tarea = new FirebaseStorage(
                                               storage.Ruta,
                                               new FirebaseStorageOptions
                                               {
                                                   AuthTokenAsyncFactory = () => Task.FromResult(usuarioStorage.FirebaseToken),
                                                   ThrowOnCancel = true
                                               })
                            .Child(CarpetaDestino)
                            .Child(NombreArchivo)
                            .PutAsync(streamParaSubir, cancelToken.Token);

                        string resultado = await tarea;
                        
                        // Si la subida fue exitosa, liberamos recursos y retornamos.
                        if (memoryStreamBackup != null)
                        {
                            await memoryStreamBackup.DisposeAsync();
                        }
                        return resultado;
                    }
                    catch (Exception)
                    {
                        // Si falla Firebase, nos preparamos para hacer el fallback local
                        if (memoryStreamBackup != null)
                        {
                            memoryStreamBackup.Position = 0;
                            streamParaSubir = memoryStreamBackup;
                        }
                        else if (RepositorioExterno.CanSeek)
                        {
                            RepositorioExterno.Position = 0;
                            streamParaSubir = RepositorioExterno;
                        }
                        // Continuamos al fallback local
                    }
                }
            }
            catch (Exception)
            {
                // Fallback a almacenamiento local ante cualquier excepción general en la validación o conexión previa
                if (memoryStreamBackup != null)
                {
                    memoryStreamBackup.Position = 0;
                    streamParaSubir = memoryStreamBackup;
                }
                else if (RepositorioExterno.CanSeek)
                {
                    RepositorioExterno.Position = 0;
                    streamParaSubir = RepositorioExterno;
                }
            }

            // Local Fallback Storage
            try
            {
                string cleanCarpetaDestino = CarpetaDestino ?? "";
                if (cleanCarpetaDestino.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    cleanCarpetaDestino = cleanCarpetaDestino.Substring(9);
                }
                else if (cleanCarpetaDestino.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    cleanCarpetaDestino = cleanCarpetaDestino.Substring(8);
                }

                string carpetaRuta = Path.Combine(_basePath, cleanCarpetaDestino);
                if (!Directory.Exists(carpetaRuta))
                {
                    Directory.CreateDirectory(carpetaRuta);
                }

                string rutaCompleta = Path.Combine(carpetaRuta, NombreArchivo);

                using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await streamParaSubir.CopyToAsync(fileStream);
                }

                return $"/uploads/{cleanCarpetaDestino}/{NombreArchivo}".Replace("\\", "/").Replace("//", "/");
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                if (memoryStreamBackup != null)
                {
                    memoryStreamBackup.Dispose();
                }
            }
        }

        public async Task<bool> EliminarStorage(string? carpetaDestino, string? nombreArchivo)
        {
            if (string.IsNullOrEmpty(nombreArchivo))
                return false;

            try
            {
                var storage = await _repositorio.Obtener(x => x.EstaActivo == 1);
                
                bool useFirebase = storage != null && 
                                   !string.IsNullOrWhiteSpace(storage.ApiKey) && 
                                   !string.IsNullOrWhiteSpace(storage.Ruta) && 
                                   !string.IsNullOrWhiteSpace(storage.Email) && 
                                   !string.IsNullOrWhiteSpace(storage.Clave);

                if (useFirebase)
                {
                    var config = new FirebaseAuthProvider(new FirebaseConfig(storage!.ApiKey));
                    var usuarioStorage = await config.SignInWithEmailAndPasswordAsync(storage.Email, storage.Clave);
                    var cancelToken = new CancellationTokenSource();
                    var tarea = new FirebaseStorage(
                                          storage.Ruta,
                                          new FirebaseStorageOptions
                                          {
                                              AuthTokenAsyncFactory = () => Task.FromResult(usuarioStorage.FirebaseToken),
                                              ThrowOnCancel = true
                                          })
                        .Child(carpetaDestino)
                        .Child(nombreArchivo)
                        .DeleteAsync();

                    await tarea;
                    return true;
                }
            }
            catch (Exception)
            {
                // Fallback to local delete on exception
            }

            // Local fallback delete
            try
            {
                string cleanCarpetaDestino = carpetaDestino ?? "";
                if (cleanCarpetaDestino.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    cleanCarpetaDestino = cleanCarpetaDestino.Substring(9);
                }
                else if (cleanCarpetaDestino.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    cleanCarpetaDestino = cleanCarpetaDestino.Substring(8);
                }

                string rutaCompleta = Path.Combine(_basePath, cleanCarpetaDestino);
                
                if (!rutaCompleta.EndsWith(nombreArchivo, StringComparison.OrdinalIgnoreCase))
                {
                    rutaCompleta = Path.Combine(rutaCompleta, nombreArchivo);
                }

                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
