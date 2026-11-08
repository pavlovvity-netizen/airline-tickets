using Airline_Tickets.Data;
using Airline_Tickets.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airline_Tickets.Repositories
{
    public class ReservationRepository
    {
        private readonly AirlineDbContext _db;

        public ReservationRepository(AirlineDbContext db)
        {
            _db = db;
        }

        public Task<Reservation?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken)
        {
            return _db.Reservations.FirstOrDefaultAsync(
                reservation => reservation.Id == reservationId,
                cancellationToken);
        }

        public void AddTicket(Ticket ticket)
        {
            _db.Tickets.Add(ticket);
        }

        public Task<Reservation?> GetBlockingReservationAsync(Guid seatId, CancellationToken cancellationToken)
        {
            return _db.Reservations.FirstOrDefaultAsync(x => x.SeatId == seatId &&
                (x.Status == ReservationStatus.Active || x.Status == ReservationStatus.Confirmed), cancellationToken);
        }

        public Task<bool> TicketExistsAsync(Guid seatId, CancellationToken cancellationToken)
        {
            return _db.Tickets.AnyAsync(x => x.SeatId == seatId, cancellationToken);
        }

        public void AddReservation(Reservation reservation)
        {
            _db.Reservations.Add(reservation);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
