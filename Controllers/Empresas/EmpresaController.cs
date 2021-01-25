using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Empresas;
using API.Utils;
using Brokers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Empresas
{
    [Route("api/v1/empresa")]
    [ApiController]
    [Authorize]
    public class EmpresaController : ControllerBase
    {
        private readonly Database _database;
        private readonly JWT _jwt;

        public EmpresaController(Database database)
        {
            _database = database;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Empresa>>> ObterEmpresas()
        {
            var empresa = await _database.Empresa.AsNoTracking().ToListAsync();

            if (empresa != null)
            {
                return Ok(empresa);
            }
            else {
                return BadRequest("Não Existe Empresas cadastradas no momento");
            }
        }

        [HttpPost("cadastro")]
        public async Task<ActionResult> CadastraNovaEmpresa(EmpresaCadastro empresa)
        {
            if (ModelState.IsValid)
            {
                Empresa emp = new Empresa
                {
                    GrupoEmpresaId = empresa.GrupoEmpresaId,
                    Nome = empresa.Nome,
                    CreatedAt = DateTime.Now,
                    CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext),
                    StringConexao = empresa.StringConexao,
                    UUID = Guid.NewGuid(),
                    Ativo = 'S'
                };

                _database.Add(emp);
                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new
                    {
                        status = true,
                        msg = $"A Empresa {empresa.Nome} foi cadastrada com sucesso!"
                    });
                } catch (Exception e)
                {
                    return Ok(new
                    {
                        status = false,
                        msg = "Não foi possível cadastrar a Empresa",
                        erro = e.Message
                    });
                }
            } else {
                return BadRequest(new
                {
                    status = false,
                    msg = "Erro, verificar..."
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AlterarEmpresa(int id, Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                if (id != empresa.Id)
                {
                    return NotFound("Erro ao atualizar, Empresa não encontrada");
                }

                if (await _database.Usuario.Where(e => e.Nome == empresa.Nome && e.Id != id).FirstOrDefaultAsync() != null)
                {
                    return BadRequest($"Erro ao atualizar, o Nome {empresa.Nome} já existe, tente com outro...");
                }

                empresa.UpdatedAt = DateTime.Now;
                empresa.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(empresa).State = EntityState.Modified;
                _database.Entry(empresa).Property(e => e.CreatedAt).IsModified = false;
                _database.Entry(empresa).Property(e => e.CreatedBy).IsModified = false;
                _database.Entry(empresa).Property(e => e.UUID).IsModified = false;
                _database.Entry(empresa).Property(e => e.DeletedAt).IsModified = false;
                _database.Entry(empresa).Property(e => e.DeletedBy).IsModified = false;
                _database.Entry(empresa).Property(e => e.GrupoEmpresa).IsModified = false;

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Empresa atualiza com sucesso"
                    });
                }
                catch (Exception e)
                {
                    return BadRequest(new {status = false, msg = "Erro ao fazer atualização, verifique novamente...", erro = e.Message});
                }
            } else {
                return BadRequest("Preencha todos os campos obrigatórios!");
            }
        }
    }
}