using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager.Repository.Implementation;

namespace ServiceLayer
{
    public class Services
    {
        private OrdersRepository orderRepository;
        public Services(string connectingPath)
        { 
            orderRepository = new OrdersRepository(connectingPath);
        }

        public IEnumerable<Order> GetInfo(int count)
        {
            return orderRepository.GetOrders(count);
        }
    }
}
