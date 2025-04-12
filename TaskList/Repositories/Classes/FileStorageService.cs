using TaskList.Repositories.Interfaces;

namespace TaskList.Repositories.Classes
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FileStorageService> _logger;

        private static readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private const string _imagesContainer = "tasklists";
        public FileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ILogger<FileStorageService> logger)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string containerName, CancellationToken cancellationToken = default)
        {
            // Validate file
            ValidateFile(file);

            try
            {
                // Create directory if it doesn't exist
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads", containerName);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename with original extension
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                // Return relative path
                return Path.Combine(containerName, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file");
                throw new ApplicationException("Error saving file", ex);
            }
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }

            // Validate file size
            if (file.Length > _maxFileSize)
            {
                throw new ArgumentException($"File size exceeds the maximum limit of {_maxFileSize / (1024 * 1024)}MB");
            }

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_allowedImageExtensions.Contains(extension))
            {
                throw new ArgumentException($"Invalid file type. Allowed types: {string.Join(", ", _allowedImageExtensions)}");
            }

            // Basic content type validation
            var contentType = file.ContentType.ToLowerInvariant();
            if (!contentType.StartsWith("image/"))
            {
                throw new ArgumentException("The file is not a valid image");
            }
        }

        public async Task DeleteFileAsync(string filePath, string containerName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                var fullPath = Path.Combine(_env.ContentRootPath, "Uploads", filePath);
                if (File.Exists(fullPath))
                {
                    // File.Delete(fullPath);

                    // Delete file asynchronously with cancellation support
                    await Task.Run(() => File.Delete(fullPath), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                throw new ApplicationException("Error deleting file", ex);
            }
        }

        public string GetFileUrl(string filePath, string containerName)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request == null) return null;

                return $"{request.Scheme}://{request.Host}/uploads/{filePath.Replace('\\', '/')}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating file URL");
                return null;
            }
        }
    }

}

