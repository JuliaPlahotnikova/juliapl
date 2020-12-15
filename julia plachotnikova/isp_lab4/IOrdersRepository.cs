using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataManager.Interfaces
{
    public interface IOrdersRepository
    {
        IEnumerable<Order> GetOrders(int count);
    }
}
