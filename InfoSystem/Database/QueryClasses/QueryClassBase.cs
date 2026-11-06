namespace InfoSystem
{
    internal abstract class QueryClassBase
    {
        protected InfoContext _dbContext;

        public void SetDbContext(InfoContext context)
        {
            _dbContext = context;
        }
    }
}
