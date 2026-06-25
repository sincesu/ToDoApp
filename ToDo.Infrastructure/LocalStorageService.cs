using Microsoft.AspNetCore.Http;
using ToDo.Application.Abstractions;

namespace ToDo.Infrastructure
{
    public class LocalStorageService : IStorageService
    {
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var originalFileName = file.FileName;
            var fileExtension = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            return $"/uploads/{uniqueFileName}";
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            await Task.CompletedTask;
        }
    }
}
