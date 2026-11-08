using Airline_Tickets.Domain;
using Airline_Tickets.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Airline_Tickets.Services
{
    public class BookingService
    {
        private readonly SeatRepository _seats;
        private readonly ReservationRepository _reservations;
        private readonly Airline_Tickets.Data.AirlineDbContext _db;

        public BookingService(SeatRepository seats, ReservationRepository reservations, Airline_Tickets.Data.AirlineDbContext db)
        {
            _seats = seats;
            _reservations = reservations;
            _db = db;
        }

        public async Task<Reservation> ReserveAsync(Guid seatId, Guid passengerId, CancellationToken cancellationToken)
        {
            if (seatId == Guid.Empty || passengerId == Guid.Empty)
                throw new ArgumentException("Укажите место и пассажира.");

            var seat = await _seats.GetByIdAsync(seatId, cancellationToken)
                ?? throw new KeyNotFoundException("Место не найдено.");

            if (!await _db.Passengers.AnyAsync(x => x.Id == passengerId, cancellationToken))
                throw new KeyNotFoundException("Пассажир не найден.");

            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            if (await _reservations.TicketExistsAsync(seatId, cancellationToken))
                throw new InvalidOperationException("Билет на это место уже выдан.");
            var old = await _reservations.GetBlockingReservationAsync(seatId, cancellationToken);
            var now = DateTimeOffset.UtcNow;
            if (old != null)
            {
                if (old.Status == ReservationStatus.Confirmed || old.ExpirationTime > now)
                    throw new InvalidOperationException("Место уже занято.");
                old.Expire(now);
                await _reservations.SaveChangesAsync(cancellationToken);
            }

            var reservation = new Reservation(seat.FlightId, passengerId, seatId, now.AddMinutes(10));
            _reservations.AddReservation(reservation);
            try
            {
                await _reservations.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("Место уже занято или бронь изменена другим запросом.");
            }
            return reservation;
        }
    }
}
