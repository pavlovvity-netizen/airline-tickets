using Airline_Tickets.Services;
using Microsoft.AspNetCore.Mvc;

namespace Airline_Tickets.Controllers
{
    public record CreateReservationRequest(Guid SeatId, Guid PassengerId);

    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly BookingService _bookings;
        private readonly TicketService _tickets;

        public ReservationsController(BookingService bookings, TicketService tickets)
        {
            _bookings = bookings;
            _tickets = tickets;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReservationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var reservation = await _bookings.ReserveAsync(request.SeatId, request.PassengerId, cancellationToken);
                return Created($"/api/reservations/{reservation.Id}", new { reservation.Id, reservation.ExpirationTime });
            }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }

        [HttpPost("{id:guid}/ticket")]
        public async Task<IActionResult> IssueTicket(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var ticket = await _tickets.IssueTicketAsync(id, cancellationToken);
                return Ok(new { ticket.Id, ticket.FlightId, ticket.PassengerId, ticket.SeatId });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }
    }
}
