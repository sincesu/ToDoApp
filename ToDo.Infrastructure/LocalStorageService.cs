using Microsoft.AspNetCore.Http;
using ToDo.Application.Abstractions;
using ToDo.Domain.Entities.Histories;

namespace ToDo.Infrastructure
{
    public class LocalStorageService : IStorageService
    {
        // Dosyayı diske kaydeden asenkron metot (Geriye klasör yolunu döner)
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            // 1. Kullanıcının yüklediği dosyanın orijinal adını alıyoruz (Örn: "rapor.pdf")
            var originalFileName = file.FileName;

            // 2. Dosyanın sadece uzantısını koparıyoruz (Örn: ".pdf")
            var fileExtension = Path.GetExtension(originalFileName);

            // 3. Aynı isimde dosya yüklenirse çakışmasın diye benzersiz bir isim üretiyoruz (Örn: "GUID.pdf")
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // 4. Projenin ana dizinini bulup, "wwwroot/uploads" hedef klasör rotasını çiziyoruz
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // 5. Hedef klasör diskte fiziksel olarak yoksa, sistem patlamasın diye sıfırdan oluşturuyoruz
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 6. Klasör rotasıyla benzersiz dosya adını birleştirip tam hedef yolu belirliyoruz
            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            // 7. Disk ile RAM arasında veri taşıma boru hattı (tünel) açıyoruz.
            // 'using' bloğu bittiği an bu tünel otomatik kapanır ve dosya kilidi RAM'den temizlenir (RAII).
            using (var stream = new FileStream(fullPath, FileMode.Create))
                // 8. RAM'deki geçici kargo paketini (file), açtığımız boru hattından diske azar azar akıtıyoruz
                await file.CopyToAsync(stream);

            // 9. Veritabanına kaydetmek ve front-end'in erişmesi için dosyanın url yolunu dönüyoruz
            return $"/uploads/{uniqueFileName}";
        }

        // Dosyayı fiziksel olarak diskten kazıyan asenkron metot
        public async Task DeleteFileAsync(string filePath)
        {
            // 1. Veritabanından gelen "/uploads/xyz.png" yolunun başındaki '/' işaretini temizleyip tam disk yolunu buluyoruz
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));

            // KRİTİK GÜVENLİK DUVARI: Dosya diskte gerçekten var mı? (Yoksa sistem kırmızı ekran basar)
            if (File.Exists(fullPath))
                // 2. Dosya diskte mevcutsa fiziksel olarak siliyoruz
                File.Delete(fullPath);

            // 3. Metot asenkron (Task) tanımlandığı ama içinde 'await' edecek bir şey olmadığı için derleyiciye "bitti" sinyali çakıyoruz
            await Task.CompletedTask;
        }
    }
}