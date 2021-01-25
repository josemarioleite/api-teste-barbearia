using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.ClientesEmpresas;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Empresa.Controllers.Clientes
{
    [Route("api/v1/cliente")]
    [ApiController]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public ClienteController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> ObterClientes()
        {
            var cli =  await _database.Cliente
                                    .AsNoTracking()
									.Where(c => c.Ativo == "S")
                                    .OrderByDescending(c => c.Id)
                                    .ToListAsync();
            
            if (cli != null)
            {
                return Ok(cli);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Clientes cadastrados"
                });
            }
        }
		
		[HttpGet("inativo")]
        public async Task<ActionResult<List<Cliente>>> ObterClientesInativos()
        {
            var cli =  await _database.Cliente
                                    .AsNoTracking()
									.Where(c => c.Ativo == "N")
                                    .OrderByDescending(c => c.Id)
                                    .ToListAsync();
            
            if (cli != null)
            {
                return Ok(cli);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem Clientes cadastrados"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> CadastrarCliente([FromBody]ClienteCadastrar cliente)
        {
            if (ModelState.IsValid)
            {
                Cliente jaExiste = await _database.Cliente.FirstOrDefaultAsync(c => c.Nome == cliente.Nome);

                if (jaExiste != null)
                {
                    return BadRequest(new {
                        status = false,
                        msg = $"O Cliente {cliente.Nome} já foi cadastrado"
                    });
                }

                Cliente novoCliente = _mapper.Map<Cliente>(cliente);
                novoCliente.CreatedAt = DateTime.Now;
                novoCliente.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.Cliente.AddAsync(novoCliente);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"O Cliente {novoCliente.Nome} foi cadastrado com sucesso",
                       novoCliente
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
        public async Task<ActionResult> AlterarCliente([FromRoute]int id,[FromBody]Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                if (id != cliente.Id)
                {
                    return NotFound(new {status = false, msg = "Erro ao atualizar, cliente não encontrado"});
                }

                Cliente cli = await _database.Cliente.FindAsync(cliente.Id);

                if (cli.Id == cliente.Id && cli.Nome == cliente.Nome && cli.TelefoneCelular == cliente.TelefoneCelular)
                {
                    return BadRequest("Não houve alterações");
                }

                if (await _database.Cliente.Where(p => p.Nome == cliente.Nome && cliente.Id != id).FirstOrDefaultAsync() != null)
                {
                    return BadRequest($"Erro ao atualizar, o Nome {cliente.Nome} já existe");
                }

                cliente.UpdatedAt = DateTime.Now;
                cliente.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(cliente).State = EntityState.Modified;
                _database.Entry(cliente).Property(p => p.Ativo).IsModified = false;
                _database.Entry(cliente).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(cliente).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(cliente).Property(p => p.DeletedAt).IsModified = false;
                _database.Entry(cliente).Property(p => p.DeletedBy).IsModified = false;

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Cliente alterado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        msg = "Erro ao alterar o Cliente, verifique novamente", 
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
        public async Task<ActionResult> DesativaCliente(int id)
        {
            Cliente cliente = await _database.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }

            if (cliente.Ativo == "N")
            {
                return NotFound("Cliente já foi desativado");
            }

            cliente.Ativo = "N";
            cliente.DeletedAt = DateTime.Now;
            cliente.DeletedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);

            _database.Entry(cliente).State = EntityState.Modified;
            _database.Entry(cliente).Property(p => p.CreatedAt).IsModified = false;
            _database.Entry(cliente).Property(p => p.CreatedBy).IsModified = false;
            _database.Entry(cliente).Property(p => p.UpdatedAt).IsModified = false;
            _database.Entry(cliente).Property(p => p.UpdatedBy).IsModified = false;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new
                {
                    status = true,
                    msg = $"O Cliente {cliente.Nome} foi excluído com sucesso"
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
        public async Task<ActionResult> ReativaCliente([FromRoute]int id)
        {
            Cliente cli = await _database.Cliente.FirstOrDefaultAsync(c => c.Id == id);
            if (cli == null)
            {
                return NotFound();
            }

            if (cli.Ativo == "S")
            {
                return BadRequest(new { status = false, msg = "Este cliente já está ativo"});
            }

            cli.Ativo = "S";
            cli.DeletedAt = null;
            _database.Entry(cli).State = EntityState.Modified;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new { status = true, msg = "Cliente reativado com sucesso!" });
            } catch (Exception e)
            {
                return BadRequest(new { status = false, msg = "Não foi possível reativar cliente", erro = e.Message });
            }
        }
    }
}