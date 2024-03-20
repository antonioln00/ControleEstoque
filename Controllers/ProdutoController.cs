using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControleEstoque.Context;
using ControleEstoque.Entities;

namespace ControleEstoque.Controllers;
[ApiController]
[Route("[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ProdutoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<Produto>>> ObterTodos() =>
        Ok(await _context.Produtos.Include(e => e.Fornecedor).ToListAsync());

    [HttpPost("cadastrar-produto")]
    public async Task<ActionResult<Produto>> CadastrarProduto([FromBody]Produto model)
    {
        try
        {
            if (model == null)
                return BadRequest("Dados inseridos inválidos.");

            var novoProduto = new Produto {
                Nome = model.Nome,
                Preco = model.Preco,
                Inativo = model.Inativo,
                FornecedorId = model.FornecedorId
            };

            if (novoProduto == null)
                return BadRequest("Novo fornecedor inválido.");

            _context.Produtos.Add(novoProduto);
            await _context.SaveChangesAsync();

            return Ok(model);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("atualizar-produto/{id:int}")]
    public async Task<ActionResult<Produto>> AtualizarProduto(int id, Produto model)
    {
        try
        {
            if (model == null)
                return BadRequest("Dados inseridos inválidos.");

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                return BadRequest($"Produto de ID {id} não existe.");

            produto.Nome = model.Nome;
            produto.Preco = model.Preco;
            produto.Inativo = model.Inativo;
            produto.FornecedorId = model.FornecedorId;

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            return Ok(model);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpDelete("deletar-produto/{id}")]
    public async Task<ActionResult> DeletarProduto(int id)
    {
        try
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                return BadRequest($"Produto de ID {id} não existe.");
            
            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.ProdutoId == id);

            if (estoque != null && estoque.Quantidade != 0)
                return BadRequest($"Não foi possível deletar esse produto porque há {estoque.Quantidade} unidades em estoque.");
            
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
