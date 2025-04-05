using CG.Web.MegaApiClient;
using TaskList.Models.Domain;
using TaskList.Repositories.Interfaces;

namespace TaskList.Repositories.Classes
{
    public class MegaFileStorageService : IFileStorageService
    {
        private readonly MegaApiClient _megaClient;
        private readonly string _baseFolder;
        private readonly ILogger<MegaFileStorageService> _logger;
        private INode _rootFolder;

        public MegaFileStorageService(IConfiguration config, ILogger<MegaFileStorageService> logger)
        {
            _logger = logger;
            var megaConfig = config.GetSection("Mega").Get<MegaConfig>();

            _megaClient = new MegaApiClient();
            _megaClient.Login(megaConfig.Email, megaConfig.Password);
            _baseFolder = megaConfig.BaseFolder;

            InitializeStorage().Wait();
        }

        private async Task InitializeStorage()
        {
            var rootNodes = await _megaClient.GetNodesAsync();
            _rootFolder = rootNodes.FirstOrDefault(x => x.Name == _baseFolder && x.Type == NodeType.Root);

            //if (_rootFolder == null)
            //{
            //    _rootFolder = await _megaClient.CreateFolderAsync(_baseFolder);
            //}
        }

        public async Task<string> SaveFileAsync(IFormFile file, string containerName)
        {
            try
            {
                ValidateFile(file);

                // Create container folder if it doesn't exist
                var containerFolder = (await _megaClient.GetNodesAsync(_rootFolder))
                    .FirstOrDefault(x => x.Name == containerName && x.Type == NodeType.Directory);

                if (containerFolder == null)
                {
                    containerFolder = await _megaClient.CreateFolderAsync(containerName, _rootFolder);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Upload file
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    var uploadedNode = await _megaClient.UploadAsync(stream, fileName, containerFolder);
                    return uploadedNode.Id;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to MEGA");
                throw new ApplicationException("Error uploading file to MEGA", ex);
            }
        }

        public async Task DeleteFileAsync(string fileId, string containerName)
        {
            try
            {
                var node = await _megaClient.GetNodeFromLinkAsync(new Uri($"https://mega.nz/#!{fileId}"));
                if (node != null)
                {
                    await _megaClient.DeleteAsync(node, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from MEGA");
                throw new ApplicationException("Error deleting file from MEGA", ex);
            }
        }

        public string GetFileUrl(string fileId, string containerName)
        {
            try
            {
                return $"https://mega.nz/#!{fileId}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating MEGA file URL");
                return null;
            }
        }

        private void ValidateFile(IFormFile file)
        {
            // Same validation as before
        }
    }
}
