using IntroAPI.Models;
using MySql.Data.MySqlClient;
using Serilog;

namespace IntroAPI.Repository
{
    public class TransacaoRepository : IRepository<Transacao>
    {
        private readonly string _stringConexao;

        public TransacaoRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }
        public bool Adicionar(Transacao entity)
        {
            bool sucesso = false;

            try
            {
                using (var conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand(@"
                        INSERT INTO Transacao (TransacaoId, Valor, Cartao, CVV, Parcelas, Situacao)
                        VALUES (@TransacaoId, @Valor, @Cartao, @CVV, @Parcelas, @Situacao);
                        SELECT LAST_INSERT_ID();", conexao))
                    {
                        cmd.Parameters.AddWithValue("@TransacaoId", entity.TransacaoId);
                        cmd.Parameters.AddWithValue("@Valor", entity.Valor);
                        cmd.Parameters.AddWithValue("@Cartao", entity.Cartao);
                        cmd.Parameters.AddWithValue("@CVV", entity.CVV);
                        cmd.Parameters.AddWithValue("@Parcelas", entity.Parcelas);
                        cmd.Parameters.AddWithValue("@Situacao", entity.Situacao);
                        entity.TransacaoId = Convert.ToInt32(cmd.ExecuteScalar()); // Obtém o ID inserido
                        Log.Information("Transação {TransacaoId} adicionada com sucesso!", entity.TransacaoId);

                        sucesso = true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Log.Error(ex, "Erro ao adicionar transação {TransacaoId}", entity.TransacaoId);
                throw;
            }

            return sucesso;
        }

        public bool Atualizar(Transacao entity)
        {
            bool sucesso = false;
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    MySqlCommand cmd = conexao.CreateCommand();
                    cmd.CommandText = @$"update Transacao
                                    set Valor = @Valor,
                                        Cartao = @Cartao,
                                        CVV = @CVV,
                                        Parcelas = @Parcelas,
                                        Situacao = @Situacao
                                    where Id = @Id";

                    int linhaAfetadas = cmd.ExecuteNonQuery();//insert, delete, update, sp, function

                    conexao.Close();
                    sucesso = true;
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return sucesso;
        }

        public bool Excluir(int id)
        {
            bool sucesso = false;
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    MySqlCommand cmd = conexao.CreateCommand();
                    cmd.CommandText = "delete from Transacao where Id = @Id";
                    cmd.Parameters.AddWithValue("@Id", id);
                    int linhaAfetadas = cmd.ExecuteNonQuery();//insert, delete, update, sp, function

                    conexao.Close();
                    sucesso = true;
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return sucesso;
        }

        public Transacao ObterPorId(int id)
        {
            Transacao transacao = new Transacao();
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    MySqlCommand cmd = conexao.CreateCommand();
                    cmd.CommandText = "select * from Transacao where Id = @Id";
                    cmd.Parameters.AddWithValue("@Id", id);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        transacao.TransacaoId = Convert.ToInt32(dr["TransacaoId"]);
                        transacao.Valor = Convert.ToDecimal(dr["Valor"]);
                        transacao.Cartao = dr["Cartao"].ToString();
                        transacao.CVV = dr["CVV"].ToString();
                        transacao.Parcelas = Convert.ToInt32(dr["Parcelas"]);
                    }
                    conexao.Close();
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return transacao;
        }

        public IEnumerable<Transacao> ObterTodos()
        {
            List<Transacao> transacoes = new List<Transacao>();
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    MySqlCommand cmd = conexao.CreateCommand();
                    cmd.CommandText = "select * from Transacao";
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Transacao transacao = new Transacao();
                        transacao.TransacaoId = Convert.ToInt32(dr["TransacaoId"]);
                        transacao.Valor = Convert.ToDecimal(dr["Valor"]);
                        transacao.Cartao = dr["Cartao"].ToString();
                        transacao.CVV = dr["CVV"].ToString();
                        transacao.Parcelas = Convert.ToInt32(dr["Parcelas"]);
                        transacoes.Add(transacao);
                    }
                    conexao.Close();
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return transacoes;
        }
    }
}
