namespace IntroAPI.Models
{
    public class PagamentoRequest
    {
        public decimal ValorTotal { get; set; }
        public decimal TaxaJuros { get; set; }
        public int QuantidadeParcelas { get; set; }
        
    }
}
