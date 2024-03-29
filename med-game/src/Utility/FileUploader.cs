﻿
namespace med_game.src.Utility
{
    public class FileUploader
    {
        public async Task<string?> UploadImage(string directoryPath, Stream stream)
        {
            string filename = Guid.NewGuid().ToString() + $".jpeg";
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
                await fileStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }

            return null;
        }
    }
}
