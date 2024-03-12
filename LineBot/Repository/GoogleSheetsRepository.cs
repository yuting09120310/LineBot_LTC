using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using LineBot.Interface;
using LineBot.Models;
using static System.Formats.Asn1.AsnWriter;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using static Google.Apis.Sheets.v4.SheetsService;

namespace LineBot.Repository
{
    public class GoogleSheetsRepository : IGoogleSheets
    {
        private string _webUrl;
        private string _jsonFileUrl;
        private string _spreadsheetId = "14RE63i1tbyuK2jx0NJ-OUZtiJWUYCPzNvPgOQjtsess";
        string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private GoogleCredential _credential;
        string range = "工作表1";


        public GoogleSheetsRepository(string webUrl)
        {
            _webUrl = webUrl;
            InitializeGoogleCredential();
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void InitializeGoogleCredential()
        {
            using (var httpClient = new HttpClient())
            {
                _jsonFileUrl = _webUrl + "/lib/grand-forge-370208-5de023c336c5.json";
                var jsonContent = httpClient.GetStringAsync(_jsonFileUrl).Result;
                _credential = GoogleCredential.FromJson(jsonContent).CreateScoped(_scopes);
            }
        }


        /// <summary>
        /// 建立SheetsService 服務
        /// </summary>
        /// <returns></returns>
        private SheetsService CreateSheetsService()
        {
            return new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = "AlexProject",
            });
        }


