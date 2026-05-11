using BLL.ContractEngine;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BLL.Interfaces
{
    public interface IPlaceholderProvider
    {
        Task ResolveAsync(Dictionary<string, string> textPlaceholders, Dictionary<string, Table> tablePlaceholders, ContractEngineContext context);
    }
}
