namespace IntroAPI.Repository
{
    public interface IRepository<T> where T : class
    {
        bool Adicionar(T entity);
        bool Atualizar(T entity);
        bool Excluir(int id);
        T ObterPorId(int id);
        IEnumerable<T> ObterTodos();

    }
}
