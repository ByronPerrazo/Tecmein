using BLL.Interfaces;
using System.Text.RegularExpressions;

namespace BLL.Implementacion
{
    public class ValidacionServices : IValidacionServices
    {
        public void ValidarCorreo(string? correo)
        {
            if (!string.IsNullOrEmpty(correo) && !Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new TaskCanceledException("El formato del correo electrónico no es válido.");
            }
        }

        public void ValidarTelefonoEcuador(string? telefono)
        {
            if (!string.IsNullOrEmpty(telefono))
            {
                string telefonoLimpio = telefono.Replace(" ", "").Replace("-", "");

                // Celular (09XXXXXXXX) - 10 dígitos
                if (Regex.IsMatch(telefonoLimpio, @"^09\d{8}$"))
                {
                    // Válido
                }
                // Convencional (0XXXXXXXX) - 9 dígitos, no 09
                else if (Regex.IsMatch(telefonoLimpio, @"^0\d{8}$") && !telefonoLimpio.StartsWith("09"))
                {
                    // Válido
                }
                // Operadora/Otros (6 o más dígitos)
                else if (telefonoLimpio.Length >= 6 && Regex.IsMatch(telefonoLimpio, @"^\d+$"))
                {
                    // Válido
                }
                else
                {
                    throw new TaskCanceledException("El formato del número de teléfono no es válido para Ecuador.");
                }
            }
        }

        public void ValidarNombre(string? nombre, string campo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new TaskCanceledException($"El {campo} es obligatorio.");
            }
        }
    }
}
