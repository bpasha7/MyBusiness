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
        public ServiceForm(List<Price> prices)
        {
            InitializeComponent();
            pricesView.Items.Clear();
            foreach (var item in prices)
            {
                var cell = new ListViewItem(new string[] { $"{item.Id}", item.Short, item.Name });
                pricesView.Items.Add(cell);
            }
        }

        private void ServiceForm_Load(object sender, EventArgs e)
        {

        }
    }
}
