using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contacts
{
    public partial class OrderForm : DevExpress.XtraEditors.XtraForm
    {
        ApplicationContext db = new ApplicationContext();
        public DoActionWithData DoAction;
        //private Client _client;
        public OrderForm(List<OrderInfo> orders, string clientName)
        {
            //_client = client;
            InitializeComponent();
            gvOrdersInfo.GroupPanelText = clientName;
           // var orders = db.Orders.Where(o => o.ClientId == _client.Id).ToList();
            gcOrdersInfo.DataSource = orders;
        }

        private void OrderForm_Load(object sender, EventArgs e)
        {

        }

        private void OrderForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void gvOrdersInfo_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            var orderInfo = e.Row as OrderInfo;
            //db = new ApplicationContext();
            var order = db.Orders.FirstOrDefault(o => o.Id == orderInfo.Id);
            if (order != null)
            {
                order.Left = orderInfo.Left;
                order.Payment = orderInfo.Payment;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //var action = price.Id == 0 ? "insert" : "update";
                //if (!DoAction("Orders", "update", order))
                //{
                //    MessageBox.Show($"Услуга не обновлена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}
            }
        }
    }
}
