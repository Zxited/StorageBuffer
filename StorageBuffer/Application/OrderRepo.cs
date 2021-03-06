﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageBuffer.Application;
using StorageBuffer.Domain;

namespace StorageBuffer.Model
{
    public sealed class OrderRepo
    {
        private static OrderRepo instance = null;
        private static readonly object padlock = new object();

        public static OrderRepo Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            throw new Exception("Object not created!");
                        }
                    }
                }
                return instance;
            }
        }

        public static void CreateInstance(IPersistable databaseRepo)
        {
            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new OrderRepo(databaseRepo);
                    }
                }
            }
        }

        private IPersistable databaseRepo;

        public List<Order> orders;

        OrderRepo(IPersistable databaseRepo)
        {
            this.databaseRepo = databaseRepo;
            PullAllOrders();
        }

        public void PullAllOrders()
        {
            orders = databaseRepo.GetAllOrders();
        }

        public List<List<string>> GetOrders(string searchQuery)
        {
            List<List<string>> result = new List<List<string>>();
            foreach (Order order in orders)
            {
                if (order.Name.ToLower().Contains(searchQuery.ToLower()) || 
                    order.CustomerName.ToLower().Contains(searchQuery.ToLower()) || 
                    CustomerRepo.Instance.GetCustomerPhone(order.CustomerId).Contains(searchQuery))
                {
                    result.Add(order.ToList());
                }
            }

            return result;
        }

        public void RegisterUsedMaterial(int orderId, int materialId, int amount)
        {
            Material material = MaterialRepo.Instance.GetMaterialObj(materialId);
            orders.Find(x => x.Id == orderId).RegisterUsedMaterial(material, amount);
        }

        public void ChangeStatusOfOrder(int orderId, Status status)
        {
            orders.Find(x => x.Id == orderId).OrderStatus = status;
        }

        public bool CreateOrder(int customerId, string orderName, string deadline, string description)
        {
            string date = DateTime.Now.ToShortDateString();
            int id = databaseRepo.CreateOrder(customerId, orderName, date, deadline, description);
            
            if (id == 0)
            {
                return false;
            }

            string customerName = CustomerRepo.Instance.GetCustomerName(customerId);
            Order order = new Order(id, customerId, customerName, Status.Received, orderName, date, deadline, description);
            orders.Add(order);
            return true;
            
        }

        public bool UpdateOrder(int orderId, string orderStatus, List<List<string>> orderlines, string description)
        {
            Order orderResult = orders.Find(x => x.Id == orderId);

            Status.TryParse(orderStatus, out Status status);
            orderResult.OrderStatus = status;
            orderResult.Description = description;

            orderResult.orderlines = new List<Orderline>();
            if(!databaseRepo.RemoveOrderlines(orderResult.Id)) return false;
            if(!databaseRepo.UpdateOrder(orderResult.Id, orderResult.OrderStatus.ToString(), orderResult.Description)) return false;

            foreach (List<string> orderline in orderlines)
            {
                RegisterUsedMaterial(orderResult.Id, int.Parse(orderline[0]), int.Parse(orderline[2]));
                int.TryParse(orderline[0], out int materialId);
                int.TryParse(orderline[2], out int quantity);
                if(!databaseRepo.InsertOrderline(orderResult.Id, materialId, quantity)) return false;
            }

            return true;
        }

        public List<string> GetOrderInfo(int orderId)
        {
            return orders.Find(x => x.Id == orderId).ToLongList();
        }

        public List<List<string>> GetOrderlines(int orderId)
        {
            return orders.Find(x => x.Id == orderId).GetOrderlines();
        }

        public bool RemoveOrder(int orderId)
        {
            Order order = orders.Find(x => x.Id == orderId);

            if (order == null)
            {
                return false;
            }

            if (!databaseRepo.RemoveOrderlines(order.Id)) return false;
            if (!databaseRepo.RemoveOrder(orderId)) return false;
            if (!orders.Remove(order)) return false;
            return true;
        }

        public bool OrderExists(int orderId)
        {
            return orders.Exists(x => x.Id == orderId);
        }
    }
}
