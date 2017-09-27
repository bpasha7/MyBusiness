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
    public delegate bool DoActionWithData(string TableName, string Action, DbEntitie Item);
    public delegate int GetLastId();

    public partial class ServiceForm : DevExpress.XtraEditors.XtraForm
    {

        public DoActionWithData DoAction;
        public GetLastId GetId;
        BindingList<Price> _prices = null;
        public List<Price> GetEditedPrices()
        {
            return _prices.ToList();
        }
        public ServiceForm(List<Price> prices)
        {
            InitializeComponent();
            _prices = new BindingList<Price>();
            foreach (var item in prices)
            {
                _prices.Add(item);
            }
            _prices.AllowNew = true;
            _prices.AllowEdit = true;
            gcServices.DataSource = _prices;
        }

        private void ServiceForm_Load(object sender, EventArgs e)
        {

        }

        private void ServiceForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void gvServices_RowDeleting(object sender, DevExpress.Data.RowDeletingEventArgs e)
        {
            if (MessageBox.Show($"Удалить услугу?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            var price = e.Row as Price;
            if (!DoAction("Prices", "delete", price))
                MessageBox.Show($"Услуга не удалена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void gvServices_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            var price = e.Row as Price;
            var action = price.Id == 0 ? "insert" : "update";
            if (!DoAction("Prices", action, price))
            {
                MessageBox.Show($"Услуга не обновлена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            price.Id = GetId();
        }

        private void gvServices_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {

        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gvServices.DeleteSelectedRows();
        }
    }
}
