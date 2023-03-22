
namespace med_game.src.Utility
{
    public class FileUploader
    {
        private readonly ILogger _logger;

        public FileUploader(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("File uploader:");
        }

        public async Task<string?> UploadImage(string directoryPath, Stream stream)
        {
            string filename = Guid.NewGuid().ToString() + $".jpeg";
            string fullPathToFile = Path.Combine(directoryPath, filename);

            using var file = File.Create(fullPathToFile);
            if (file == null)
            {
                _logger.LogInformation($"File was not loaded on the path: {directoryPath}");
                return null;
            }

            await stream.CopyToAsync(file);
            _logger.LogInformation($"File uploaded on the path: {directoryPath}");
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
                _logger.LogInformation("User received a file");
                return memoryStream.ToArray();
            }

            _logger.LogInformation("File not exist");
            return null;
        }
    }
}
