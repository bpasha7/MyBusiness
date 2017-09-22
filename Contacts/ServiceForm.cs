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
    public partial class ServiceForm : DevExpress.XtraEditors.XtraForm
    {
        public delegate

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

        }

        private void gvServices_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {

        }
    }
}
