using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicasCatolicasAPI.Data;
using MusicasCatolicasAPI.DTOs;
using MusicasCatolicasAPI.Models;

namespace MusicasCatolicasAPI.Controllers
{
    [ApiController]
    [Route("v1/categorias")]
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Categoria>>> GetAll(
           [FromQuery(Name = "indice")] int page = 1,
           [FromQuery(Name = "qtde")] int pageSize = 20,
           [FromQuery(Name = "termo")] string? termo = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 200) pageSize = 200;

            var query = _context.Categorias.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                var t = $"%{termo.Trim()}%";
                query = query.Where(c => EF.Functions.Like(c.Nome, t) || EF.Functions.Like(c.Nome, t));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(g => g.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Categoria>
            {
                Items = items,
                TotalItems = total,
                Page = page,
                PageSize = pageSize
            };

            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetById(int id)
        {
            var musica = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (musica == null) return NotFound();

            return Ok(musica);
        }

        [HttpPost]
        public async Task<ActionResult<Musica>> Create(SimpleDto simpleDto)
        {
            if (simpleDto == null)
                return BadRequest("O objeto não pode ser nulo");

            var categoria = new Categoria
            {
                Guid = Guid.NewGuid(),
                Nome = simpleDto.Nome
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
        }

        [HttpPost("/cadastro-em-massa")]
        public async Task<ActionResult<Musica>> CreateEmMassa(List<SimpleDto> simpleDto)
        {
            if (simpleDto == null)
                return BadRequest("O objeto não pode ser nulo");

            foreach (var dto in simpleDto)
            {
                var categoria = new Categoria
                {
                    Guid = Guid.NewGuid(),
                    Nome = dto.Nome
                };
                _context.Categorias.Add(categoria);
            }
            
            await _context.SaveChangesAsync();

            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SimpleDto simpleDto)
        {
            var existente = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existente == null) return NotFound();

            existente.Nome = simpleDto.Nome;
            existente.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            var categoriaEmUso = await _context.Musicas.FirstOrDefaultAsync(m => m.CategoriaId == id);

            if (categoriaEmUso != null)
                return BadRequest("Categoria em uso!");

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
