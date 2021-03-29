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
    [Route("api/v1/caixaoperador/periodo")]
    [ApiController]
    [Authorize]
    public class PeriodoCaixaController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public PeriodoCaixaController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult> ObterPeriodoCaixa()
        {
            var periodoCaixa = await _database.PeriodoCaixa
                                    .AsNoTracking()
                                    .ToListAsync();
            
            if (periodoCaixa != null)
            {
                return Ok(periodoCaixa);
            } else {
                return Ok(new {
                    status = false,
                    msg = "Não existem Períodos Cadastrados cadastrados"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PeriodoCaixa>> InserePeriodoCaixa([FromBody]PeriodoCaixaCadastro periodoCaixa)
        {
            if (ModelState.IsValid)
            {
                PeriodoCaixa estaVazio = await _database.PeriodoCaixa.FirstOrDefaultAsync(p => p.DataAbertura == periodoCaixa.DataAbertura && p.CaixaId == periodoCaixa.CaixaId);

                if (estaVazio != null)
                {
                    return Ok(new {
                        status = false,
                        msg = $"Este Período de Caixa já foi aberto"
                    });
                }

                
                PeriodoCaixa novoPeriodoCaixa = _mapper.Map<PeriodoCaixa>(periodoCaixa);
                novoPeriodoCaixa.DataAbertura = DateTime.Now;
                novoPeriodoCaixa.UsuarioAberturaId = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.PeriodoCaixa.AddAsync(novoPeriodoCaixa);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "O Período do Caixa está aberto"
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new {
                        status = false,
                        msg = "Erro ao inserir período do caixa, verifique novamente...",
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
    }
}