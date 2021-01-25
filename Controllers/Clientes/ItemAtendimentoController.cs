using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.Atendimentos;
using API.Models.ItensAtendimento;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Clientes
{
    [Route("api/v1/itematendimento")]
    [ApiController]
    [Authorize]
    public class ItemAtendimentoController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public ItemAtendimentoController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemAtendimento>>> ObterItensAtendimento()
        {
            var item =  await _database.ItemAtendimento
                                    .AsNoTracking() 
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();
            
            if (item != null)
            {
                return Ok(item);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Itens cadastrados"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<ItemAtendimento>>> ObterItensPorID([FromRoute]int id)
        {
            var item =  await _database.ItemAtendimento
                                    .AsNoTracking()
                                    .Where(i => i.Id == id)
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();

            if (item != null)
            {
                return Ok(item);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Itens cadastrados"
                });
            }
        }

        [HttpGet("atendimento/{id}")]
        public async Task<ActionResult<List<ItemAtendimento>>> ObterAtendimentoPorID([FromRoute]int id)
        {
            var item =  await _database.ItemAtendimento
                                    .AsNoTracking()
                                    .Where(i => i.AtendimentoId == id)
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();

            if (item != null)
            {
                return Ok(item);
            } else {
                return Ok (new {
                    status = false,
                    msg = "Não existem Itens cadastrados nesse Atendimento"
                });
            }
        }

        [HttpGet("atendimentoproduto/{id}")]
        public async Task<ActionResult<List<ItemAtendimento>>> ObterItensPorAtendimentoProduto([FromRoute]int id)
        {
            var item =  await _database.ItemAtendimento
                                    .AsNoTracking()
                                    .Where(i => i.AtendimentoId == id && i.TipoItem == "P")
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();

            if (item != null)
            {
                return Ok(item);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Itens cadastrados"
                });
            }
        }

        [HttpGet("atendimentoservico/{id}")]
        public async Task<ActionResult<List<ItemAtendimento>>> ObterItensPorAtendimentoServico([FromRoute]int id)
        {
            var item =  await _database.ItemAtendimento
                                    .AsNoTracking()
                                    .Where(i => i.AtendimentoId == id && i.TipoItem == "S")
                                    .OrderByDescending(i => i.Id)
                                    .ToListAsync();

            if (item != null)
            {
                return Ok(item);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Itens cadastrados"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CadastrarItemAtendimento([FromBody]ItemAtendimentoCadastrar itemAtendimento)
        {
            if (ModelState.IsValid)
            {
                Atendimento existe = await _database.Atendimento.FirstOrDefaultAsync(a => a.Id == itemAtendimento.AtendimentoId);
                if (existe == null) {
                    return NotFound("Atendimento não encontrado ou não existe");
                }

                ItemAtendimento novoItemAtendimento = _mapper.Map<ItemAtendimento>(itemAtendimento);
                novoItemAtendimento.CreatedAt = DateTime.Now;
                novoItemAtendimento.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.ItemAtendimento.AddAsync(novoItemAtendimento);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"O(s) Item(s) foram cadastrado(s) com sucesso"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Erro ao cadastrar item, verifique novamente",
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
    }
}