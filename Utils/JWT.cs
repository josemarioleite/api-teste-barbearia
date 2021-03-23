using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using API.Models.ClientesUsuarios;
using API.Models.Usuarios;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace API.Utils
{
    public class JWT
    {
        public async Task<string> CriaTokenJWT(Usuario usuario, JwtModel jwtModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtModel.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = 
                    new ClaimsIdentity(new [] {
                    new Claim("Id", usuario.Id.ToString()),
                    new Claim("Usuario", usuario.Nome),
                    new Claim("Login", usuario.Login),
                    new Claim("Status", "true")
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public async Task<string> CriaTokenJWT(ClienteUsuario usuario, JwtModel jwtModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtModel.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject =
                    new ClaimsIdentity(new[] {
                    new Claim("Id", usuario.Id.ToString()),
                    new Claim("Usuario", usuario.Nome),
                    new Claim("Login", usuario.Login),
                    new Claim("Root", usuario.Root),
                    new Claim("PrimeiroAcesso", usuario.PrimeiroAcesso),
                    new Claim("Ativo", usuario.Ativo)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public async Task<int> RetornaIdUsuarioDoToken(HttpContext context)
        {
            var identidade = context.User.Identity as ClaimsIdentity;
            if(identidade != null)
            {
                IEnumerable<Claim> claims = identidade.Claims;
                int usuarioID = int.Parse(identidade.FindFirst("Id").Value);
                return usuarioID;
            } else {
                return 0;
            }
        }
    }
}