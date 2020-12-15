using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Extensions
{
    public static class SqlCommandExtensions
    {

        public static IEnumerable<TEntity> GetOrders<TEntity>(this SqlCommand command) where TEntity : new()
        {
            var result = GetOrdersInternal<TEntity>(command);
            return result;
        }

        private static IEnumerable<TEntity> GetOrdersInternal<TEntity>(SqlCommand command) where TEntity : new()
        {
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                reader.Close();
                return Enumerable.Empty<TEntity>();
            }

            var entities = reader.ParseFromReaderInternal<TEntity>();

            reader.Close();

            return entities;
        }
        private static IEnumerable<TEntity> ParseFromReaderInternal<TEntity>(this SqlDataReader reader) where TEntity : new()
        {
            var entityType = typeof(TEntity);
            var entityProps = entityType.GetProperties();

            var entities = new List<TEntity>();

            while (reader.Read())
            {
                var entity = new TEntity();

                foreach (var entityPropsInfo in entityProps)
                {
                    var valueFromReader = reader[entityPropsInfo.Name];

                    if(valueFromReader is DBNull)
                    {
                        valueFromReader = null;
                    }
                    entityPropsInfo.SetValue(entity, valueFromReader);
                }
                entities.Add(entity);
            }
            return entities;
        }
    }

}
