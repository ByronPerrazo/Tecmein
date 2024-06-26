﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TecmeinWebApp.Utilidades.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string nombreUsuario = string.Empty;
            string urlFotoUsuario = string.Empty;

            if (claimUser != null && 
                claimUser.Identity.IsAuthenticated)
            {
                nombreUsuario =
                    claimUser
                    .Claims
                    .Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();

                urlFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlFoto").Value;
            }
            ViewData["nombreUsuario"] = nombreUsuario;
            ViewData["UrlFotoUsuario"] = urlFotoUsuario;

            return View();
        }
    } 
    
}
