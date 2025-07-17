using System.IO;
using System.Threading.Tasks;

namespace EHR_FileImportHelper.Services
{
    public sealed class FileImporter : IFileImporter
    {
        public async Task ImportAsync(string sourcePath, string destinationDirectory)
        {
            var fileName = Path.GetFileName(sourcePath);
            var destPath = Path.Combine(destinationDirectory, fileName);

            Directory.CreateDirectory(destinationDirectory);

            await using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            await using var destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None);

            await sourceStream.CopyToAsync(destStream);
            sourceStream.Close();
            File.Delete(sourcePath);
        }
    }
}