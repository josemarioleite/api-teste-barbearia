using API.Brokers;
using API.Models;
using API.Models.ClientesUsuarios;
using API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Utils;

namespace API.Controllers.Clientes
{
    [Route("api/v1/auth")]
    [ApiController]
    public class ClienteAuthController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly JwtModel _jwtModel;

        public ClienteAuthController(ClienteDatabase database, JwtModel jwtModel)
        {
            _database = database;
            _jwtModel = jwtModel;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ClienteUsuario>> FazLogin([FromBody] ClienteUsuarioAuth usuarioLogin)
        {
            if (ModelState.IsValid)
            {
                ClienteUsuario usuario;
                try
                {
                    usuario = await _database.Usuario.FirstOrDefaultAsync(u => u.Login == usuarioLogin.Login);
                }
                catch (Exception e)
                {
                    return BadRequest(new { msg = e.ToString() });
                }

                if (usuario == null)
                {
                    return NotFound(new { status = false, msg = "O Usuário digitado não foi encontrado" });
                }
                else if (usuario.Ativo.Equals("N"))
                {
                    return Unauthorized(new { status = false, msg = "Esse Usuário está Inativo" });
                }
                else
                {
                    Hash hash = new Hash();
                    if (hash.VerificaSenhaHash(usuario, usuarioLogin.Senha))
                    {
                        var token = await new JWT().CriaTokenJWT(usuario, _jwtModel);
                        return Ok(new
                        {
                            status = true,
                            token = token,
                            idUsuario = usuario.Id,
                            primeiroAcesso = usuario.PrimeiroAcesso,
                            nome = usuario.Nome,
                            root = usuario.Root,
                            ativo = usuario.Ativo
                        });
                    }
                    else
                    {
                        return Unauthorized(new { status = false, msg = "A Senha digitada está incorreta" });
                    }
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("token")]
        public void TokenValido()
        {
            return;
        }
    }
}
