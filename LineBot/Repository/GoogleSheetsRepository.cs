using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using LineBot.Interface;
using LineBot.Models;
using static System.Formats.Asn1.AsnWriter;

namespace LineBot.Repository
{
    public class GoogleSheetsRepository : IGoogleSheets
    {
        public GoogleSheetsRepository()
        {

        }

        string[] scopes = { SheetsService.Scope.Spreadsheets };

        public void CreateGoogleSheet(ReservationRequest reservationRequest)
        {
            try
            {
                GoogleCredential credential;
                using (FileStream steam = new FileStream("C:\\grand-forge-370208-5de023c336c5.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(steam).CreateScoped(scopes);
                }

                SheetsService service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "AlexProject",
                });


                string spreadsheetId = "14RE63i1tbyuK2jx0NJ-OUZtiJWUYCPzNvPgOQjtsess";
                //string range = "工作表1!O:O";
                string range = "工作表1";

                SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);

                Google.Apis.Sheets.v4.Data.ValueRange response = request.Execute();
                IList<IList<object>> values = response.Values;



                // 設定要寫入的資料
                List<object> rowData = new List<object> {
                    values.Count + 1,
                    reservationRequest.UserId,
                    reservationRequest.FullName,
                    reservationRequest.ServiceDate,
                    reservationRequest.ServiceTime,
                    reservationRequest.PickupLocation,
                    reservationRequest.DropOffLocation,
                    reservationRequest.ReturnServiceTime,
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
                    = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, sRange);
                updateRequest.ValueInputOption
                    = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                UpdateValuesResponse uUVR = updateRequest.Execute();
            }
            catch (Exception ex)
            {

            }
        }


        public ReservationRequest ReadGoogleSheet(int Id)
        {
            GoogleCredential credential;
            using (FileStream steam = new FileStream("C:\\grand-forge-370208-5de023c336c5.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(steam).CreateScoped(scopes);
            }

            SheetsService service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "AlexProject",
            });


            string spreadsheetId = "14RE63i1tbyuK2jx0NJ-OUZtiJWUYCPzNvPgOQjtsess";
            //string range = "工作表1!O:O";
            string range = "工作表1";

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);

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
                // 檢查列表是否包含目標值
                if (row.Count > 1 && Convert.ToInt32(row[0].ToString()) == Id)
                {
                    //如果你想將每一行的資料轉換為一個 Model 物件，可以在這裡進行轉換
                    reservationRequest.Id = row[0].ToString();
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


        public List<ReservationRequest> ListReadGoogleSheet(string userId)
        {
            GoogleCredential credential;
            using (FileStream steam = new FileStream("C:\\grand-forge-370208-5de023c336c5.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(steam).CreateScoped(scopes);
            }

            SheetsService service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "AlexProject",
            });


            string spreadsheetId = "14RE63i1tbyuK2jx0NJ-OUZtiJWUYCPzNvPgOQjtsess";
            //string range = "工作表1!O:O";
            string range = "工作表1";

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);

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
                    reservationRequest.Id = row[0].ToString();
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


        public void UpdateGoogleSheet(ReservationRequest reservationRequest)
        {
            // 實現你的 Google Sheets 邏輯
        }


        public void DeleteGoogleSheet(int userId)
        {
            // 實現你的 Google Sheets 邏輯
        }
    }
}
