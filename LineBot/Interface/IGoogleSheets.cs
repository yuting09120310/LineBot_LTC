using LineBot.Models;

namespace LineBot.Interface
{
    public interface IGoogleSheets
    {
        void CreateGoogleSheet(ReservationRequest reservationRequest);

        ReservationRequest ReadGoogleSheet(int Id);

        List<ReservationRequest> ListReadGoogleSheet(string userId);

        void UpdateGoogleSheet(ReservationRequest reservationRequest);

        void DeleteGoogleSheet(int Id);
    }
}
