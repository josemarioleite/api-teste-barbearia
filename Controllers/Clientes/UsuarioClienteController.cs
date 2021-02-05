using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.ClientesUsuarios;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace API.Controllers.Clientes
{
    [Route("api/v1/usuario")]
    [ApiController]
    [Authorize]
    public class UsuarioClienteController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public UsuarioClienteController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<ClienteUsuario>>> ObterUsuarios()
        {
            return await _database.Usuario.AsNoTracking().ToListAsync();
        }

        [HttpPost("cadastro")]
        public async Task<ActionResult<ClienteUsuario>> CriarUsuario(ClienteUsuarioCadastro usuario)
        {
            if (ModelState.IsValid)
            {
                ClienteUsuario existe = await _database.Usuario.FirstOrDefaultAsync(u => u.Login == usuario.Login);
                if (existe != null)
                {
                    return Ok(new {
                        status = false,
                        msg = $"O Login {usuario.Login} já existe, tente outro..."
                    });
                }

                ClienteUsuario existe2 = await _database.Usuario.FirstOrDefaultAsync(u => u.Nome == usuario.Nome);
                if (existe != null)
                {
                    return Ok(new {
                        status = false,
                        msg = $"O Usuário {usuario.Nome} já existe, tente outro..."
                    });
                }

                Hash hash = new Hash();
                hash.HasheiaSenha(usuario);
                ClienteUsuario novoUsuario = _mapper.Map<ClienteUsuario>(usuario);

                novoUsuario.CreatedAt = DateTime.Now;
                novoUsuario.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.Usuario.AddAsync(novoUsuario);

                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"O Usuário {usuario.Nome} foi inserido com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Erro ao cadastrar usuário",
                        erro = e.Message
                    });
                }
            } else {
                return BadRequest(new {
                    status = false,
                    msg = "Por favor, preencha todos os campos"
                });
            }
        }        
    }
}