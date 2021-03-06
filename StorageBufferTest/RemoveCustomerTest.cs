﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageBuffer.Application;
using StorageBuffer.Domain;

namespace StorageBufferTest
{
    [TestClass]
    public class RemoveCustomerTest
    {
        private Controller control;
        private List<Customer> customers;
        private List<Material> materials;
        private List<Order> orders;

        [TestInitialize]
        public void SetupTest()
        {
            control = Controller.Instance;
            control.GetAllData(DatabaseRepoFakey.Instance);

            customers = control.customerRepo.customers;
            materials = control.materialRepo.materials;
            orders = control.orderRepo.orders;
        }

        [TestCleanup]
        public void CleanUpTest()
        {
            control.PullAllData();
        }

        [TestMethod]
        public void RemoveCustomerReturnsTrueOnSuccess()
        {
            bool succeeded = control.RemoveCustomer(2);

            Assert.IsTrue(succeeded);
        }

        [TestMethod]
        public void RemoveCustomer_CustomerRemoved()
        {
            control.RemoveCustomer(1);

            Assert.IsFalse(control.CustomerExist(1));
        }

        [TestMethod]
        public void RemoveCustomer_CustomerDosentExists()
        {
            bool succeeded = control.RemoveCustomer(14);

            Assert.IsFalse(succeeded);
        }
    }
}
