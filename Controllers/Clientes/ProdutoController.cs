using API.Brokers;
using API.Models.Produtos;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers.Clientes
{
    [Route("api/v1/produto")]
    [ApiController]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public ProdutoController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Produto>>> ObterProdutos()
        {
            var prod =  await _database.Produto
                                    .AsNoTracking()
                                    .Where(p => p.Ativo == "S")
                                    .OrderByDescending(c => c.Id)
                                    .ToListAsync();
            
            if (prod != null)
            {
                return Ok(prod);
            } else {
                return BadRequest (new {
                    status = false,
                    msg = "Não existem produtos cadastrados"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> CadastrarProduto([FromBody]ProdutoCadastrar produto)
        {
            if (ModelState.IsValid)
            {
                Produto jaExiste = await _database.Produto.FirstOrDefaultAsync(p => p.Nome == produto.Nome);

                if (jaExiste != null)
                {
                    return Ok(new {
                        status = false,
                        msg = $"O Produto {produto.Nome} já foi cadastrado"
                    });
                }

                Produto novoProduto = _mapper.Map<Produto>(produto);
                novoProduto.CreatedAt = DateTime.Now;
                novoProduto.CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                await _database.Produto.AddAsync(novoProduto);

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                       status = true,
                       msg = $"O Produto {novoProduto.Nome} foi cadastrado com sucesso",
                       novoProduto
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
        public async Task<ActionResult> AlterarProduto([FromRoute]int id,[FromBody]Produto produto)
        {
            if (ModelState.IsValid)
            {
                if (id != produto.Id)
                {
                    return NotFound(new {status = false, msg = "Erro ao atualizar, produto não encontrado"});
                }

                if (await _database.Produto.Where(p => p.Nome == produto.Nome && produto.Id != id).FirstOrDefaultAsync() != null)
                {
                    return BadRequest($"Erro ao atualizar, o Nome {produto.Nome} já existe");
                }

                produto.UpdatedAt = DateTime.Now;
                produto.UpdatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
                _database.Entry(produto).State = EntityState.Modified;
                _database.Entry(produto).Property(p => p.Ativo).IsModified = false;
                _database.Entry(produto).Property(p => p.CreatedAt).IsModified = false;
                _database.Entry(produto).Property(p => p.CreatedBy).IsModified = false;
                _database.Entry(produto).Property(p => p.DeletedAt).IsModified = false;
                _database.Entry(produto).Property(p => p.DeletedBy).IsModified = false;

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Produto alterado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        msg = "Erro ao alterar o Produto, verifique novamente", 
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
        public async Task<ActionResult> DesativarProduto([FromRoute]int id)
        {
            Produto produto = await _database.Produto.FindAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não Encontrado");
            }

            produto.Ativo = "N";
            produto.DeletedAt = DateTime.Now;
            produto.DeletedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext);
            _database.Entry(produto).State = EntityState.Modified;
            _database.Entry(produto).Property(p => p.CreatedAt).IsModified = false;
            _database.Entry(produto).Property(p => p.CreatedBy).IsModified = false;
            _database.Entry(produto).Property(p => p.UpdatedAt).IsModified = false;
            _database.Entry(produto).Property(p => p.UpdatedBy).IsModified = false;

            try {
                await _database.SaveChangesAsync();
                return Ok(new {msg = "Produto deletado com sucesso", status = true});
            } catch (Exception e) {
                return BadRequest(new {msg = "Erro ao deletar Produto, tente novamente", status = true, erro = e.Message});
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> ReativaProduto([FromRoute]int id)
        {
            Produto produto = await _database.Produto.FirstOrDefaultAsync(c => c.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            if (produto.Ativo == "S")
            {
                return BadRequest(new { status = false, msg = "Este Produto já está ativo"});
            }

            produto.Ativo = "S";
            produto.DeletedAt = null;
            _database.Entry(produto).State = EntityState.Modified;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new { status = true, msg = "Produto reativado com sucesso!" });
            } catch (Exception e)
            {
                return BadRequest(new { status = false, msg = "Não foi possível reativar o Produto", erro = e.Message });
            }
        }
    }
}
