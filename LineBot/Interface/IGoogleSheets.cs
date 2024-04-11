using LineBot.Models;

namespace LineBot.Interface
{
    public interface IGoogleSheets
    {
        void CreateReservation(ReservationRequest reservationRequest);

        List<ReservationRequest> GetAllReservation();

        List<ReservationRequest> GetUserReservation(string userId);

        ReservationRequest GetReservation(int Id);

        void UpdateReservation(ReservationRequest reservationRequest);

        void DeleteReservation(int Id);
    }
}
