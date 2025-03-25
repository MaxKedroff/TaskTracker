using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskTracker
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));

        public static string GenerateJwtToken(string username)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            

            var token = new JwtSecurityToken(
                issuer: ISSUER,
                audience: AUDIENCE,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
