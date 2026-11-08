using Airline_Tickets.Domain;
using Airline_Tickets.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Airline_Tickets.Services
{
    public class TicketService
    {
        private readonly ReservationRepository _reservations;

        public TicketService(ReservationRepository reservations)
        {
            _reservations = reservations;
        }

        public async Task<Ticket> IssueTicketAsync(
            Guid reservationId,
            CancellationToken cancellationToken)
        {
            Reservation? reservation =
                await _reservations.GetByIdAsync(reservationId, cancellationToken);

            if (reservation is null)
                throw new KeyNotFoundException("Бронь не найдена.");

            // Метод внутри Reservation проверяет статус и срок брони.
            reservation.Confirm(DateTimeOffset.UtcNow);

            var ticket = new Ticket(
                reservation.FlightId,
                reservation.PassengerId,
                reservation.SeatId);

            _reservations.AddTicket(ticket);

            try
            {
                await _reservations.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "Бронь уже изменена другим запросом.");
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("Место уже занято.");
            }

            return ticket;
        }
    }
}
