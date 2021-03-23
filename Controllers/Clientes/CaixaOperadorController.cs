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

        [HttpPut("abertura/{id}")]
        public async Task<ActionResult> AbreCaixa([FromRoute]int id, [FromBody]Caixa caixaOperador)
        {
            if (ModelState.IsValid)
            {
                var caixa = await _database.Caixa.FirstOrDefaultAsync(c => c.Id == id);

                if (id != caixaOperador.Id)
                {
                    return NotFound(new {
                        status = false,
                        msg = "Não foi possível atualizar, Caixa não encontrado"
                    });
                }

                if (await _database.Caixa.Where(c => c.CaixaAberto == caixaOperador.CaixaAberto && caixaOperador.Id == id).FirstOrDefaultAsync() != null)
                {
                    return BadRequest(new {
                       status = false,
                       msg = $"Erro ao abrir caixa, o Caixa {caixaOperador.Nome} já está aberto" 
                    });
                }

                caixaOperador.UpdatedAt = DateTime.Now;
                caixaOperador.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(caixaOperador).Property(p => p.Ativo).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.DeletedAt).IsModified = false;
                _database.Entry(caixaOperador).Property(p => p.DeletedBy).IsModified = false;
                _database.Entry(caixaOperador).State = EntityState.Modified;

                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Caixa foi aberto"
                    });
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
    }
}