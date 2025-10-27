
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Implementacion
{
    public class MenuDiscoveryService : IMenuDiscoveryService
    {
        private readonly IGenericRepository<Menu> _repoMenu;
        private const string TempMenuName = "Módulos sin Asignar";

        public MenuDiscoveryService(IGenericRepository<Menu> repoMenu)
        {
            _repoMenu = repoMenu;
        }

        public async Task<string> DiscoverAndRegisterMenusAsync(Assembly controllerAssembly)
        {
            // 1. Find or create the temporary parent menu
            Menu tempParentMenu = await _repoMenu.Obtener(m => m.Descripcion == TempMenuName);
            if (tempParentMenu == null)
            {
                tempParentMenu = new Menu
                {
                    Descripcion = TempMenuName,
                    EsActivo = 1,
                    Controlador = null,
                    PaginaAccion = null,
                    Icono = "fa-cogs", // A default icon
                    FechaRegistro = DateTime.Now
                };
                tempParentMenu = await _repoMenu.Crear(tempParentMenu);
            }

            // 2. Get existing controller names from the database
            var existingControllerNames = (await _repoMenu.Consultar(m => m.Controlador != null))
                .Select(m => m.Controlador)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // 3. Discover controllers from the assembly
            var discoveredControllerNames = controllerAssembly.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type) && !type.IsAbstract && type.Name.EndsWith("Controller"))
                .Select(type => type.Name.Replace("Controller", ""))
                .Where(name => name != "Acceso" && name != "Home")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // 4. Identify new menus to add
            var newControllerNames = discoveredControllerNames.Except(existingControllerNames);
            var newMenusToAdd = new List<Menu>();

            foreach (var controllerName in newControllerNames)
            {
                var newMenu = new Menu
                {
                    Descripcion = controllerName,
                    Controlador = controllerName,
                    PaginaAccion = "Index",
                    SecMenuPadre = tempParentMenu.Secuencial,
                    EsActivo = 1,
                    Icono = "fa-puzzle-piece", // A default icon for new items
                    FechaRegistro = DateTime.Now
                };
                newMenusToAdd.Add(newMenu);
            }

            // 5. Save new menus to the database
            if (newMenusToAdd.Any())
            {
                foreach (var menu in newMenusToAdd)
                {
                    await _repoMenu.Crear(menu);
                }
            }

            // 6. Formulate the response message
            if (newMenusToAdd.Any())
            {
                return $"{newMenusToAdd.Count} nuevo(s) módulo(s) agregado(s) bajo '{TempMenuName}'. Por favor, categorícelos desde la gestión de menús.";
            }
            else
            {
                return "No se encontraron nuevos módulos para agregar.";
            }
        }
    }
}
