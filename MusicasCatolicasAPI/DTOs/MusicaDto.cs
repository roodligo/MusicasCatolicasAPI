namespace MusicasCatolicasAPI.DTOs
{
    public class MusicaDto
    {
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
    }
}
