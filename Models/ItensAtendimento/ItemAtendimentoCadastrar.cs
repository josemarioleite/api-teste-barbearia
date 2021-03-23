using System.ComponentModel.DataAnnotations;

namespace API.Models.ItensAtendimento
{
    public class ItemAtendimentoCadastrar
    {
        [Required(ErrorMessage = "É necessário informar o Tipo do Item")]
        public string TipoItem { get; set; }
        [Required(ErrorMessage = "É necessário informar a Quantidade")]
        public int Quantidade { get; set; } = 0;
        [Required(ErrorMessage = "É necessário informar a Descrição")]
        public string Descricao { get; set; }
        [Required(ErrorMessage = "É necessário informar o Valor Unitário")]
        public double ValorUnitario { get; set; } = 0;
        [Required(ErrorMessage = "É necessário informar o Valor Total")]
        public double ValorTotal { get; set; } = 0;
        [Required(ErrorMessage = "É necessário informar a qual Atendimento o item pertence")]
        public int AtendimentoId { get; set; }
        [Required(ErrorMessage = "É necessário informar o ID do Produto/Serviço")]
        public int ProdutoServicoId { get; set; }
    }
}