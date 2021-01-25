using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.Servicos;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Empresa.Controllers.Clientes
{
    [Route("api/v1/servico")]
    [ApiController]
    [Authorize]
    public class ServicoController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public ServicoController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Servico>>> ObterServico()
        {
            var servicos =  await _database.Servico
                                        .AsNoTracking()
                                        .Where(s => s.Ativo == "S")
                                        .OrderByDescending(c => c.Id)
                                        .ToListAsync();
            
            if (servicos != null)
            {
                return Ok(servicos);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Serviços cadastrados"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Servico>> CadastrarServico([FromBody]ServicoCadastrar servico)
        {
            if (ModelState.IsValid)
            {
                Servico jaExiste = await _database.Servico.FirstOrDefaultAsync(c => c.Descricao == servico.Descricao && c.Valor == servico.Valor);

                if (jaExiste != null)
                {
                    return Ok(new {
                        status = false,
                        msg = $"O Serviço {servico.Descricao} já foi cadastrado"
                    });
                }

                Servico novoServico = _mapper.Map<Servico>(servico);
                novoServico.CreatedAt = DateTime.Now;
                novoServico.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.Servico.AddAsync(novoServico);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"O Serviço {servico.Descricao} foi cadastrado com sucesso",
                       novoServico
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DesativarServico([FromRoute]int id)
        {
            Servico servico = await _database.Servico.FindAsync(id);

            if (servico == null)
            {
                return NotFound("O Serviço não foi encontrado");
            }
            
            servico.Ativo = "N";
            servico.DeletedAt = DateTime.Now;
            servico.DeletedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
            _database.Entry(servico).State = EntityState.Modified;
            _database.Entry(servico).Property(p => p.CreatedAt).IsModified = false;
            _database.Entry(servico).Property(p => p.CreatedBy).IsModified = false;
            _database.Entry(servico).Property(p => p.UpdatedAt).IsModified = false;
            _database.Entry(servico).Property(p => p.UpdatedBy).IsModified = false;

            try {
                await _database.SaveChangesAsync();
                return Ok(new {
                    status = true,
                    msg = "O Serviço foi desativado com sucesso"
                });
            } catch (Exception e) {
                return BadRequest(new {
                    status = false,
                    msg = "Não foi possível desativar o serviço, tente novamente mais tarde",
                    erro = e.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AlterarServico([FromRoute]int id,[FromBody]Servico servico)
        {
            if (ModelState.IsValid)
            {
                if (id != servico.Id)
                {
                    return NotFound(new {status = false, msg = "Erro ao atualizar, serviço não encontrado"});
                }

                if (await _database.Servico.Where(s => s.Descricao == servico.Descricao && servico.Id != id).FirstOrDefaultAsync() != null)
                {
                    return BadRequest($"Erro ao atualizar, o Serviço {servico.Descricao} já existe");
                }

                servico.UpdatedAt = DateTime.Now;
                servico.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(servico).State = EntityState.Modified;
                _database.Entry(servico).Property(p => p.Ativo).IsModified = false;
                _database.Entry(servico).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(servico).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(servico).Property(p => p.DeletedAt).IsModified = false;
                _database.Entry(servico).Property(p => p.DeletedBy).IsModified = false;

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Serviço alterado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        msg = "Erro ao alterar o Serviço, verifique novamente", 
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
    }
}