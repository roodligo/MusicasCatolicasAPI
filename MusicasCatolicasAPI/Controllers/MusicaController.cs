using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MusicasCatolicasAPI.Data;
using MusicasCatolicasAPI.DTOs;
using MusicasCatolicasAPI.Models;
using System.Security.Cryptography;

namespace MusicasCatolicasAPI.Controllers
{
    [ApiController]
    [Route("v1/musicas")]
    public class MusicaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MusicaController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<PagedResult<Musica>>> GetAll(
            [FromQuery(Name = "indice")] int page = 1,
            [FromQuery(Name = "qtde")] int pageSize = 20,
            [FromQuery(Name = "categoriaId")] int? categoriaId = null,
            [FromQuery(Name = "subCategoriaId")] int? subCategoriaId = null,
            [FromQuery(Name = "termo")] string? termo = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 200) pageSize = 200;



            var query = _context.Musicas.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                var t = $"%{termo.Trim()}%";
                query = query.Where(c => EF.Functions.Like(c.Nome, t) || EF.Functions.Like(c.Nome, t));
            }

            if (categoriaId.HasValue && categoriaId.Value > 0)
                query = query.Where(p => p.CategoriaId == categoriaId.Value);

            if (subCategoriaId.HasValue && subCategoriaId.Value > 0)
                query = query.Where(p => p.SubCategoriaId == subCategoriaId.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(g => g.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Musica>
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
        public async Task<ActionResult<Musica>> GetById(int id)
        {
            var musica = await _context.Musicas
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Include(p => p.SubCategoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (musica == null) return NotFound();

            return Ok(musica);
        }

        [HttpPost]
        public async Task<ActionResult<Musica>> Create(MusicaDto musicaDto)
        {
            if (musicaDto == null)
                return BadRequest("O objeto não pode ser nulo");

            var musica = new Musica
            {
                Guid = Guid.NewGuid(),
                Nome = musicaDto.Nome,
                Cantor = musicaDto.Cantor,
                LivroNumero = musicaDto.LivroNumero,
                CategoriaId = musicaDto.CategoriaId,
                SubCategoriaId = musicaDto.SubCategoriaId,
                CD = musicaDto.CD,
                LivroNome = musicaDto.LivroNome,
                Observacao = musicaDto.Observacao,
                Cifra = musicaDto.Cifra,
                Partitura = musicaDto.Partitura,
                Video = musicaDto.Video,
                Mid = musicaDto.Mid
            };

            _context.Musicas.Add(musica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = musica.Id }, musica);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MusicaDto musicaDto)
        {
            var existente = await _context.Musicas
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existente == null) return NotFound();

            existente.Nome = musicaDto.Nome;
            existente.Cantor = musicaDto.Cantor;
            existente.LivroNumero = musicaDto.LivroNumero;
            existente.CategoriaId = musicaDto.CategoriaId;
            existente.SubCategoriaId = musicaDto.SubCategoriaId;
            existente.CD = musicaDto.CD;
            existente.LivroNome = musicaDto.LivroNome;
            existente.Observacao = musicaDto.Observacao;
            existente.Cifra = musicaDto.Cifra;
            existente.Partitura = musicaDto.Partitura;
            existente.Video = musicaDto.Video;
            existente.Mid = musicaDto.Mid;
            existente.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var musica = await _context.Musicas.FindAsync(id);
            if (musica == null) return NotFound();

            _context.Musicas.Remove(musica);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
