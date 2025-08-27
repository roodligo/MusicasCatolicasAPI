using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicasCatolicasAPI.Data;
using MusicasCatolicasAPI.DTOs;
using MusicasCatolicasAPI.Models;

namespace MusicasCatolicasAPI.Controllers
{
    [ApiController]
    [Route("v1/subcategorias")]
    public class SubCategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubCategoriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SubCategoria>>> GetAll(
           [FromQuery(Name = "indice")] int page = 1,
           [FromQuery(Name = "qtde")] int pageSize = 20,
           [FromQuery(Name = "termo")] string? termo = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 200) pageSize = 200;

            var query = _context.SubCategorias.AsNoTracking().AsQueryable();

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

            var result = new PagedResult<SubCategoria>
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
        public async Task<ActionResult<SubCategoria>> GetById(int id)
        {
            var item = await _context.SubCategorias
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null) return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<SubCategoria>> Create(SimpleDto simpleDto)
        {
            if (simpleDto == null)
                return BadRequest("O objeto não pode ser nulo");

            var item = new SubCategoria
            {
                Guid = Guid.NewGuid(),
                Nome = simpleDto.Nome
            };

            _context.SubCategorias.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SimpleDto simpleDto)
        {
            var existente = await _context.SubCategorias
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
            var item = await _context.SubCategorias.FindAsync(id);
            if (item == null) return NotFound();

            var subCategoriaEmUso = await _context.Musicas.FirstOrDefaultAsync(m => m.SubCategoriaId == id);

            if (subCategoriaEmUso != null)
                return BadRequest("SubCategoria em uso!");

            _context.SubCategorias.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
