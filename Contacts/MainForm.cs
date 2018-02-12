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
using System.Diagnostics;
using System.Net;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

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

        BindingList<Material> _materials = null;

        #region Clients data
        List<Client> _clients;
        List<Price> _prices;
        List<MyEvent> _events;
        int _lastInsertedId = 0;
        bool _haveNewClient = false;
        //AutoCompleteStringCollection collection;
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
                notify.ShowBalloonTip(750, "Ошибка", "Не найден файл БД.", ToolTipIcon.Error);

                _log.Error($"{ex}");
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
            catch (Exception ex)
            {
                notify.ShowBalloonTip(750, "Ошибка", "Не найден файл БД.", ToolTipIcon.Error);

                _log.Error($"{ex}");
            }
            finally
            {

            }
        }

        #endregion



        object _lock;

        public DateTime GetNistTime()
        {
            DateTime dateTime = DateTime.MinValue;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://www.microsoft.com");
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string todaysDates = response.Headers["date"];

                dateTime = DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal);
            }

            return dateTime;
        }


        bool Check()
        {
            try
            {
                string[] files = System.IO.Directory.GetFiles(Directory.GetCurrentDirectory(), "*.lic");
                if (files.Count() != 1)
                    throw new Exception("Обнаружено несколько файлов лицензий!");
                string line = "";
                using (StreamReader sr = new StreamReader(files[0]))
                {
                    line = sr.ReadToEnd();
                }
                using (MD5 md5Hash = MD5.Create())
                {
                    string hash = GetMd5Hash(md5Hash, line);
                    var fi = new FileInfo(files[0]);
                    if (fi.Name.Replace(".lic","") !=  hash)
                    {
                        throw new Exception("Лицензия нарушена!");
                    }
                }
                line = Base64Decode(line);
                line = Base64Decode(line);
                var param = line.Split(new char[] { '|' });
                var machine = param[0];
                var computerName = System.Environment.MachineName;
                if(machine != computerName)
                {
                    throw new Exception("Лицензия не подходит на данный компьютер!");
                }
                var User = param[1];
                var dtNow = GetNistTime();
                var dt = new DateTime(Convert.ToInt64(param[2]));
                var days = (dtNow - dt).Days;
                if (days < 0)
                    notify.ShowBalloonTip(750, "Лицензия", $"{User}, осталось дней: {Math.Abs(days)}.", ToolTipIcon.Info);
                return true;
            }
            catch(Exception ex)
            {
                notify.ShowBalloonTip(750, "Внимание", $"{ex.Message}", ToolTipIcon.Warning);
                this.Enabled = false;
                _log.Error($"{ex}");
                return false;
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private bool DoActionWithData(string TableName, string Action, DbEntitie Item)
        {

            _lock = new object();
            lock (_lock)
            {
                try
                {
                    db = new ApplicationContext();
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
                        case "Materials":
                            switch (Action)
                            {
                                case "insert":
                                    db.Materials.Add(Item as Material);
                                    break;
                                case "update":
                                    db.Entry(Item as Material).State = System.Data.Entity.EntityState.Modified;
                                    break;
                                case "delete":
                                    db.Materials.Remove(Item as Material);
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
                    _log.Error($"Ошибка обращения к БД. MESSAGE: {ex}");
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
                _clientEdit.Birthday = birthdayEdit.DateTime;
                _clientEdit.Name = nameEdit.Text;
                _clientEdit.LastName = lastNameEdit.Text;
                _clientEdit.Email = emailEdit.Text;
                _clientEdit.Phone = phoneEdit.Text;
                _clientEdit.Link = linkEdit.Text;
                _clientEdit.ImageToBase64(pictureEdit1.Image, System.Drawing.Imaging.ImageFormat.Jpeg);
                bool resUpdate = DoActionWithData("Clients", "update", _clientEdit);
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
                birthdayEdit.DateTime = DateTime.Now;
                nameEdit.Text = "";
                lastNameEdit.Text = "";
                emailEdit.Text = "";
                phoneEdit.Text = "";
                linkEdit.Text = "";
                pictureEdit1.Image = null;
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
            //_clientEdit = _clients.Where(c => $"{c.LastName} {c.Name}" == textEdit1.Text).First();
            //lastNameEdit.Text = _clientEdit.LastName;
            //pictureEdit1.Image = _clientEdit.Base64ToImage();
            //nameEdit.Text = _clientEdit.Name;
            //birthdayEdit.DateTime = _clientEdit.Birthday;
            //phoneEdit.Text = _clientEdit.Phone;
            //emailEdit.Text = _clientEdit.Email;
            //linkEdit.Text = _clientEdit.Link;
            //newUserControl.Visible = true;
            //clientsGridControl.Visible = false;
        }

        private void tileBarDropDownContainer1_Enter(object sender, EventArgs e)
        {
            //textEdit1.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //textEdit1.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //textEdit1.MaskBox.AutoCompleteCustomSource = collection;
            //textEdit1.Text = "";
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
            catch(Exception ex)
            {

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
            //Task.Run(() =>
            //{
            //    collection = new AutoCompleteStringCollection();
            //    foreach (var c in _clients)
            //        collection.Add($"{c.LastName} {c.Name}");
            //});
            this.Text = "";
            notify.ShowBalloonTip(750, "Клиенты загружены", "Список клиентов загружен в программу", ToolTipIcon.Info);
            if (_prices == null)
                CheckPriceData();
        }

        private void bandedGridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            var focusedRow = bandedGridView1.FocusedRowHandle;
            var client = bandedGridView1.GetRow(focusedRow) as Client;
            _clientEdit = _clients.Where(c => $"{c.LastName} {c.Name}" == client.FullName).First();
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
            {
                notify.ShowBalloonTip(750, "Ошибка", $"Ошибка авторизации в календаре", ToolTipIcon.Error);
               //MessageBox.Show(_calendar.GetStatus());
                return;
            }
            this.Text = _calendar.GetStatus();
            this.Text = "Загрузка событий...";
            _events = await _calendar.GetEventsAsync(startDateEdit.DateTime, endDateEdit.DateTime.AddDays(1), _prices);
            EventsGrid.DataSource = _events;
            //this.Text = _calendar.GetStatus();
            notify.ShowBalloonTip(150, "Импорт данных", _calendar.GetStatus(), ToolTipIcon.None);
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
                try
                {
                    var item = gridView1.GetRow(index) as MyEvent;
                    //_clients = db.Clients.w
                    var foundedClients = db.Clients.Where(c => (c.LastName + " " + c.Name).ToLower() == item.Name.ToLower()).ToList();
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
                        {
                            clientsFails++;
                            continue;
                        }
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
                    {
                        ordersFails++;
                        continue;
                    }
                    ordersAdded++;
                }
                catch(Exception ex)
                {
                    _log.Error($"Ошибка занесения в базу события из календаря. MESSAGE: {ex}");
                }
            }
            //if(clientsFails != 0 || ordersFails != 0)
            //    MessageBox.Show($"Ошибки: добавления клиентов {clientsFails}; занесения услуг {ordersFails}.");
            this.Text = $"Новых клиентов {clientsAdded}. Занесено в базу услуг {ordersAdded}.";
            notify.ShowBalloonTip(750, "Импорт данных", $"Выполнен иморт данных из календаря. Новых клиентов {clientsAdded}. Занесено в базу услуг {ordersAdded}.", ToolTipIcon.Info);
            _clients = db.Clients.ToList();
            await ClearFormCaption();
            //var f = new ImportForm();
            //f.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Check())
                return;
            CheckClientData();
            DateOrdersEnd.DateTime = DateTime.Now;
            dateOrdersStart.DateTime = DateTime.Now.AddDays( -DateTime.Now.Day + 1);
            repDateEnd.DateTime = DateTime.Now;
            repDateStart.DateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
            dateMaterialEnd.DateTime = DateTime.Now;
            dateMaterialStart.DateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
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
            try
            {
                var ordersInfo = new List<OrderInfo>();
                db = new ApplicationContext();
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
            catch (Exception ex)
            {
                _log.Error($"{ex}");
            }
        }

        private void открытьЛогToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("notepad.exe", "errors.log");
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка открытия лог файла. MESSAGE: {ex}");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notify.Dispose();
        }

        private void clientsGridControl_DoubleClick(object sender, EventArgs e)
        {
            //DataViewBase view = gridControl.View;
            
            //object row = gridControl.GetRow(gridControl.GetSelectedRowHandles()[0]);
            //_clientEdit = _clients.Where(c => $"{c.LastName} {c.Name}" == textEdit1.Text).First();
            //lastNameEdit.Text = _clientEdit.LastName;
            //pictureEdit1.Image = _clientEdit.Base64ToImage();
            //nameEdit.Text = _clientEdit.Name;
            //birthdayEdit.DateTime = _clientEdit.Birthday;
            //phoneEdit.Text = _clientEdit.Phone;
            //emailEdit.Text = _clientEdit.Email;
            //linkEdit.Text = _clientEdit.Link;
            //newUserControl.Visible = true;
            //clientsGridControl.Visible = false;
        }

        private void historyBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_clientEdit != null)
                {
                    //var orders = db.Orders.Where(o => o.ClientId == _clientEdit.Id).ToList();
                    List<OrderInfo> orders = new List<OrderInfo>();
                    db = new ApplicationContext();
                    foreach (var order in db.Orders.Where(o => o.ClientId == _clientEdit.Id))
                    {
                        orders.Add(new OrderInfo
                        {
                            Id = order.Id,
                            ClientName = "",// _clients?.Where(c => c.Id == order.ClientId).SingleOrDefault()?.FullName,
                            Payment = order.Payment,
                            Date = order.Date,
                            PriceName = _prices?.Where(p => p.Id == order.PriceId).FirstOrDefault()?.Name,
                            Left = order.Left
                        });
                    }
                    if (orders.Count == 0)
                    {
                        MessageBox.Show($"Клиент еще не записывался", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    var clientOrders = new OrderForm(orders, _clientEdit.FullName);
                    clientOrders.DoAction = DoActionWithData;
                    clientOrders.Show();
                }
            }
            catch(Exception ex)
            {
                notify.ShowBalloonTip(750, "Ошибка", $"{ex.Message}", ToolTipIcon.Error);

                _log.Error($"{ex}");
            }

        }

        private void materialBtnShow_Click(object sender, EventArgs e)
        {
            db = new ApplicationContext();
            var materials = db.Materials.Where(o => o.Date >= dateMaterialStart.DateTime && o.Date <= dateMaterialEnd.DateTime).ToList();
            _materials = new BindingList<Material>(materials);
            _materials.AllowNew = true;
            _materials.AllowEdit = true;
            gcMaterial.DataSource = _materials;
        }

        private void gvMaterial_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            var material = e.Row as Material;
            var action = material.Id == 0 ? "insert" : "update";
            if (!DoActionWithData("Materials", action, material))
            {
                MessageBox.Show($"Материал не обновлен!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            material.Id = _lastInsertedId;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            birthdayEdit.DateTime = DateTime.Now;
            nameEdit.Text = "";
            lastNameEdit.Text = "";
            emailEdit.Text = "";
            phoneEdit.Text = "";
            linkEdit.Text = "";
            pictureEdit1.Image = null;
            newUserControl.Visible = false;
            clientsGridControl.Visible = true;
        }

        private void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            if (_materials == null)
                materialBtnShow_Click(sender, e);
                //_materials = new BindingList<Material>(new List<Material>());
        }

        private void repShowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                db = new ApplicationContext();
                var report = new List<ReportByMonth>();
                var dt = repDateStart.DateTime;
               // var months = (repDateEnd.DateTime - repDateStart.DateTime)
                //for (int i = 0; i < repDateEnd.DateTime.Month; i++)
                while(dt <= repDateEnd.DateTime)
                {
                    var orders = db.Orders.Where(o => o.Date.Month == dt.Month && o.Date.Year == dt.Year);
                    var ordersSum = orders.Count() > 0 ? orders.Sum(s => s.Payment) : 0;
                    var materials = db.Materials.Where(o => o.Date.Month == dt.Month && o.Date.Year == dt.Year);
                    var materialsSum = materials.Count() > 0 ? materials.Sum(s => s.Payment) : 0;
                    report.Add(new ReportByMonth { Date = dt, MaterialsSum = materialsSum, OrdersSum = ordersSum });
                    dt = dt.AddMonths(1);
                }
                gcRep.DataSource = report;
            }
            catch(Exception ex)
            {
                notify.ShowBalloonTip(750, "Ошибка", $"{ex.Message}", ToolTipIcon.Error);

                _log.Error($"{ex}");
            }
        }
    }
}