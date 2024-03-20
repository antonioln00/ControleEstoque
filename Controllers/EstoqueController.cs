using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControleEstoque.Context;
using ControleEstoque.Entities;

namespace ControleEstoque.Controllers;
[ApiController]
[Route("[controller]")]
public class EstoqueController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public EstoqueController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<Estoque>>> ObterTodos() =>
        Ok(await _context.Estoques.Include(e => e.Produto).ToListAsync());

    [HttpPost("cadastrar-estoque")]
    public async Task<ActionResult<Estoque>> CadastrarEstoque([FromBody] Estoque model)
    {
        try
        {
            if (model == null)
                return BadRequest("Dados inseridos inválidos.");

            var novoEstoque = new Estoque {
                Quantidade = model.Quantidade,
                ProdutoId = model.ProdutoId
            };

            if (novoEstoque == null)
                return BadRequest("Novo estoque inválido.");

            _context.Estoques.Add(novoEstoque);
            await _context.SaveChangesAsync();

            return Ok(model);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("atualizar-estoque/{id:int}")]
    public async Task<ActionResult<Estoque>> AtualizarEstoque(int id, decimal quantidade)
    {
        try
        {
            if (quantidade == 0)
                return BadRequest("Insira uma quantidade válida.");

            var estoque = await _context.Estoques.FindAsync(id);

            if (estoque == null)
                return BadRequest($"Estoque de ID {id} não existe.");

            estoque.Quantidade = quantidade;

            _context.Estoques.Update(estoque);
            await _context.SaveChangesAsync();

            return Ok(estoque);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpDelete("deletar-estoque/{id}")]
    public async Task<ActionResult> DeletarEstoque(int id)
    {
        try
        {
            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.Id == id);

            if (estoque == null)
                return BadRequest($"Estoque de ID {id} não existe.");

            if (estoque.Quantidade != 0)
                return BadRequest($"Não foi possível deletar porque há {estoque.Quantidade} unidades em estoque.");

            _context.Estoques.Remove(estoque);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception)
        {
            
            throw;
        }
    }
}
