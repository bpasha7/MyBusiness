using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SQLite;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace Contacts
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        ApplicationContext db = new ApplicationContext();

        MyGoogleCalendar _calendar = new MyGoogleCalendar();

        #region Clients data
        List<Client> _clients;
        bool _haveNewClient = false;
        AutoCompleteStringCollection collection;
        Client _clientEdit;

        private void CheckCientData()
        {
            if ((_clients == null && !dbLoader.IsBusy) || _haveNewClient)
            {
                this.Text = "Загрузка клиентов...";
                clientBarItem.Checked = false;
                dbLoader.RunWorkerAsync();
            }
        }
        #endregion

        object _lock;
        private bool DoActionWithData(string TableName, string Action, object Item)
        {

            _lock = new object();
            lock (_lock)
            {
                try
                {
                    switch (TableName)
                    {
                        case "Clients":
                            switch (Action)
                            {
                                case "insert":
                                    db.Clients.Add(Item as Client);
                                    _haveNewClient = true;
                                    break;
                                case "update":
                                    // db.Clients.Attach(Item as Client);
                                    var client = Item as Client;
                                    var upd = $"update Clients set LastName = '{client.LastName}', Name = '{client.Name}', Phone = '{client.Phone}', Email = '{client.Email}' where Id = {client.Id}";
                                    var res = db.Database.ExecuteSqlCommand(upd);
                                    if (res != 1)
                                        throw new Exception("Ошибка обновления клиента!");
                                    break;
                                case "delete":
                                    db.Clients.Remove(Item as Client);
                                    break;
                            }
                            break;
                        default: return false;
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();
            newUserControl.Visible = false;

        }
        private void tileBar_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            navigationFrame.SelectedPageIndex = tileBarGroupTables.Items.IndexOf(e.Item);
        }
        /// <summary>
        /// Создать или обновить клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitBtn_Click(object sender, EventArgs e)
        {
            //обновление
            if (_clientEdit != null)
            {
                bool resUpdate = DoActionWithData("Clients", "update", _clientEdit);
                _clientEdit.Birthday = birthdayEdit.DateTime;
                _clientEdit.Name = nameEdit.Text;
                _clientEdit.LastName = lastNameEdit.Text;
                _clientEdit.Email = emailEdit.Text;
                _clientEdit.Phone = phoneEdit.Text;
                _clientEdit.Link = linkEdit.Text;
                if (resUpdate)
                {
                    _haveNewClient = true;
                    CheckCientData();
                    MessageBox.Show($"Клиент {_clientEdit.LastName} {_clientEdit.Name} успешно обновлен!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Клиент {_clientEdit.LastName} {_clientEdit.Name} не обновлен!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                newUserControl.Visible = false;
                clientsGridControl.Visible = true;
                return;
            }
            //создание нового
            var client = new Client
            {
                Birthday = birthdayEdit.DateTime,
                Name = nameEdit.Text,
                LastName = lastNameEdit.Text,
                Email = emailEdit.Text,
                Phone = phoneEdit.Text,
                Link = linkEdit.Text
            };
            client.ImageToBase64(pictureEdit1.Image, System.Drawing.Imaging.ImageFormat.Jpeg);
            bool res = DoActionWithData("Clients", "insert", client);
            if (res)
            {
                _haveNewClient = true;
                CheckCientData();
                MessageBox.Show($"Клиент {client.LastName} {client.Name} успешно добавлен!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Клиент {client.LastName} {client.Name} не добавлен!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            newUserControl.Visible = false;
            clientsGridControl.Visible = true;

        }
        /// <summary>
        /// Поиск клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            _clientEdit = _clients.Where(c => $"{c.LastName} {c.Name}" == textEdit1.Text).First();
            lastNameEdit.Text = _clientEdit.LastName;
            pictureEdit1.Image = _clientEdit.Base64ToImage();
            nameEdit.Text = _clientEdit.Name;
            birthdayEdit.DateTime = _clientEdit.Birthday;
            phoneEdit.Text = _clientEdit.Phone;
            emailEdit.Text = _clientEdit.Email;
            linkEdit.Text = _clientEdit.Link;
            newUserControl.Visible = true;
            clientsGridControl.Visible = false;
        }

        private void tileBarDropDownContainer1_Enter(object sender, EventArgs e)
        {
            textEdit1.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textEdit1.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textEdit1.MaskBox.AutoCompleteCustomSource = collection;
            textEdit1.Text = "";
        }

        private void employeesNavigationPage_Enter(object sender, EventArgs e)
        {
            CheckCientData();
        }

        private void eployeesTileBarItem_ItemPress(object sender, TileItemEventArgs e)
        {
            newUserControl.Visible = false;
            clientsGridControl.Visible = true;
            CheckCientData();
        }

        private void eployeesTileBarItem_ItemDoubleClick(object sender, TileItemEventArgs e)
        {
            newUserControl.Visible = true;
            clientsGridControl.Visible = false;
        }

        private void dbLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _clients = db.Clients.ToList();
                _haveNewClient = false;
            }
            finally
            {

            }
        }

        private void dbLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            clientsGridControl.DataSource = _clients;
            clientBarItem.Checked = true;
            Task.Run(() =>
            {
                collection = new AutoCompleteStringCollection();
                foreach (var c in _clients)
                    collection.Add($"{c.LastName} {c.Name}");
            });
            this.Text = "";
        }

        private void bandedGridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            var focusedRow = bandedGridView1.FocusedRowHandle;
            var client = bandedGridView1.GetRow(focusedRow);
        }

        private void Import()
        {
            using (var reader = new StreamReader("test.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    var names = values[0].Split(' ');
                    var phone = values.Length > 2 ? values[2] : "";
                    var cln = new Client
                    {
                        Name = names[0],
                        LastName = names[1],
                        Link = values[1],
                        Phone = phone
                    };
                    DoActionWithData("Clients", "insert", cln);
                }
            }
        }

        private /*async*/ void newOrderBtn_Click(object sender, EventArgs e)
        {
            //if (!_calendar.Auth())
            //    MessageBox.Show(_calendar.GetStatus());
            //var events = await _calendar.GetEventsAsync(DateTime.Now);
        }

        private void navigationFrame_Click(object sender, EventArgs e)
        {

        }

        private void tileBar_Click(object sender, EventArgs e)
        {

        }

        private void dbNavigationPage_Paint(object sender, PaintEventArgs e)
        {

        }

        private async Task ClearFormCaption()
        {
            await Task.Run(() => Thread.Sleep(2000));
            this.Text = "";
        }

        private async void tileBarItem3_ItemDoubleClick(object sender, TileItemEventArgs e)
        {
            if (!_calendar.Auth())
                MessageBox.Show(_calendar.GetStatus());
            this.Text = _calendar.GetStatus();
            this.Text = "Загрузка событий...";
            var events = await _calendar.GetEventsAsync(DateTime.Now);
            EventsGrid.DataSource = events;
            this.Text = _calendar.GetStatus();
            await ClearFormCaption();
        }

        private void EventsGrid_Resize(object sender, EventArgs e)
        {
            //EventsGrid.Location = new Point(0, 36);
            //EventsGrid.Dock = DockStyle.Bottom;
        }

        private void datesPanel_Resize(object sender, EventArgs e)
        {
            //var padding = datesPanel.Width / 4;
            //layoutControlGroup3.Padding.Left = padding;

        }
    }
}