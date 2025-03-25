using IntroAPI.DTOs;
using IntroAPI.Enums;
using IntroAPI.Models;
using IntroAPI.Repository;

namespace IntroAPI.Services
{
    public class TransacaoService
    {
        private readonly TransacaoRepository _transacaoRepository;

        public TransacaoService(TransacaoRepository transacaoRepository)
        {
            _transacaoRepository = transacaoRepository;
        }

        public List<PagamentoResponse> CalcularParcelas(PagamentoRequest pagamentoRequest)
        {
            try
            {
                var parcelas = new List<PagamentoResponse>();

                var valorParcela = pagamentoRequest.ValorTotal * pagamentoRequest.TaxaJuros / pagamentoRequest.QuantidadeParcelas;

                for (int i = 0; i < pagamentoRequest.QuantidadeParcelas; i++)
                {
                    parcelas.Add(new PagamentoResponse
                    {
                        Parcela = i + 1,
                        Valor = valorParcela,
                    });
                }

                return parcelas;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CriarPagamentoResponseDTO EfetuarPagamento(CriarPagamentoDTO criarPagamentoDTO)
        {
            try
            {
                if (criarPagamentoDTO.Valor <= 0)
                {
                    throw new Exception("Valor total deve ser maior que zero");
                }

                Transacao novaTransacao = new Transacao
                {
                    Valor = criarPagamentoDTO.Valor,
                    Parcelas = criarPagamentoDTO.Parcelas,
                    Situacao = SituacaoEnum.PENDENTE, 
                    CVV = criarPagamentoDTO.CVV,
                    Cartao = criarPagamentoDTO.Cartao,
                };

                if (_transacaoRepository.Adicionar(novaTransacao))
                {
                    return new CriarPagamentoResponseDTO
                    {
                        transacaoId = (int)novaTransacao.TransacaoId,
                        Valor = (decimal)novaTransacao.Valor,
                        Parcelas = (int)novaTransacao.Parcelas,
                        situacaoEnum = (SituacaoEnum)novaTransacao.Situacao,
                        CVV = novaTransacao.CVV,
                        Cartao = novaTransacao.Cartao
                    };
                }
                else
                {
                    throw new Exception("Erro ao criar transação");
                }
            }
            catch (Exception ex)
            {
                // Lançando a exceção novamente com a mensagem detalhada
                throw new Exception($"Erro ao efetuar pagamento: {ex.Message}");
            }
        }

        public SituacaoEnum getStatusTransacao(int id)
        {
            try
            {
                var transacao = _transacaoRepository.ObterPorId(id);

                if (transacao == null)
                {
                    throw new Exception("Transação não encontrada");
                }

                return (SituacaoEnum)transacao.Situacao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Transacao ConfirmarPagamento (int id){
            try
            {
                var transacao = _transacaoRepository.ObterPorId(id);

                if (transacao == null)
                {
                    throw new Exception("Transação não encontrada");
                }
                if(transacao.Situacao == SituacaoEnum.CONFIRMADO)
                {
                    throw new Exception("Transação já confirmada");
                }
                if(transacao.Situacao == SituacaoEnum.CANCELADO)
                {
                    throw new Exception("Transação cancelada");
                }
                transacao.Situacao = SituacaoEnum.CONFIRMADO;

                _transacaoRepository.Atualizar(transacao);

                return transacao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Transacao CancelarPagamento (int id){
            try
            {
                var transacao = _transacaoRepository.ObterPorId(id);

                if (transacao == null)
                {
                    throw new Exception("Transação não encontrada");
                }
                if(transacao.Situacao == SituacaoEnum.CANCELADO)
                {
                    throw new Exception("Transação já cancelada");
                }
                if(transacao.Situacao == SituacaoEnum.CONFIRMADO)
                {
                    throw new Exception("Transação já confirmada");
                }

                transacao.Situacao = SituacaoEnum.CANCELADO;

                _transacaoRepository.Atualizar(transacao);

                return transacao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
