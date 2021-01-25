using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Usuarios;
using API.Utils;
using AutoMapper;
using Brokers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace API.Controllers.Empresas
{
    [Route("api/v1/empresa/usuario")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly Database _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public UsuarioController(Database database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> ObterUsuarios()
        {
            return await _database.Usuario.AsNoTracking().ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> CriarUsuario(UsuarioCadastro usuario)
        {
            if (ModelState.IsValid)
            {
                Usuario jaExiste = await _database.Usuario.FirstOrDefaultAsync(user => user.Login == usuario.Login);
                if (jaExiste != null)
                {
                    return BadRequest(new {
                        status = false,
                        msg = $"O Login {usuario.Login} já está cadastrado, tente outro"
                    });
                }
                Hash hash = new Hash();
                hash.HasheiaSenha(usuario);
                Usuario novoUsuario = _mapper.Map<Usuario>(usuario);

                novoUsuario.CreatedAt = DateTime.Now;
                novoUsuario.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.Usuario.AddAsync(novoUsuario);

                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Usuário Cadastrado com sucesso"
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
		
		[HttpGet("token")]
        public void TokenValido()
        {
            return;
        }
    }
}