using API.Brokers;
using API.Models.Atendimentos;
using API.Models.Selects;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Empresa.Controllers.Clientes
{
    [Route("api/v1/atendimento")]
    [ApiController]
    [Authorize]
    public class AtendimentoController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public AtendimentoController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet] 
        public async Task<ActionResult<List<Atendimento>>> ObterAtendimentosAtivos()
        {
            var atendimento = await _database.Atendimento
                                    .AsNoTracking()
                                    .Include(b => b.Profissional)
                                    .Include(c => c.Cliente)
                                    .Include(d => d.Situacao)
                                    .Where(a => a.Ativo == "S")
                                    .OrderByDescending(a => a.Id)
                                    .ToListAsync();

            if (atendimento != null)
            {
                return Ok(atendimento);
            }
            else
            {
                return Ok(new
                {
                    status = false,
                    msg = "Não existem O.S cadastradas"
                });
            }
        }

        [HttpGet("finalizado")] 
        public async Task<ActionResult<List<Atendimento>>> ObterAtendimentosFinalizado()
        {
            var atendimento = await _database.Atendimento
                                    .AsNoTracking()
                                    .Include(b => b.Profissional)
                                    .Include(c => c.Cliente)
                                    .Include(d => d.Situacao)
                                    .Where(a => a.Ativo == "S" && a.SituacaoId == 3)
                                    .OrderByDescending(a => a.Id)
                                    .ToListAsync();

            if (atendimento != null)
            {
                return Ok(atendimento);
            }
            else
            {
                return Ok(new
                {
                    status = false,
                    msg = "Não existem O.S cadastradas"
                });
            }
        }

        [HttpGet("aberto")] 
        public async Task<ActionResult<List<Atendimento>>> ObterAtendimentosAberto()
        {
            var atendimento = await _database.Atendimento
                                    .AsNoTracking()
                                    .Include(b => b.Profissional)
                                    .Include(c => c.Cliente)
                                    .Include(d => d.Situacao)
                                    .Where(a => a.Ativo == "S" && a.SituacaoId == 1)
                                    .OrderByDescending(a => a.Id)
                                    .ToListAsync();

            if (atendimento != null)
            {
                return Ok(atendimento);
            }
            else
            {
                return Ok(new
                {
                    status = false,
                    msg = "Não existem O.S cadastradas"
                });
            }
        }

        [HttpGet("financeiro")] 
        public async Task<ActionResult<List<Atendimento>>> ObterAtendimentosFinanceiro()
        {
            var atendimento = await _database.Atendimento
                                    .AsNoTracking()
                                    .Include(b => b.Profissional)
                                    .Include(c => c.Cliente)
                                    .Include(d => d.Situacao)
                                    .Where(a => a.Ativo == "S" && a.SituacaoId == 2)
                                    .OrderByDescending(a => a.Id)
                                    .ToListAsync();

            if (atendimento != null)
            {
                return Ok(atendimento);
            }
            else
            {
                return Ok(new
                {
                    status = false,
                    msg = "Não existem O.S cadastradas"
                });
            }
        }

        [HttpGet("geral/total")]
        public async Task<ActionResult> ObtemGeralAtendimentoPorProfissional() 
        {
            var profissional = await _database.AtendimentoGeral
                                                .FromSqlRaw("Select p.\"Id\", p.\"Nome\", sum(i.\"Valor\") as Valor" +
                                                            " from \"Atendimento\" a" +
                                                            " join \"ItemFormaPagamento\" i on (a.\"Id\" = i.\"AtendimentoId\")" +
                                                            " join \"Profissional\" p on (a.\"ProfissionalId\" = p.\"Id\") where Date(i.\"CreatedAt\") = current_date group by p.\"Id\", p.\"Nome\";")
												.ToListAsync();
            if (profissional != null)
            {
                return Ok(profissional);
            } else {
                return Ok(new {
                    status = false,
                    msg = "Não foi possível trazer dados"
                });
            }
        }
		
		[HttpGet("geral/formapagamento")]
        public async Task<ActionResult> ObtemGeralFormaPagamentoAtendimento()
        {
            var formaPagamento = await _database.AtendimentoGeralFormaPagamento
                                        .FromSqlRaw("Select f.\"Id\", f.\"Titulo\", f.\"Descricao\", sum(i.\"Valor\") as \"Total\" from \"FormaPagamento\" as f " +
                                                    "join \"ItemFormaPagamento\" as i on (i.\"FormaPagamentoId\" = f.\"Id\") " +
                                                    "Where i.\"Ativo\" = 'S' and Date(i.\"CreatedAt\") = current_date Group by f.\"Id\", f.\"Titulo\", f.\"Descricao\" ")
                                        .OrderByDescending(a => a.Id)
                                        .ToListAsync();
            
            if (formaPagamento != null)
            {
                return Ok(formaPagamento);
            } else {
                return Ok(new {
                    status = false,
                    msg = "Não foi possível trazer dados"
                });
            }
        }

        [HttpGet("{id}")] 
        public async Task<ActionResult<List<Atendimento>>> ObterAtendimentosAtivosPorId([FromRoute]int id)
        {
            var atendimento = await _database.Atendimento
                                    .AsNoTracking()
                                    .Include(b => b.Profissional)
                                    .Include(c => c.Cliente)
                                    .Include(d => d.Situacao)
                                    .Where(a => a.Ativo == "S" && a.Id == id)
                                    .OrderByDescending(a => a.Id)
                                    .ToListAsync();

            if (atendimento != null)
            {
                return Ok(atendimento);
            }
            else
            {
                return Ok(new
                {
                    status = false,
                    msg = "A O.S solicitada não existe"
                });
            }
        }


        [HttpPost]
        public async Task<ActionResult<Atendimento>> InserirAtendimento([FromBody]AtendimentoCadastrar atendimento)
        {
            if (ModelState.IsValid)
            {
                Atendimento novoAtendimento = _mapper.Map<Atendimento>(atendimento);
                novoAtendimento.CreatedAt = DateTime.Now;
                novoAtendimento.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.Atendimento.AddAsync(novoAtendimento);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"O Atendimento - {novoAtendimento.Id} foi cadastrado com sucesso",
                       novoAtendimento
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
        public async Task<ActionResult> DesativaAtendimento([FromRoute]int id)
        {
            Atendimento atendimento = await _database.Atendimento.FindAsync(id);
            if (atendimento == null)
            {
                return NotFound("Atendimento não encontrado");
            }

            if (atendimento.Ativo == "N")
            {
                return NotFound("Atendimento já foi desativado");
            }

            atendimento.Ativo = "N";
            atendimento.DeletedAt = DateTime.Now;
            atendimento.DeletedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);

            _database.Entry(atendimento).State = EntityState.Modified;
            _database.Entry(atendimento).Property(p => p.CreatedAt).IsModified = false;
            _database.Entry(atendimento).Property(p => p.CreatedBy).IsModified = false;
            _database.Entry(atendimento).Property(p => p.UpdatedAt).IsModified = false;
            _database.Entry(atendimento).Property(p => p.UpdatedBy).IsModified = false;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new
                {
                    status = true,
                    msg = $"O Atendimento {atendimento.Id} foi excluído com sucesso"
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

        [HttpPatch("finaliza/{id}")]
        public async Task<ActionResult> FinalizaAtendimento([FromRoute]int id)
        {
            Atendimento atendimento = await _database.Atendimento.FindAsync(id);
            if (atendimento == null) {
                return Ok("Atendimento não encontrado!");
            }

            if (atendimento.Ativo == "N") {
                return Ok("Atendimento desativado/cancelado!");
            }

            if (atendimento.SituacaoId == 3) {
                return Ok("Atendimento já foi finalizado");
            }

            atendimento.SituacaoId = 3;
            _database.Entry(atendimento).State = EntityState.Modified;

            try {
                await _database.SaveChangesAsync();
                return Ok(new {
                    status = true,
                    msg = "O Atendimento foi finalizado com sucesso"
                });
            } catch (Exception e) {
                return BadRequest(new {
                    erro = e.Message,
                    status = false,
                    msg = "Erro a finalizador atendimento, tente novamente mais tarde"
                });
            }
        }

        [HttpPatch("financeiro/{id}")]
        public async Task<ActionResult> GeraFinanceiroAtendimento([FromRoute]int id)
        {
            Atendimento atendimento = await _database.Atendimento.FindAsync(id);
            if (atendimento == null) {
                return Ok("Atendimento não encontrado!");
            }

            if (atendimento.Ativo == "N") {
                return Ok("Atendimento desativado/cancelado!");
            }

            if (atendimento.SituacaoId == 3) {
                return Ok("Atendimento está finalizado");
            }

            atendimento.SituacaoId = 2;
            _database.Entry(atendimento).State = EntityState.Modified;

            try {
                await _database.SaveChangesAsync();
                return Ok(new {
                    status = true
                });
            } catch (Exception e) {
                return BadRequest(new {
                    erro = e.Message,
                    status = false,
                    msg = "Erro ao Gerar Financeiro, tente novamente mais tarde"
                });
            }
        }
    }
}