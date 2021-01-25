using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.FormaPagamentos;
using API.Utils;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Empresa.Controllers.Clientes
{
    [Route("api/v1/formapagamento")]
    [ApiController]
    [Authorize]
    public class FormaPagamentoController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public FormaPagamentoController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<FormaPagamento>>> ObterFormaPagamento()
        {
            var forma =  await _database.FormaPagamento
                                    .AsNoTracking()
                                    .Where(f => f.Ativo == "S")
									.OrderByDescending(f => f.Id)
                                    .ToListAsync();
            
            if (forma != null)
            {
                return Ok(forma);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Formas de Pagamentos cadastrados"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<FormaPagamento>> CadastrarFormaPagamento([FromBody]FormaPagamento formaPagamento)
        {
            if (ModelState.IsValid)
            {
                FormaPagamento jaExiste = await _database.FormaPagamento.FirstOrDefaultAsync(c => c.Descricao == formaPagamento.Descricao && c.Ativo == formaPagamento.Ativo);

                if (jaExiste != null)
                {
                    return Ok(new {
                        status = false,
                        msg = $"Esta forma de pagamento já foi inserida no sistema"
                    });
                }

                FormaPagamento novaFormaPagamento = _mapper.Map<FormaPagamento>(formaPagamento);
                novaFormaPagamento.Ativo = "S";
                novaFormaPagamento.CreatedAt = DateTime.Now;
                novaFormaPagamento.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.FormaPagamento.AddAsync(novaFormaPagamento);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"A Forma-Pagamento {novaFormaPagamento.Descricao} foi cadastrado com sucesso"
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
        public async Task<ActionResult> AlterarOrdemServico([FromRoute]int id,[FromBody]FormaPagamento formaPagamento)
        {
            if (ModelState.IsValid)
            {
                if (id != formaPagamento.Id)
                {
                    return NotFound(new {status = false, msg = "Erro ao atualizar, Forma-Pagamento não encontrado"});
                }

                formaPagamento.UpdatedAt = DateTime.Now;
                formaPagamento.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(formaPagamento).State = EntityState.Modified;
                _database.Entry(formaPagamento).Property(p => p.Ativo).IsModified = false;
                _database.Entry(formaPagamento).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(formaPagamento).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(formaPagamento).Property(p => p.DeletedAt).IsModified = false;
                _database.Entry(formaPagamento).Property(p => p.DeletedBy).IsModified = false;

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Forma-Pagamento alterado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        msg = "Erro ao alterar o Forma-Pagamento, verifique novamente", 
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
        public async Task<ActionResult> DesativaFormaPagamento(int id)
        {
            FormaPagamento formaPagamento = await _database.FormaPagamento.FindAsync(id);
            if (formaPagamento == null)
            {
                return NotFound("Forma-Pagamento não encontrado");
            }

            formaPagamento.Ativo = "N";
            formaPagamento.DeletedAt = DateTime.Now;
            formaPagamento.DeletedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);

            _database.Entry(formaPagamento).State = EntityState.Modified;
            _database.Entry(formaPagamento).Property(p => p.CreatedAt).IsModified = false;
            _database.Entry(formaPagamento).Property(p => p.CreatedBy).IsModified = false;
            _database.Entry(formaPagamento).Property(p => p.UpdatedAt).IsModified = false;
            _database.Entry(formaPagamento).Property(p => p.UpdatedBy).IsModified = false;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new
                {
                    status = true,
                    msg = $"A Forma de Pagamento {formaPagamento.Descricao} foi excluído com sucesso"
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

        [HttpPatch("{id}")]
        public async Task<ActionResult> ReativaformaPagamento([FromRoute]int id)
        {
            FormaPagamento formaPagamento = await _database.FormaPagamento.FirstOrDefaultAsync(c => c.Id == id);
            if (formaPagamento == null)
            {
                return NotFound();
            }

            if (formaPagamento.Ativo == "S")
            {
                return BadRequest(new { status = false, msg = "Esta Forma de Pagamento já está ativo"});
            }

            formaPagamento.Ativo = "S";
            formaPagamento.DeletedAt = null;
            _database.Entry(formaPagamento).State = EntityState.Modified;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new { status = true, msg = "Forma de Pagamento reativado com sucesso!" });
            } catch (Exception e)
            {
                return BadRequest(new { status = false, msg = "Não foi possível reativar Forma de Pagamento", erro = e.Message });
            }
        }
    }
}