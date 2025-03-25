using IntroAPI.Models;
using MySql.Data.MySqlClient;
using Serilog;

namespace IntroAPI.Repository
{
    public class CartaoRepository : IRepository<Cartao>
    {
        private readonly string _stringConexao;

        public CartaoRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }
        public bool Adicionar(Cartao entity)
        {
            bool sucesso = false;

            try
            {
                using (var conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand(@"
                        INSERT INTO Cartao ( Numero, Validade)
                        Values (@Numero, @Nome);
                        SELECT LAST_INSERT_ID();", conexao))
                    {
                        cmd.Parameters.AddWithValue("@Numero", entity.Numero);
                        cmd.Parameters.AddWithValue("@Nome", entity.Validade);
                        int linhasAfetadas = cmd.ExecuteNonQuery();
                        sucesso = true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Log.Error(ex, "Erro ao adicionar Cartão");
                throw;
            }

            return sucesso;
        }

        public bool Atualizar(Cartao entity)
        {
            bool sucesso = false;

            try
            {
                using (var conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand(@"
                        UPDATE Cartao
                        SET Numero = @Numero, Validade = @Validade;
                        ", conexao))
                    {
                        cmd.Parameters.AddWithValue("@Numero", entity.Numero);
                        cmd.Parameters.AddWithValue("@Validade", entity.Validade);
                        int linhasAfetadas = cmd.ExecuteNonQuery();
                        sucesso = true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Log.Error(ex, "Erro ao atualizar Cartao");
                throw;
            }

            return sucesso;
        }

        public bool Excluir(string Numero)
        {
            bool sucesso = false;

            try
            {
                using (var conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand(@"
                        DELETE FROM Cartao
                        WHERE Numero = @Numero;
                        ", conexao))
                    {
                        cmd.Parameters.AddWithValue("@Numero", Numero);
                        int linhasAfetadas = cmd.ExecuteNonQuery();
                        sucesso = true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Log.Error(ex, "Erro ao excluir Cartao");
                throw;
            }

            return sucesso;
        }

        public Cartao ObterPorId(string Numero)
        {
            Cartao cartao = null;

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    MySqlCommand cmd = conexao.CreateCommand();
                    cmd.CommandText = "select * from Cartao where Numero = @Numero";
                    cmd.Parameters.AddWithValue("@Numero", Numero);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        cartao = new Cartao();
                        cartao.Numero = dr["Numero"].ToString();
                        cartao.Validade = Convert.ToDateTime(dr["Validade"]);
                    }
                    conexao.Close();
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return cartao;
        }

        public IEnumerable<Cartao> ObterTodos()
        {
            List<Cartao> cartoes = new List<Cartao>();

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    MySqlCommand cmd = conexao.CreateCommand();
                    cmd.CommandText = "select * from Cartao";
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Cartao cartao = new Cartao();
                        cartao.Numero = dr["Numero"].ToString();
                        cartao.Validade = Convert.ToDateTime(dr["Validade"]);
                        cartoes.Add(cartao);
                    }
                    conexao.Close();
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return cartoes;
        }
    }
}
