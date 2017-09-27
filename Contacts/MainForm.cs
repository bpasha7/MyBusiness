﻿using System;
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
        /// <summary>
        /// Лог
        /// </summary>
        private static NLog.Logger _log;
        #region Clients data
        List<Client> _clients;
        List<Price> _prices;
        List<MyEvent> _events;
        int _lastInsertedId = 0;
        bool _haveNewClient = false;
        AutoCompleteStringCollection collection;
        Client _clientEdit;

        public int GetLastId()
        {
            return _lastInsertedId;
        }

        ServiceForm _sf = null;

        private void CheckClientData()
        {
            if ((_clients == null && !dbLoader.IsBusy) || _haveNewClient)
            {
                this.Text = "Загрузка клиентов...";
                clientBarItem.Checked = false;
                dbLoader.DoWork += (obj, e) => LoadClients();
                dbLoader.RunWorkerCompleted += dbLoader_RunWorkerCompleted;
                dbLoader.RunWorkerAsync();
            }
        }

        private void CheckPriceData()
        {
            if ((!dbLoader.IsBusy))
            {
                this.Text = "Загрузка списка услуг...";
                EventsBarItem.Checked = false;
                //dbLoader.DoWork  null;
                var bg_worker = new BackgroundWorker();
                bg_worker.DoWork += (obj, e) => LoadPrices();
                bg_worker.RunWorkerCompleted += dbLoader_Prices_RunWorkerCompleted;
                bg_worker.RunWorkerAsync();
            }
        }

        private void LoadPrices()
        {
            try
            {
                _prices = db.Prices.ToList();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }


        private void LoadClients()
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

        #endregion

        object _lock;
        private bool DoActionWithData(string TableName, string Action, DbEntitie Item)
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
                                    /* var client = Item as Client;
                                     var upd = $"update Clients set LastName = '{client.LastName}', Name = '{client.Name}', Phone = '{client.Phone}', Email = '{client.Email}' where Id = {client.Id}";
                                     var res = db.Database.ExecuteSqlCommand(upd);
                                     if (res != 1)
                                         throw new Exception("Ошибка обновления клиента!");*/
                                    db.Entry(Item as Client).State = System.Data.Entity.EntityState.Modified;
                                    break;
                                case "delete":
                                    db.Clients.Remove(Item as Client);
                                    break;
                            }
                            break;
                        case "Prices":
                            switch (Action)
                            {
                                case "insert":
                                    db.Prices.Add(Item as Price);
                                    break;
                                case "update":
                                    db.Entry(Item as Price).State = System.Data.Entity.EntityState.Modified;
                                    break;
                                case "delete":
                                    db.Prices.Remove(Item as Price);
                                    break;
                            }
                            break;
                        case "Orders":
                            switch (Action)
                            {
                                case "insert":
                                    db.Orders.Add(Item as Order);
                                    break;
                                case "update":
                                    db.Entry(Item as Order).State = System.Data.Entity.EntityState.Modified;
                                    break;
                                case "delete":
                                    db.Orders.Remove(Item as Order);
                                    break;
                            }
                            break;
                        default: return false;
                    }
                    db.SaveChanges();
                    _lastInsertedId = Item.Id;
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
            _log = NLog.LogManager.GetLogger("MainForm");
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
                    CheckClientData();
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
                CheckClientData();
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
            CheckClientData();
        }

        private void eployeesTileBarItem_ItemPress(object sender, TileItemEventArgs e)
        {
            newUserControl.Visible = false;
            clientsGridControl.Visible = true;
            CheckClientData();
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

        private async void dbLoader_Prices_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EventsBarItem.Checked = true;
            this.Text = "";
            dictionaryListBtn.Enabled = true;
            await ClearFormCaption();
            EventsBarItem.Checked = false;
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
            if (_prices == null)
                CheckPriceData();
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
            EventsBarItem.Checked = false;
            pricesBarItem.Checked = false;
            clientBarItem.Checked = false;
        }

        private async void tileBarItem3_ItemDoubleClick(object sender, TileItemEventArgs e)
        {
            CheckPriceData();
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

        private void endDateEdit_EditValueChanged(object sender, EventArgs e)
        {

        }

        private async void uploadEventsBtn_Click(object sender, EventArgs e)
        {
            if (!_calendar.Auth())
                MessageBox.Show(_calendar.GetStatus());
            this.Text = _calendar.GetStatus();
            this.Text = "Загрузка событий...";
            _events = await _calendar.GetEventsAsync(startDateEdit.DateTime, endDateEdit.DateTime.AddDays(1), _prices);
            EventsGrid.DataSource = _events;
            this.Text = _calendar.GetStatus();
            gridView1.GroupPanelText = $"Записи с {startDateEdit.DateTime.ToShortDateString()} по {endDateEdit.DateTime.ToShortDateString()}";
            gridView1.SelectAll();
            await ClearFormCaption();
        }

        private async void importEventsBtn_Click(object sender, EventArgs e)
        {
            var clientsAdded = 0;
            var clientsFails = 0;
            var ordersFails = 0;
            var ordersAdded = 0;
            foreach (var index in gridView1.GetSelectedRows())
            {
                var item = gridView1.GetRow(index) as MyEvent;
                var foundedClients = _clients.Where(c => $"{c.LastName} {c.Name}".ToLower() == item.Name.ToLower());
                Client client = null;
                if (foundedClients.Count() == 1)
                {
                    client = foundedClients.First();

                }
                else if (foundedClients.Count() == 0)
                {
                    var clientName = item.Name.Split(' ');
                    client = new Client
                    {
                        LastName = clientName[0],
                        Name = clientName[1],
                        Phone = item.Phone
                    };
                    if (!DoActionWithData("Clients", "insert", client))
                        clientsFails++;
                    client.Id = _lastInsertedId;
                    clientsAdded++;
                }
                else
                {
                    continue;
                }
                var order = new Order
                {
                    ClientId = client.Id,
                    Date = item.Date,
                    Payment = item.Payment,
                    PriceId = (int)_prices?.Where(p => p.Name == item.Items || p.Short == item.Items).FirstOrDefault().Id,
                    CalendarId = item.Id
                };
                if (!DoActionWithData("Orders", "insert", order))
                    ordersFails++;
                ordersAdded++;
            }
            if(clientsFails != 0 || ordersFails != 0)
                MessageBox.Show($"Ошибки: добавления клиентов {clientsFails}; занесения услуг {ordersFails}.");
            this.Text = $"Новых клиентов {clientsAdded}. Занесено в базу услуг {ordersAdded}.";
            await ClearFormCaption();
            //var f = new ImportForm();
            //f.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckClientData();
            DateOrdersEnd.DateTime = DateTime.Now;
            dateOrdersStart.DateTime = DateTime.Now.AddDays( -DateTime.Now.Day + 1);
            startDateEdit.DateTime = DateTime.Now;
            endDateEdit.DateTime = DateTime.Now.AddDays(1);
        }

        private void dictionaryListBtn_Click(object sender, EventArgs e)
        {
            _sf = new ServiceForm(_prices);
            _sf.DoAction += this.DoActionWithData;
            _sf.GetId += this.GetLastId;
            _sf.ShowDialog();
            _prices = _sf.GetEditedPrices();
        }

        private void tileBarItem3_ItemClick(object sender, TileItemEventArgs e)
        {
            //CheckPriceData();
        }

        private void showOrdersInfoBtn_Click(object sender, EventArgs e)
        {
            var ordersInfo = new List<OrderInfo>();
            foreach (var order in db.Orders.Where(o => o.Date >= dateOrdersStart.DateTime && o.Date <= DateOrdersEnd.DateTime && o.Left == false))
            {
                ordersInfo.Add(new OrderInfo
                {
                    ClientName = _clients?.Where(c => c.Id == order.ClientId).SingleOrDefault()?.FullName,
                    Payment = order.Payment,
                    Date = order.Date,
                    PriceName = _prices?.Where(p => p.Id == order.PriceId).FirstOrDefault()?.Name
                });
            }
            gcOrdersInfo.DataSource = ordersInfo;
            gvOrdersInfo.GroupPanelText = $"Поступления с {dateOrdersStart.DateTime.ToShortDateString()} по {DateOrdersEnd.DateTime.ToShortDateString()}";

        }
    }
}