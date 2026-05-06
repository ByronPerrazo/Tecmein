using BLL.DTOs;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPlaceholderProvider
    {
        Task ResolveAsync(Dictionary<string, string> textPlaceholders, Dictionary<string, Table> tablePlaceholders, PreContratoGeneratorDTO data);
    }
}
