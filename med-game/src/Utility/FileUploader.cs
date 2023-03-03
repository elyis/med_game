using Microsoft.AspNetCore.Mvc;

namespace med_game.src.Utility
{
    public class FileUploader
    {
        public async Task<string?> UploadImage(string directoryPath, Stream stream, string fileExtension)
        {
            if (fileExtension[0] == '.')
                fileExtension = fileExtension.Substring(1);

            string filename = Guid.NewGuid().ToString() + $".{fileExtension}";
            string fullPathToFile = Path.Combine(directoryPath, filename);

            using var file = File.Create(fullPathToFile);
            if (file == null)
                return null;

            await stream.CopyToAsync(file);
            return filename;
        }

        public async Task<byte[]?> GetStreamImage(string directoryPath, string filename)
        {
            string fullPathToFile = Path.Combine(directoryPath, filename);
            if (File.Exists(fullPathToFile))
            {
                using Stream fileStream = File.OpenRead(fullPathToFile);
                var memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
            return null;
        }
    }
}
