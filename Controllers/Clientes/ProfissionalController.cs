using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.Profissionais;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Empresa.Controllers.Clientes
{
    [Route("api/v1/profissional")]
    [ApiController]
    [Authorize]
    public class ProfissionalController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public ProfissionalController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Profissional>>> ObterProfissional()
        {
            var prof =  await _database.Profissional
                                    .AsNoTracking()
									.Where(c => c.Ativo == "S")
                                    .OrderByDescending(c => c.Id)
                                    .ToListAsync();
            
            if (prof != null)
            {
                return Ok(prof);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Profissionais cadastrados"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Profissional>> CadastrarProfissional([FromBody]ProfissionalCadastrar profissional)
        {
            if (ModelState.IsValid)
            {
                Profissional jaExiste = await _database.Profissional.FirstOrDefaultAsync(p => p.Nome == profissional.Nome && p.DataNascimento == profissional.DataNascimento);

                if (jaExiste != null)
                {
                    return Ok(new {
                        status = false,
                        msg = $"O Profissional {profissional.Nome} já foi cadastrado"
                    });
                }

                Profissional novoProfissional = _mapper.Map<Profissional>(profissional);
                novoProfissional.CreatedAt = DateTime.Now;
                novoProfissional.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.Profissional.AddAsync(novoProfissional);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"O Profissional {novoProfissional.Nome} foi cadastrado com sucesso",
                       novoProfissional
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Erro ao cadastrar, verifique novamente",
                        erro = e.Message
                    });
                }
            } else {
                return BadRequest(new {
                    status = false,
                    msg = "Preencha todos os campos obrigatórios"
                });
            }
        }
		
		[HttpPut("{id}")]
        public async Task<ActionResult> AlterarProfissional([FromRoute]int id,[FromBody]Profissional profissional)
        {
            if (ModelState.IsValid)
            {
                if (id != profissional.Id)
                {
                    return NotFound(new {status = false, msg = "Erro ao atualizar, Profissional não encontrado"});
                }

                if (await _database.Profissional.Where(p => p.Nome == profissional.Nome && profissional.Id != id).FirstOrDefaultAsync() != null)
                {
                    return BadRequest($"Erro ao atualizar, o Nome {profissional.Nome} já existe");
                }

                profissional.UpdatedAt = DateTime.Now;
                profissional.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(profissional).State = EntityState.Modified;
                _database.Entry(profissional).Property(p => p.Ativo).IsModified = false;
                _database.Entry(profissional).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(profissional).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(profissional).Property(p => p.DeletedAt).IsModified = false;
                _database.Entry(profissional).Property(p => p.DeletedBy).IsModified = false;

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Profissional alterado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        msg = "Erro ao alterar o Profissional, verifique novamente", 
                        status = false,
                        erro = e.Message
                    });
                }
            } else {
                return BadRequest(new {
                    status = false,
                    msg = "Preencha todos os campos obrigatórios!"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DesativarProfissional([FromRoute]int id)
        {
            Profissional profissional = await _database.Profissional.FindAsync(id);

            if (profissional == null)
            {
                return NotFound("O Profissional não foi encontrado");
            }
            
            profissional.Ativo = "N";
            profissional.DeletedAt = DateTime.Now;
            profissional.DeletedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
            _database.Entry(profissional).State = EntityState.Modified;
            _database.Entry(profissional).Property(p => p.CreatedAt).IsModified = false;
            _database.Entry(profissional).Property(p => p.CreatedBy).IsModified = false;
            _database.Entry(profissional).Property(p => p.UpdatedAt).IsModified = false;
            _database.Entry(profissional).Property(p => p.UpdatedBy).IsModified = false;

            try {
                await _database.SaveChangesAsync();
                return Ok(new {
                    status = true,
                    msg = "O Profissional foi desativado com sucesso"
                });
            } catch (Exception e) {
                return BadRequest(new {
                    status = false,
                    msg = "Não foi possível desativar Profissional, tente novamente mais tarde",
                    erro = e.Message
                });
            }
        }
    }
}