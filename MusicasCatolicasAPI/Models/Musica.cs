using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace MusicasCatolicasAPI.Models
{
    public class Musica
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
        public bool Visivel { get; set; } = true;
        public string? Nome { get; set; } = "";
        public int CategoriaId { get; set; }
        public int SubCategoriaId { get; set; }
        public string? Cantor { get; set; } = "";
        public string? LivroNumero { get; set; } = "";
        public string? CD { get; set; } = "";
        public string? LivroNome { get; set; } = "";
        public string? Observacao { get; set; } = "";
        public string? Cifra { get; set; } = "";
        public string? Letra { get; set; } = "";
        public string? Partitura { get; set; } = "";
        public string? Video { get; set; } = "";
        public string? Mid { get; set; } = "";

        [JsonIgnore]
        [ValidateNever]
        public Categoria? Categoria { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public SubCategoria? SubCategoria { get; set; }

        public static implicit operator bool(Musica? v)
        {
            throw new NotImplementedException();
        }
    }
}
