namespace IntroAPI.Repository
{
    public interface IRepository<T> where T : class
    {
        bool Adicionar(T entity);
        bool Atualizar(T entity);
        IEnumerable<T> ObterTodos();

    }
}
