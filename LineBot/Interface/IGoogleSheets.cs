using LineBot.Models;

namespace LineBot.Interface
{
    public interface IGoogleSheets
    {
        void CreateData(ReservationRequest reservationRequest);

        List<ReservationRequest> ListReadData();

        List<ReservationRequest> ListReadData(string userId);

        ReservationRequest ReadData(int Id);

        void UpdateData(ReservationRequest reservationRequest);

        void DeleteData(int Id);
    }
}
