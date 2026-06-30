using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Domain.Entities.Users;

namespace ToDo.Infrastructure
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Rastgele, tahmin edilemez bir Refresh Token dizesi üreten metot
        public string CreateRefreshToken()
        {
            var randomNumber = new byte[64];

            // İşletim sisteminin donanımsal kriptografi motorunu çağırıyoruz
            using (var rng = RandomNumberGenerator.Create())
                // Boş kabımızı tamamen rastgele ve güvenli byte'larla dolduruyoruz
                rng.GetBytes(randomNumber);

            // Elde ettiğimiz bu byte dizisini, taşınabilir upuzun bir string'e çevirip dönüyoruz
            return Convert.ToBase64String(randomNumber);
        }

        //Kullanıcı login olduğunda eline vereceğimiz o dijital kimlik karıtnı (Token) üreten metot
        public string CreateToken(AppUser user)
        {
            // 1. Güvenlik Duvarı: Adamın veritabanında bir adı yoksa ona kimlik basamayız. 
            if (user.name == null)
                throw new NotFoundException("Username or password incorrect");

            // 2. Claim'ler (Kimlik Bilgileri): Bunlar kimliğin üzerinde basılacak yazılar.
            // Kullanıcı API'ye her istek attığında, API bu claim'lere bakarak adamı tanır.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Role, user.role)
            };

            // 3. SecretKey (Devlet Mührü): Bu kimliği taklit edilemez yapan en önemli parça.
            // Asla dışarı sızmamalı (appsettings.json'da durur).
            var secretKey = _configuration["JwtOptions:SecretKey"];

            // Mühür yoksa sistemi durdur, sahte kimlik basmaktan iyi.
            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("JWT SecretKey is missing in appsetting.json");

            // 4. Mührü dijital bir anahtara dönüştürme (String şifreyi Byte dizisine çevirme) 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // 5. İmzayı Atma: Belirlediğimiz anahtarı (key) ve zorlu bir kripto algoritmasını
            // kullanarak "imza kalemimizi" (creds) hazırlıyoruz.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 6. Kimliğin (Token'ın) İnşası: Header ve Payload (Gövde) burada birleşir.
            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"], // Bu kimliği kim üretti? 
                audience: _configuration["JwtOptions:Audience"], // Bu kimlik nerede geçerli?
                claims: claims, // Üzerinde yazan bilgiler
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtOptions:ExpireMinutes"])), // Kimliğin son kullanma tarihi 
                signingCredentials: creds
            );

            // Oluşan bu karmaşık JwtSecurityToken nesnesini,
            // front kolay anlayabilsin diye uzun string'e çeviriyoz
            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }
    }
}
