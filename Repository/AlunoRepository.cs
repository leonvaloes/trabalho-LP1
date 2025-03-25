
using System.Net.Http.Headers;
using MySql.Data;
using MySql.Data.MySqlClient;
using Npgsql;

namespace IntroAPI.Repository
{
    public class AlunoRepository : IRepository<Aluno>
    {
        public string _stringConexao;

        public AlunoRepository()
        {
            _stringConexao = "Server=129.148.21.211;Database=professor;Uid=professor;Pwd=12345678Xx$;";
        }

        public bool Adicionar(Aluno entity)
        {
            bool sucesso = false;
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    MySqlCommand cmd = conexao.CreateCommand();
                    cmd.CommandText = @$"insert into Aluno (Nome, Idade)
                                     values (@Nome, @Idade)";

                    cmd.Parameters.AddWithValue("@Nome", entity.Nome);
                    cmd.Parameters.AddWithValue("@Idade", entity.Idade);


                    int linhaAfetadas = cmd.ExecuteNonQuery();//insert, delete, update, sp, function

                    //cmd.CommandText = @$"SELECT LAST_INSERT_ID();";
                    //cmd.ExecuteReader();

                    entity.Id = (int)cmd.LastInsertedId;

                    conexao.Close();
                    sucesso = true;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return sucesso;

        }

        public bool Adicionar(List<Aluno> entities)
        {
            bool sucesso = false;

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    MySqlTransaction trans = conexao.BeginTransaction();
                    conexao.Open();

                    try
                    {

                        MySqlCommand cmd = conexao.CreateCommand();

                        cmd.CommandText = @$"insert into Aluno (Nome, Idade)
                                     values (@Nome, @Idade)";

                        foreach (var entity in entities)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Nome", entity.Nome);
                            cmd.Parameters.AddWithValue("@Idade", entity.Idade);
                            int linhaAfetadas = cmd.ExecuteNonQuery();//insert, delete, update, sp, function
                            entity.Id = (int)cmd.LastInsertedId;
                        }
                        trans.Commit();
                        conexao.Close();
                        sucesso = true;
                    }
                    catch (MySqlException ex)
                    {
                        trans.Rollback();
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
            catch(MySqlException ex)
            {
                throw;
            }

            return sucesso;

        }


        public bool Atualizar(Aluno entity)
        {
            bool sucesso = false;
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
                {
                    conexao.Open();
                    MySqlCommand cmd = conexao.CreateCommand();
                    cmd.CommandText = @$"update Aluno   
                                    set Nome = @Nome, 
                                        Idade = @Idade
                                    where Id = @Id";

                    cmd.Parameters.AddWithValue("@Nome", entity.Nome);
                    cmd.Parameters.AddWithValue("@Idade", entity.Idade);
                    cmd.Parameters.AddWithValue("@Id", entity.Id);


                    int linhaAfetadas = cmd.ExecuteNonQuery();//insert, delete, update, sp, function

                    conexao.Close();
                    sucesso = true;
                }
            }
            catch(MySqlException ex)
            {
                throw;
            }

            return sucesso;
        }

        public bool Excluir(int id)
        {
            bool sucesso = false;

            using (MySqlConnection conexao = new MySqlConnection(_stringConexao))
            {
                conexao.Open();
                MySqlCommand cmd = conexao.CreateCommand();
                cmd.CommandText = @$"delete from Aluno where Id = {id}";

                cmd.ExecuteNonQuery();//insert, delete, update, sp, function

                sucesso = true;
            }

            return sucesso;
        }

        public Aluno ObterPorId(int id)
        {
            Aluno aluno = null;

            try
            {
                using (MySqlConnection conn =
                      new MySqlConnection(_stringConexao))
                {
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    //NpgsqlCommand cmd2 = 
                    cmd.CommandText = @$"select Id, Nome, Idade 
                                     from Aluno
                                     where Id = @Id";
                    cmd.Parameters.AddWithValue("@Id", id);
                    var dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        aluno = new Aluno();
                        //aluno.Id = (int)(dr["Id"]);
                        //aluno.Nome = dr["Nome"].ToString();
                        //aluno.Idade = Convert.ToInt32(dr["Idade"]);
                        aluno.Id = dr.GetInt32("Id");
                        aluno.Nome = dr.GetString("Nome");
                        aluno.Idade = dr.GetInt32("Idade");

                        //aluno.Id = (int)dr[0];
                    }

                    conn.Close();
                }
                //NpgsqlConnection conn2 = null;
            }
            catch (MySqlException ex)
            {
            }

            return aluno;
        }

        public IEnumerable<Aluno> ObterTodos()
        {
            List<Aluno> alunos = new List<Aluno>();

            try
            {
                using (MySqlConnection conn =
                      new MySqlConnection(_stringConexao))
                {
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    //NpgsqlCommand cmd2 = 
                    cmd.CommandText = @$"select Id, Nome, Idade 
                                     from Aluno";
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        var aluno = new Aluno();
                        //aluno.Id = (int)(dr["Id"]);
                        //aluno.Nome = dr["Nome"].ToString();
                        //aluno.Idade = Convert.ToInt32(dr["Idade"]);
                        aluno.Id = dr.GetInt32("Id");
                        aluno.Nome = dr.GetString("Nome");
                        aluno.Idade = dr.GetInt32("Idade");

                        //aluno.Id = (int)dr[0];

                        alunos.Add(aluno);
                    }

                    conn.Close();
                }
                //NpgsqlConnection conn2 = null;
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return alunos;
        }


        public int QuantidadeAluno()
        {
            int quantidade = 0;

            try
            {
                using (MySqlConnection conn =
                     new MySqlConnection(_stringConexao))
                {
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @$"select count(*)
                                     from Aluno";

                    quantidade = (int)cmd.ExecuteScalar();

                   /// conn.Close();
                }
            }
            catch (MySqlException ex) {
                //log.
                throw;
            }
            return quantidade;
        }
    }
}
