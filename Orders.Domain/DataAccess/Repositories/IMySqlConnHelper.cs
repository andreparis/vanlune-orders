using System.Data.Common;

namespace Orders.Domain.DataAccess.Repositories
{
    public interface IMySqlConnHelper
    {
        DbConnection MySqlConnection();
    }
}
