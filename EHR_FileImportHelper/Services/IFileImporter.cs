using System.Threading.Tasks;

namespace EHR_FileImportHelper.Services
{
    public interface IFileImporter
    {
        Task ImportAsync(string sourcePath, string destinationDirectory);
    }
}