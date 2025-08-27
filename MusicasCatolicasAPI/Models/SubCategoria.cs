namespace MusicasCatolicasAPI.Models
{
    public class SubCategoria
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
        public string Nome { get; set; } = "";
    }
}
