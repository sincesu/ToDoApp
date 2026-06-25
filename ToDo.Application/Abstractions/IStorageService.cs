using Microsoft.AspNetCore.Http;

namespace ToDo.Application.Abstractions
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);

        Task DeleteFileAsync(string filePath);
    }
}