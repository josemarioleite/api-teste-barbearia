namespace API.Models.FluxoCaixa
{
    public class CaixaCadastro
    {
        public string Nome { get; set; }
        public string CaixaAberto { get; set; } = "N";
        public string Ativo { get; set; } = "S";
		public string Observacao { get; set; } = "S/ Observação";
    }
}