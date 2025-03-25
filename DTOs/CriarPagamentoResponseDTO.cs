using IntroAPI.Enums;

namespace IntroAPI.DTOs
{
    public class CriarPagamentoResponseDTO
    {
        public int transacaoId { get; set;}
        public decimal Valor { get; set;  }
        public SituacaoEnum situacaoEnum { get; set; }
        public string Cartao { get; set; }
        public string CVV { get; set; }
        public int Parcelas { get; set; }
    }
}
