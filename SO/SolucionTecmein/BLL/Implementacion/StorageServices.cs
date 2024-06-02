using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
//using Firebase.Auth;
//using Firebase.Storage;


namespace BLL.Implementacion
{
    public class StorageServices : IStorageServices
    {
        private IGenericRepository<Empresastorage> _repositorio;
        public StorageServices(IGenericRepository<Empresastorage> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream RepositorioExterno, string CarpetaDestino, string NombreArchivo)
        {
            var UrlImagen = string.Empty;
            //try
            //{
            //    var storage
            //        = await _repositorio
            //                .Obtener(x => x.EstaActivo == true);

            //    var config
            //        = new FirebaseAuthProvider(
            //            new FirebaseConfig(storage.ApiKey));
            //    var usuarioStorage
            //        = await
            //            config
            //            .SignInWithEmailAndPasswordAsync(storage.Email,
            //                                             storage.Clave);
            //    var cancelToken = new CancellationTokenSource();
            //    var tarea
            //        = new FirebaseStorage(
            //                              storage.Ruta,
            //                              new FirebaseStorageOptions
            //                              {
            //                                  AuthTokenAsyncFactory = ()
            //                                      => Task.FromResult(usuarioStorage.FirebaseToken),
            //                                  ThrowOnCancel = true
            //                              })
            //        .Child(CarpetaDestino)
            //        .Child(NombreArchivo)
            //        .PutAsync(RepositorioExterno, cancelToken.Token);

            //    return UrlImagen = await tarea;

            //} catch (Exception){}

            return UrlImagen;
        }
        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            var seProceso = false;
            try
            {
                var storage
                    = await _repositorio
                            .Obtener(x => x.EstaActivo == 1 );

                //var config
                //    = new FirebaseAuthProvider(
                //        new FirebaseConfig(storage.ApiKey));
                //var usuarioStorage
                //    = await
                //        config
                //        .SignInWithEmailAndPasswordAsync(storage.Email,
                //                                         storage.Clave);
                //var cancelToken = new CancellationTokenSource();
                //var tarea
                //    = new FirebaseStorage(
                //                          storage.Ruta,
                //                          new FirebaseStorageOptions
                //                          {
                //                              AuthTokenAsyncFactory = ()
                //                                  => Task.FromResult(usuarioStorage.FirebaseToken),
                //                              ThrowOnCancel = true
                //                          })
                //    .Child(CarpetaDestino)
                //    .Child(NombreArchivo)
                //    .DeleteAsync();

                seProceso = true;

            }
            catch (Exception) { }

            return seProceso;

        }


    }
}
