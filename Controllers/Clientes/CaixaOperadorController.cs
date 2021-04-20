using System;
using System.Linq;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.FluxoCaixa;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Clientes
{
    [Route("api/v1/caixaoperador")]
    [ApiController]
    [Authorize]
    public class CaixaOperadorController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public CaixaOperadorController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpPost("cadastro")]
        public async Task<ActionResult<Caixa>> CadastraCaixaOperador([FromBody]CaixaCadastro caixaCadastro)
        {
            if (ModelState.IsValid)
            {
                Caixa caixaOperador = await _database.Caixa.FirstOrDefaultAsync(c => c.Nome == caixaCadastro.Nome);

                if (caixaOperador != null)
                {
                    return Ok(new {
                        status = false,
                        msg = $"O Caixa {caixaCadastro.Nome} já foi cadastrado, tente outro..."
                    });
                }

                
                Caixa novoCaixaOperador = _mapper.Map<Caixa>(caixaCadastro);
                novoCaixaOperador.CreatedAt = DateTime.Now;
                novoCaixaOperador.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.Caixa.AddAsync(novoCaixaOperador);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = $"O caixa {novoCaixaOperador.Nome} foi criado com sucesso!"
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new {
                        status = false,
                        msg = "Erro ao cadastrar, verifique novamente...",
                        erro = ex.Message
                    });
                }
            } else {
                return BadRequest(new {
                    status = false,
                    msg = "Preencha todos os campos obrigatórios"
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> ObterCaixa()
        {
            var caixa = await _database.Caixa
                                    .AsNoTracking()
                                    .Where(c => c.Ativo == "S")
                                    .OrderByDescending(c => c.CaixaAberto)
                                    .ToListAsync();
            
            if (caixa != null)
            {
                return Ok(caixa);
            } else {
                return BadRequest(new {
                    status = false,
                    msg = "Não existem Caixas cadastrados"
                });
            }
        }

        [HttpPut("aberturafechamento/{id}/{abreFecha}")]
        public async Task<ActionResult> AbreCaixa([FromRoute]int id, [FromRoute]string abreFecha)
        {
            if (ModelState.IsValid)
            {
                Caixa caixa = await _database.Caixa.FirstOrDefaultAsync(c => c.Id == id);

                if (id != caixa.Id)
                {
                    return NotFound(new {
                        status = false,
                        msg = "Não foi possível atualizar, Caixa não encontrado"
                    });
                }

                if (id == caixa.Id && caixa.CaixaAberto == "S" && abreFecha == "A")
                {
                    return BadRequest(new {
                       status = false,
                       msg = $"O Caixa {caixa.Nome} já está aberto" 
                    });
                }

                if (id == caixa.Id && caixa.CaixaAberto == "N" && abreFecha == "F")
                {
                    return BadRequest(new {
                       status = false,
                       msg = $"O Caixa {caixa.Nome} já está fechado" 
                    });
                }

                if (abreFecha == "A") {
                    caixa.CaixaAberto = "S";
                } else if (abreFecha == "F") {
                    caixa.CaixaAberto = "N";
                }
                caixa.UpdatedAt = DateTime.Now;
                caixa.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(caixa).State = EntityState.Modified;
                _database.Entry(caixa).Property(p => p.Nome).IsModified = false;
                _database.Entry(caixa).Property(p => p.Ativo).IsModified = false;
                _database.Entry(caixa).Property(p => p.Observacao).IsModified = false;
                _database.Entry(caixa).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(caixa).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(caixa).Property(p => p.DeletedAt).IsModified = false;
                _database.Entry(caixa).Property(p => p.DeletedBy).IsModified = false;

                try {
                    await _database.SaveChangesAsync();
                    if (abreFecha == "A") {
                        return Ok(new {
                            status = true,
                            msg = "Caixa foi aberto"
                        });
                    } else {
                        return Ok(new {
                            status = true,
                            msg = "Caixa foi Fechado com sucesso"
                        });
                    }
                    
                } catch (System.Exception ex) {
                    return BadRequest(new {
                        status = false,
                        msg = "Erro ao abrir caixa, verifique novamente mais tarde",
                        erro = ex.Message
                    });
                }
            } else {
                return BadRequest(new {
                    status = false,
                    msg = "Preencha todos os campos válidos"
                });
            }
        }

        [HttpPut("alteracao/{id}")]
        public async Task<ActionResult> AlteraCaixa([FromRoute]int id, [FromBody]Caixa caixaOperador)
        {
            if (ModelState.IsValid)
            {                
                if (id != caixaOperador.Id)
                {
                    return NotFound(new {
                        status = false,
                        msg = "Não foi encontrado nenhum caixa"
                    });
                }

                if (await _database.Caixa.Where(c => c.Nome == caixaOperador.Nome && c.Observacao == caixaOperador.Observacao).FirstOrDefaultAsync() != null)
                {
                    return Ok(new {
                       status = false,
                       msg = "Não teve nenhuma alteração" 
                    });
                }

                caixaOperador.UpdatedAt = DateTime.Now;
                caixaOperador.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(caixaOperador).State = EntityState.Modified;
                _database.Entry(caixaOperador).Property(p => p.CaixaAberto).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.Ativo).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.DeletedAt).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.DeletedBy).IsModified = false;

                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "O caixa foi alterado com sucesso"
                    });                  
                } catch (System.Exception ex) {
                    return BadRequest(new {
                        status = false,
                        msg = "Erro ao alterar caixa, verifique novamente mais tarde",
                        erro = ex.Message
                    });
                }
            } else {
                return BadRequest(new {
                    status = false,
                    msg = "Preencha todos os campos válidos"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DesativaCaixa(int id)
        {
            Caixa caixa = await _database.Caixa.FindAsync(id);
            if (caixa == null)
            {
                return NotFound("Caixa não encontrado");
            }

            if (caixa.Ativo == "N")
            {
                return NotFound("Caixa já foi desativado");
            }

            caixa.Ativo = "N";
            caixa.DeletedAt = DateTime.Now;
            caixa.DeletedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);

            _database.Entry(caixa).State = EntityState.Modified;
            _database.Entry(caixa).Property(p => p.CreatedAt).IsModified = false;
            _database.Entry(caixa).Property(p => p.CreatedBy).IsModified = false;
            _database.Entry(caixa).Property(p => p.UpdatedAt).IsModified = false;
            _database.Entry(caixa).Property(p => p.UpdatedBy).IsModified = false;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new
                {
                    status = true,
                    msg = $"O Caixa {caixa.Nome} foi desativado com sucesso"
                });
            } catch (Exception e)
            {
                return BadRequest(new
                {
                    status = false,
                    msg = "Não foi possível fazer a exclusão, tente novamente mais tarde",
                    erro = e.Message
                });
            }
        }
    }
}