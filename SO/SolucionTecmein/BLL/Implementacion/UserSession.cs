using Entity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BLL.Implementacion
{
    public class UserSession : IUserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? SecUsuario
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst("Secuencial");
                return (claim != null && int.TryParse(claim.Value, out int id)) ? id : null;
            }
        }

        public int? SecRol
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
                return (claim != null && int.TryParse(claim.Value, out int rol)) ? rol : null;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
