namespace IntroAPI.DTOs
{
    public class TransacaoCriarDTO
    {
        public decimal Valor { get; set;  }
        public string Cartao { get; set; }
        public string CVV { get; set; }
        public int Parcelas { get; set; }
        public int Situacao { get; set; }
    }
}
