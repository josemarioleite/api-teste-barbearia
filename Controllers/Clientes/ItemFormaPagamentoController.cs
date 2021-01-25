using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.FormaPagamentos;
using API.Utils;
using AutoMapper;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Clientes
{
    [Route("api/v1/itemformapagamento")]
    [ApiController]
    [Authorize]
    public class ItemFormaPagamentoController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public ItemFormaPagamentoController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemFormaPagamento>>> ObterItemFormaPagamento()
        {
            var item =  await _database.ItemFormaPagamento
                                    .AsNoTracking() 
                                    .ToListAsync();
            
            if (item != null)
            {
                return Ok(item);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Itens de Forma de Pagamento cadastrados"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<ItemFormaPagamento>>> ObterItemFormaPagamentoPorId([FromRoute]int id)
        {
            var item =  await _database.ItemFormaPagamento
                                    .AsNoTracking()
                                    .Include(i => i.FormaPagamento)
                                    .Where(i => i.AtendimentoId == id)
                                    .ToListAsync();
            
            if (item != null)
            {
                return Ok(item);
            } else {
                return Ok (new {
                    status = false,
                    msg = "Não existem Itens de Pagamentos deste Atendimento"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ItemFormaPagamento>> CadastrarItemFormaPagamento([FromBody]ItemFormaPagamentoCadastrar itemFormaPagamento)
        {
            if (ModelState.IsValid)
            {
                ItemFormaPagamento novoItemFormaPagamento = _mapper.Map<ItemFormaPagamento>(itemFormaPagamento);
                novoItemFormaPagamento.CreatedAt = DateTime.Now;
                novoItemFormaPagamento.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.ItemFormaPagamento.AddAsync(novoItemFormaPagamento);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"Forma de Pagamento incluído com sucesso"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Erro ao inserir item, tente novamente mais tarde",
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
        public async Task<ActionResult<Boolean>> DesativarItemFormaPagamento([FromRoute]int id)
        {
            bool gerouErro = false;
            List<ItemFormaPagamento> itemFormaPagamento = await _database.ItemFormaPagamento.Where(i => i.AtendimentoId == id).ToListAsync();
            if (itemFormaPagamento == null)
            {
                return NotFound("Item Forma-Pagamento não Encontrado");
            }

            foreach (var item in itemFormaPagamento)
            {
                item.Ativo = "N";
                item.DeletedAt = DateTime.Now;
                item.DeletedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(item).State = EntityState.Modified;
                _database.Entry(item).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(item).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(item).Property(p => p.UpdatedAt).IsModified = false;
                _database.Entry(item).Property(p => p.UpdatedBy).IsModified = false;

                try {
                    await _database.SaveChangesAsync();
                    gerouErro = false;
                } catch (Exception) {
                    gerouErro = true;
                }
            }

            if (gerouErro == false) {
                return Ok(new {msg = "Item Forma de Pagamento deletado com sucesso", status = true});
            } else {
                return BadRequest(new {msg = "Erro ao deletar Item Forma de Pagamento, tente novamente mais tarde", status = true});
            }
        }
    }
}