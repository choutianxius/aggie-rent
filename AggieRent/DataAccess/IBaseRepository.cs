namespace AggieRent.DataAccess
{
    public interface IBaseRepository<TEntity>
    {
        TEntity? Get(string id);

        TEntity? GetVerbose(string id);

        IQueryable<TEntity> GetAll();

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);
    }
}
