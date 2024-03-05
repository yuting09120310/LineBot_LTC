using LineBot.Models;

namespace LineBot.Interface
{
    public interface IGoogleSheets
    {
        void CreateData(ReservationRequest reservationRequest);

        ReservationRequest ReadData(int Id);

        List<ReservationRequest> ListReadData(string userId);

        void UpdateData(ReservationRequest reservationRequest);

        void DeleteData(int Id);
    }
}