        /// <summary>
        /// 新增訂單
        /// </summary>
        /// <param name="reservationRequest">訂單資料</param>
        public void CreateData(ReservationRequest reservationRequest)
        {
            try
            {
                SheetsService service = CreateSheetsService();

                SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(_spreadsheetId, range);

                Google.Apis.Sheets.v4.Data.ValueRange response = request.Execute();
                IList<IList<object>> values = response.Values;

                // 設定要寫入的資料
                List<object> rowData = new List<object> {
                    values.Count + 1,
                    reservationRequest.UserId,
                    reservationRequest.FullName,
                    reservationRequest.ServiceDate.ToString("yyyy-MM-dd"),
                    reservationRequest.ServiceTime.ToString("c"),
                    reservationRequest.PickupLocation,
                    reservationRequest.DropOffLocation,
                    reservationRequest.ReturnServiceTime?.ToString("c"),
                    reservationRequest.MedicalPurpose,
                    reservationRequest.AccompanyingPersons,
                    reservationRequest.ContactTitle,
                    reservationRequest.ContactPhoneNumber,
                    reservationRequest.ServiceType,
                    reservationRequest.LongTermCareQualification,
                    reservationRequest.Notes
                };

                // 指定寫入的範圍
                string sRange = String.Format("{0}!A{1}:O{1}", range, values.Count + 1);

                // 創建 ValueRange 對象
                ValueRange valueRange = new ValueRange
                {
                    MajorDimension = "ROWS",
                    Values = new List<IList<object>> { rowData }
                };

                //執行寫入動作
                SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest
                    = service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, sRange);
                updateRequest.ValueInputOption
                    = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                UpdateValuesResponse uUVR = updateRequest.Execute();
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// 取得所有訂單
        /// </summary>
        /// <returns></returns>
        public List<ReservationRequest> ListReadData()
        {
            SheetsService service = CreateSheetsService();

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(_spreadsheetId, range);

            Google.Apis.Sheets.v4.Data.ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            List<ReservationRequest> ListReservationRequest = new List<ReservationRequest>();

            // 移除標題行
            if (values.Count > 0)
            {
                values.RemoveAt(0);
            }

            foreach (var row in values)
            {
                //如果你想將每一行的資料轉換為一個 Model 物件，可以在這裡進行轉換
                ReservationRequest reservationRequest = new ReservationRequest();
                reservationRequest.Id = Convert.ToInt32(row[0].ToString());
                reservationRequest.UserId = row[1].ToString();
                reservationRequest.FullName = row[2].ToString();
                reservationRequest.ServiceDate = Convert.ToDateTime(row[3].ToString());
                reservationRequest.ServiceTime = TimeSpan.Parse(row[4].ToString());
                reservationRequest.PickupLocation = row[5].ToString();
                reservationRequest.DropOffLocation = row[6].ToString();
                if (TimeSpan.TryParse(row[7].ToString(), out TimeSpan returnServiceTime))
                {
                    reservationRequest.ReturnServiceTime = returnServiceTime;
                }
                reservationRequest.MedicalPurpose = row[8].ToString();
                reservationRequest.AccompanyingPersons = Convert.ToInt16(row[9].ToString());
                reservationRequest.ContactTitle = row[10].ToString();
                reservationRequest.ContactPhoneNumber = row[11].ToString();
                reservationRequest.ServiceType = row[12].ToString();
                reservationRequest.LongTermCareQualification = row[13].ToString();
                if (row.Count > 14 && row[14] != null)
                {
                    reservationRequest.Notes = row[14].ToString();
                }

                ListReservationRequest.Add(reservationRequest);
            }

            return ListReservationRequest;
        }


        /// <summary>
        /// 取得該使用者所有訂單
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ReservationRequest> ListReadData(string userId)
        {
            SheetsService service = CreateSheetsService();

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(_spreadsheetId, range);

            Google.Apis.Sheets.v4.Data.ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            List<ReservationRequest> ListReservationRequest = new List<ReservationRequest>();

            // 移除標題行
            if (values.Count > 0)
            {
                values.RemoveAt(0);
            }

            foreach (var row in values)
            {
                // 檢查列表是否包含目標值
                if (row.Count > 1 && row[1].ToString() == userId)
                {
                    //如果你想將每一行的資料轉換為一個 Model 物件，可以在這裡進行轉換
                    ReservationRequest reservationRequest = new ReservationRequest();
                    reservationRequest.Id = Convert.ToInt32(row[0].ToString());
                    reservationRequest.UserId = row[1].ToString();
                    reservationRequest.FullName = row[2].ToString();
                    reservationRequest.ServiceDate = Convert.ToDateTime(row[3].ToString());
                    reservationRequest.ServiceTime = TimeSpan.Parse(row[4].ToString());
                    reservationRequest.PickupLocation = row[5].ToString();
                    reservationRequest.DropOffLocation = row[6].ToString();
                    if (TimeSpan.TryParse(row[7].ToString(), out TimeSpan returnServiceTime))
                    {
                        reservationRequest.ReturnServiceTime = returnServiceTime;
                    }
                    reservationRequest.MedicalPurpose = row[8].ToString();
                    reservationRequest.AccompanyingPersons = Convert.ToInt16(row[9].ToString());
                    reservationRequest.ContactTitle = row[10].ToString();
                    reservationRequest.ContactPhoneNumber = row[11].ToString();
                    reservationRequest.ServiceType = row[12].ToString();
                    reservationRequest.LongTermCareQualification = row[13].ToString();
                    reservationRequest.Notes = row[14].ToString();

                    ListReservationRequest.Add(reservationRequest);
                }
            }

            return ListReservationRequest;
        }


        /// <summary>
        /// 取得該筆訂單資料
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ReservationRequest ReadData(int Id)
        {
            SheetsService service = CreateSheetsService();

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(_spreadsheetId, range);

            Google.Apis.Sheets.v4.Data.ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;


            // 移除標題行
            if (values.Count > 0)
            {
                values.RemoveAt(0);
            }

            ReservationRequest reservationRequest = new ReservationRequest();

            foreach (var row in values)
            {
                int.TryParse(row[0].ToString(), out int rowId);
                // 檢查列表是否包含目標值
                if (row.Count > 1 && rowId == Id)
                {
                    //如果你想將每一行的資料轉換為一個 Model 物件，可以在這裡進行轉換
                    reservationRequest.Id = Convert.ToInt32(row[0].ToString());
                    reservationRequest.UserId = row[1].ToString();
                    reservationRequest.FullName = row[2].ToString();
                    reservationRequest.ServiceDate = Convert.ToDateTime(row[3].ToString());
                    reservationRequest.ServiceTime = TimeSpan.Parse(row[4].ToString());
                    reservationRequest.PickupLocation = row[5].ToString();
                    reservationRequest.DropOffLocation = row[6].ToString();
                    if (TimeSpan.TryParse(row[7].ToString(), out TimeSpan returnServiceTime))
                    {
                        reservationRequest.ReturnServiceTime = returnServiceTime;
                    }
                    reservationRequest.MedicalPurpose = row[8].ToString();
                    reservationRequest.AccompanyingPersons = Convert.ToInt16(row[9].ToString());
                    reservationRequest.ContactTitle = row[10].ToString();
                    reservationRequest.ContactPhoneNumber = row[11].ToString();
                    reservationRequest.ServiceType = row[12].ToString();
                    reservationRequest.LongTermCareQualification = row[13].ToString();
                    reservationRequest.Notes = row[14].ToString();
                }
            }


            return reservationRequest;
        }


        /// <summary>
        /// 更新訂單
        /// </summary>
        /// <param name="reservationRequest">新資料</param>
        public void UpdateData(ReservationRequest reservationRequest)
        {
            SheetsService service = CreateSheetsService();

            // 設定要寫入的資料
            List<object> rowData = new List<object> {
                    reservationRequest.Id ,
                    reservationRequest.UserId,
                    reservationRequest.FullName,
                    reservationRequest.ServiceDate.ToString("yyyy-MM-dd"),
                    reservationRequest.ServiceTime.ToString("c"),
                    reservationRequest.PickupLocation,
                    reservationRequest.DropOffLocation,
                    reservationRequest.ReturnServiceTime?.ToString("c"),
                    reservationRequest.MedicalPurpose,
                    reservationRequest.AccompanyingPersons,
                    reservationRequest.ContactTitle,
                    reservationRequest.ContactPhoneNumber,
                    reservationRequest.ServiceType,
                    reservationRequest.LongTermCareQualification,
                    reservationRequest.Notes
                };

            // 指定寫入的範圍
            string sRange = String.Format("{0}!A{1}:O{1}", range, reservationRequest.Id);

            // 創建 ValueRange 對象
            ValueRange valueRange = new ValueRange
            {
                MajorDimension = "ROWS",
                Values = new List<IList<object>> { rowData }
            };

            //執行寫入動作
            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest
                = service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, sRange);
            updateRequest.ValueInputOption
                = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            UpdateValuesResponse uUVR = updateRequest.Execute();
        }


        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="Id">訂單編號</param>
        public void DeleteData(int Id)
        {
            SheetsService service = CreateSheetsService();

            // 設定要寫入的資料
            List<object> rowData = new List<object> {"", ""};

            // 指定寫入的範圍
            string sRange = String.Format("{0}!A{1}:B{1}", range, Id);

            // 創建 ValueRange 對象
            ValueRange valueRange = new ValueRange
            {
                MajorDimension = "ROWS",
                Values = new List<IList<object>> { rowData }
            };

            //執行寫入動作
            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest
                = service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, sRange);
            updateRequest.ValueInputOption
                = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            UpdateValuesResponse uUVR = updateRequest.Execute();
        }

    }
}
