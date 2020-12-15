using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager.Interfaces;
using Models;
using DataManager.Extensions;

namespace DataManager.Repository.Implementation
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly string _connection;
        private SqlConnection _sqlConnection;

        public OrdersRepository(string connection)
        {
            _connection = connection ?? throw new ArgumentOutOfRangeException($"{nameof(connection)} can not type for");
            _sqlConnection = new SqlConnection(_connection);
        }

        public IEnumerable<Order> GetOrders(int count)
        {
            try
            {
                _sqlConnection.Open();

                var command = new SqlCommand();

                command.CommandText = (@"[dbo].[GetOrders]");
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Count", count);

                var entity = command.GetOrders<Order>();

                _sqlConnection.Close();
                return entity;

            }
            catch
            {
                _sqlConnection.Close();
            }
            finally
            {
                _sqlConnection.Close();
            }
            return null;
        }
    }
}
