using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace Contacts
{
    public class MyGoogleCalendar
    {
        /// <summary>
        /// Сервис календаря
        /// </summary>
        private CalendarService _calendar;
        /// <summary>
        /// Лог
        /// </summary>
        private static NLog.Logger _log;
        /// <summary>
        /// Скоп
        /// </summary>
        private static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        /// <summary>
        /// Статус последнего действия
        /// </summary>
        private string _status = "";
        /// <summary>
        /// Конструктор
        /// </summary>
        public MyGoogleCalendar()
        {
            _log = NLog.LogManager.GetLogger("MyGoogleCalendar");
        }
        /// <summary>
        /// Авторизоваться
        /// </summary>
        /// <returns></returns>
        public bool Auth()
        {
            try
            {
                UserCredential credential;

                using (var stream =
                    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;

                }

                // Create Google Calendar API service.
                _calendar = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "MyContact",
                });
                _status = "Подключен к Google календарю.";
                return true;
            }
            catch(Exception ex)
            {
                _log.Error(ex.ToString());
                return false;
            }
        }
        /// <summary>
        /// Получить статус
        /// </summary>
        /// <returns></returns>
        public string GetStatus()
        {
            var toGet = _status;
            _status = "";
            return toGet;
        }
        /// <summary>
        /// Получить события из календаря начинай с даты
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        private List<MyEvent> GetEvents(DateTime date, DateTime end, List<Price> prices)
        {
            var myEvents = new List<MyEvent>();
            try
            {
                EventsResource.ListRequest request = _calendar.Events.List("primary");
                request.TimeMin = date;
                request.TimeMax = end;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 10;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                Events events = request.Execute();

                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var eventItem in events.Items)
                    {
                        try
                        {
                            //0 - Услуга, 1 - Стоимость, 2 - Предоплата, 3 Телефон, 4 Коментарий
                            var comments = eventItem.Description?.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                            var myEvent = new MyEvent
                            {
                                Id = eventItem.Id,
                                Name = eventItem.Summary,
                                Location = eventItem.Location,
                                TimeStart = eventItem.Start.DateTime.Value,
                                TimeFinish = eventItem.End.DateTime.Value,
                                Date = eventItem.Start.DateTime.Value,
                                Phone = comments.Length >= 4 ? comments[3] : "",
                                Payment = comments.Length >= 2 ? Convert.ToDecimal(comments[1]) : 0,
                                Prepayment = comments.Length >= 3 ? Convert.ToDecimal(comments[2]) : 0,
                                Items = comments.Length >= 1 ? comments[0] : "",
                                Commentary = comments.Length >= 5 ? comments[4] : ""
                            };
                            var fullNameItem = prices.Where(p => p.Short == myEvent.Items).FirstOrDefault()?.Name;
                            myEvent.Items = fullNameItem != "" ? fullNameItem : myEvent.Items;

                            myEvents.Add(myEvent);
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.ToString());
                        }
                    }                   
                    _status = $"Считано {myEvents.Count} из {events.Items.Count} записей.";
                }
                else
                {
                    _status = $"Нет данных для считвания.";
                }            
            }
            catch (Exception ex)
            {
                _status = $"Ошибка получения событий из Google календарь";
                _log.Error(ex.ToString());
            }
            return myEvents;

        }

        public async Task<List<MyEvent>> GetEventsAsync(DateTime date, DateTime end, List<Price> prices)
        {          
            return await Task.Run(() =>  GetEvents(date, end, prices));
        }
    }
}
