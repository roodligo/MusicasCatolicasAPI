using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace MusicasCatolicasAPI.Models
{
    public class SubCategoria
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
        public string Nome { get; set; } = "";

        [JsonIgnore]
        [ValidateNever]
        public Categoria? Categoria { get; set; }
    }
}
