using ForestChurches.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ForestChurches.Areas.Identity.Data
{
    public class TruncateTables
    {
        private readonly ForestChurchesContext _context;
        string _tableName;
        public TruncateTables(string tableName, ForestChurchesContext context)
        {
            _context = context;
            _tableName = tableName;
        }

        public async Task<bool> tableHasRows()
        {
            var dbSetProperty = _context.GetType().GetProperty(_tableName);
            if (dbSetProperty == null)
            {
                throw new ArgumentException($"Table '{_tableName}' does not exist in the context.");
            }

            // Get the DBSet instance
            var dbSet = dbSetProperty.GetValue(_context);

            // Get the Any method dynamically
            var anyMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .First(m => m.Name == "Any" && m.GetParameters().Length == 1)
                .MakeGenericMethod(dbSetProperty.PropertyType.GenericTypeArguments[0]);

            bool hasRows = (bool)anyMethod.Invoke(null, new object[] { dbSet });
            return hasRows;
        }

        public async Task truncateTable()
        {
            if (! await tableHasRows()) 
            {
                _context.Database.ExecuteSqlRaw("TRUNCATE TABLE " + _tableName + ";");
                _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('" + _tableName + "', RESEED, 0)");
            }
        }

        // Call this daily...
        public async Task checkTables()
        {
            var tablesNames = _context.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .Distinct()
                .ToList();

            foreach (var table in tablesNames)
            {
                await truncateTable();
            }
        }
    }
}
