namespace TaskList.Repositories.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string containerName, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string filePath, string containerName, CancellationToken cancellationToken = default);
        string GetFileUrl(string filePath, string containerName);
    }
}
