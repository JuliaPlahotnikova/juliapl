using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager.Repository.Implementation;
using Models;

namespace ServiceLayer
{
    public class ErrorService
    {
        private OrdersRepository Database { get; set; }

        public ErrorService(OrdersRepository unitOfWork)
        {
            Database = unitOfWork;
        }

        public void AddErrors(Errors error)
        {
            Database.Errors.Create(error);
        }
    }
}
