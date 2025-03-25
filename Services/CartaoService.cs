using IntroAPI.Models;
using IntroAPI.Repository;

namespace IntroAPI.Services
{
    public class CartaoService
    {
        private readonly CartaoRepository _cartaoRepository;

        public string ObterBandeira(string cartao)
        {
            if (cartao.Length != 16 || !long.TryParse(cartao, out _))
            {
                return null;
            }

            string bin = cartao.Substring(0, 4); // Primeiros 4 dígitos
            char oitavoDigito = cartao[7]; // 8º dígito do cartão

            return bin switch
            {
                "1111" when oitavoDigito == '1' => "VISA",
                "2222" when oitavoDigito == '2' => "MASTERCARD",
                "3333" when oitavoDigito == '3' => "ELO",
                _ => null
            };
        }

        public bool Validar (String numeroCartao)
        {
            var cartao = _cartaoRepository.ObterPorId(numeroCartao);

            if (cartao == null || cartao.Validade < DateTime.Now)
            {
                return false;
            }
            return true;
        }

    }
}
