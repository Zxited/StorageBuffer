﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StorageBuffer.Application;
using StorageBuffer.Domain;

namespace StorageBuffer
{
    public partial class OrderWindow : Window
    {
        private Controller control;
        private int orderId;
        private string orderName;
        private string orderDate;
        private string orderDeadline;
        private string customerName;
        private string orderStatus;
        private List<List<string>> orderlines;

        private event NotifyItemChanged notifyItemChanged;

        private bool removeOrder = false;

        public OrderWindow(Controller control, int orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.control = control;
            Setup();
        }

        private void Setup()
        {
            GetOrderInfo();
            orderlines = control.GetOrderlines(orderId);
            GetAllOrderlines();
        }

        private void GetOrderInfo()
        {
            List<string> orderInfo = control.GetOrderInfo(orderId);
            orderName = orderInfo[1];
            orderDate = orderInfo[2];
            orderDeadline = orderInfo[3];
            customerName = orderInfo[4];
            orderStatus = orderInfo[5];

            Title = "Ordre - " + orderId + " : " + orderName;
            lOrderNumber.Content = "Ordrenummer: " + orderId;
            lOrderDate.Content = "Ordredato: " + orderDate;
            lDeadline.Content = "Deadline: " + orderDeadline;
            lCustomerName.Content = "Kunde: " + customerName;
            tbOrderDescription.Text = orderInfo[6];

            switch (orderStatus)
            {
                case "Received":
                    cbOrderChoice.SelectedItem = cbOrderChoice.Items[0];
                    break;

                case "InProgress":
                    cbOrderChoice.SelectedItem = cbOrderChoice.Items[1];
                    break;

                case "Done":
                    cbOrderChoice.SelectedItem = cbOrderChoice.Items[2];
                    break;

                case "Shipped":
                    cbOrderChoice.SelectedItem = cbOrderChoice.Items[3];
                    break;

                case "Billed":
                    cbOrderChoice.SelectedItem = cbOrderChoice.Items[4];
                    break;

                case "Paid":
                    cbOrderChoice.SelectedItem = cbOrderChoice.Items[5];
                    break;
                case "Canceled":
                    cbOrderChoice.SelectedItem = cbOrderChoice.Items[6];
                    break;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            MaterialChooseWindow chooseWindow = new MaterialChooseWindow(control);

            chooseWindow.Owner = this;
            chooseWindow.Top = this.Top;
            chooseWindow.Left = this.Left + 8;
            chooseWindow.ShowDialog();

            if (chooseWindow.DialogResult.Value)
            {
                bool materialAlreadyAdded = false;
                foreach (List<string> orderline in orderlines)
                {
                    if (orderline[0] == chooseWindow.MaterialId.ToString())
                    {
                        materialAlreadyAdded = true;
                        break;
                    }
                }

                if (!materialAlreadyAdded)
                {
                    List<string> orderlineNew = new List<string>() { chooseWindow.MaterialId.ToString(), chooseWindow.MaterialName, "0" };
                    orderlines.Add(orderlineNew);
                    lvResult.Items.Add(new { MaterialId = orderlineNew[0], MaterialName = orderlineNew[1], Quantity = orderlineNew[2] } );
                }
            }
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var listViewItem = (ListViewItem)sender;
            var orderline = listViewItem.Content;

            PropertyInfo[] props = orderline.GetType().GetProperties();

            int materialId = int.Parse(props[0].GetValue(orderline, null).ToString());

            List<string> material = control.GetMaterial(materialId);

            ChangeOrderline changeOrderline = new ChangeOrderline(orderline, material[3]);
            changeOrderline.Owner = this;
            changeOrderline.Top = this.Top;
            changeOrderline.Left = this.Left + 8;
            changeOrderline.ShowDialog();

            if (changeOrderline.delete)
            {
                orderlines.Remove(orderlines.Find(x => x[0] == materialId.ToString()));
            }
            else
            {
                orderlines.Find(x => x[0] == materialId.ToString())[2] = changeOrderline.quantity.ToString();
            }

            GetAllOrderlines();
        }

        private void GetAllOrderlines()
        {
            lvResult.Items.Clear();
            foreach (List<string> item in orderlines)
            {
                lvResult.Items.Add(new { MaterialId = item[0], MaterialName = item[1], Quantity = item[2] });
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveOrder();
        }

        private void SaveOrder()
        {
            string messsage = "";
            if (control.UpdateOrder(orderId, orderStatus, orderlines, tbOrderDescription.Text))
            {
                messsage = "Ordren blev gemt.";
            }
            else
            {
                messsage = "Ordren kunne IKKE gemmes.";
            }

            MessageWindow messageWindow = new MessageWindow(messsage);
            messageWindow.Owner = this;
            messageWindow.Top = this.Top;
            messageWindow.Left = this.Left + 8;
            messageWindow.ShowDialog();
        }

        private void CbOrderChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbOrderChoice.SelectedIndex)
            {
                case 0:
                    orderStatus = "Received";
                    break;

                case 1:
                    orderStatus = "InProgress";
                    break;

                case 2:
                    orderStatus = "Done";
                    break;

                case 3:
                    orderStatus = "Shipped";
                    break;

                case 4:
                    orderStatus = "Billed";
                    break;

                case 5:
                    orderStatus = "Paid";
                    break;

                case 6:
                    orderStatus = "Canceled";
                    break;
            }
        }

        private void OrderWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!removeOrder)
            {
                ConfirmationWindow confirmationWindow = new ConfirmationWindow("Vil du gemme ændringerne?");
                confirmationWindow.Owner = this;
                confirmationWindow.Top = this.Top;
                confirmationWindow.Left = this.Left + 8;
                confirmationWindow.ShowDialog();

                if ((bool)confirmationWindow.DialogResult)
                {
                    SaveOrder();
                } 
            }

            Notify();
        }

        private void BtnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationWindow confirmationWindow = new ConfirmationWindow("Er du sikker på du vil SLETTE ordren?");
            confirmationWindow.Owner = this;
            confirmationWindow.Top = this.Top;
            confirmationWindow.Left = this.Left + 8;
            confirmationWindow.ShowDialog();

            if ((bool)confirmationWindow.DialogResult)
            {
                RemoveOrder();
            }
        }

        private void RemoveOrder()
        {
            string messsage = "";
            bool removed = false;
            if (control.RemoveOrder(orderId))
            {
                messsage = "Ordren blev slettet.";
                removed = true;
            }
            else
            {
                messsage = "Ordren kunne IKKE slettes.";
            }

            MessageWindow messageWindow = new MessageWindow(messsage);
            messageWindow.Owner = this;
            messageWindow.Top = this.Top;
            messageWindow.Left = this.Left + 8;
            messageWindow.ShowDialog();

            if (removed)
            {
                removeOrder = true;
                this.Close();
            }
        }

        private void Notify()
        {
            if (notifyItemChanged != null)
            {
                notifyItemChanged();
            }
        }

        public void AddObserver(NotifyItemChanged listener)
        {
            notifyItemChanged += listener;
        }

        public void RemoveObserver(NotifyItemChanged listener)
        {
            notifyItemChanged -= listener;
        }
    }
}
