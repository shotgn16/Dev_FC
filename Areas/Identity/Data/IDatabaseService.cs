namespace ForestChurches.Areas.Identity.Data
{
    public interface IDatabaseService
    {
        Task<string> GetDatabaseSize(string database);
    }
}
