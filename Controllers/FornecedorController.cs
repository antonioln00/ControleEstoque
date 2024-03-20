using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControleEstoque.Context;
using ControleEstoque.Entities;

namespace ControleEstoque.Controllers;
[ApiController]
[Route("[controller]")]
public class FornecedorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FornecedorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<Fornecedor>>> ObterTodos() =>
        Ok (await _context.Fornecedores.Include(e => e.Produtos).ToListAsync());

    [HttpPost("cadastrar-fornecedor")]
    public async Task<ActionResult<Fornecedor>> CadastrarFornecedor(string nome)
    {
        try
        {
            if (nome == null)
                return BadRequest("Insira um nome válido para cadastrar fornecedor.");

            var novoFornecedor = new Fornecedor {
                Nome = nome
            };

            if (novoFornecedor == null)
                return BadRequest("Novo fornecedor inválido.");

            _context.Fornecedores.Add(novoFornecedor);
            await _context.SaveChangesAsync();

            return Ok(novoFornecedor);
        }
        catch (Exception)
        {
            throw;
        }
    }    

    [HttpPut("atualizar-fornecedor/{id:int}")]
    public async Task<ActionResult<Fornecedor>> AtualizarFornecedor(int id, string nome)
    {
        try
        {
            if (nome == null)
                return BadRequest("Insira um nome válido para cadastrar fornecedor.");

            var fornecedor = await _context.Fornecedores.FindAsync(id);

            if (fornecedor == null)
                return BadRequest($"Fornecedor de ID {id} não existe.");

            fornecedor.Nome = nome;

            _context.Fornecedores.Update(fornecedor);
            await _context.SaveChangesAsync();

            return Ok(fornecedor);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpDelete("deletar-fornecedor/{id}")]
    public async Task<ActionResult> DeletarFornecedor(int id)
    {
        try
        {
            var fornecedor = await _context.Fornecedores
                .Include(e => e.Produtos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (fornecedor == null)
                return BadRequest($"Fornecedor de ID {id} não existe.");

            if (fornecedor.Produtos.Any())
                return BadRequest("Não foi possível deletar Fornecedor porque há produtos atrelado a ele.");

            _context.Fornecedores.Remove(fornecedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
