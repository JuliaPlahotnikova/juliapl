using Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Repository.Implementation
{
        class ErrorsRepository
        {
            private string _connectionString;
            public ErrorsRepository(string connectionString)
            {
                _connectionString = connectionString;
            }
            public void Create(Errors item)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("AddMessage", connection)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };

                    SqlParameter excepParam = new SqlParameter
                    {
                        ParameterName = "@exception",
                        Value = item.Exception
                    };
                    command.Parameters.Add(excepParam);

                    SqlParameter messParam = new SqlParameter
                    {
                        ParameterName = "@message",
                        Value = item.Message
                    };
                    command.Parameters.Add(messParam);

                    SqlParameter dateParam = new SqlParameter
                    {
                        ParameterName = "@time",
                        Value = item.Time
                    };
                    command.Parameters.Add(dateParam);

                    var result = command.ExecuteScalar();
                }
            }


            public void Delete(DateTime birthDay)
            {
                throw new NotImplementedException();
            }

            public Errors Get(int id)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Errors> GetAll()
            {
                throw new NotImplementedException();
            }

            public void Update(Errors item)
            {
                throw new NotImplementedException();
            }
        }
}
