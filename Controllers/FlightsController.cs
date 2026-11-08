using Airline_Tickets.Data;
using Airline_Tickets.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airline_Tickets.Controllers
{
    [ApiController]
    [Route("api/flights")]
    public class FlightsController : ControllerBase
    {
        private readonly AirlineDbContext _db;

        public FlightsController(AirlineDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetFlights(CancellationToken ct)
        {
            var flights = await _db.Flights.AsNoTracking()
                .OrderBy(x => x.DepartureTime)
                .Select(x => new { x.Id, x.FlightNumber, x.Origin, x.Destination, x.DepartureTime, x.ArrivalTime })
                .ToListAsync(ct);
            return Ok(flights);
        }

        [HttpGet("{flightId:guid}/seats")]
        public async Task<IActionResult> GetSeats(Guid flightId, CancellationToken ct)
        {
            if (!await _db.Flights.AnyAsync(x => x.Id == flightId, ct)) return NotFound();
            var now = DateTimeOffset.UtcNow;
            var seats = await _db.Seats.AsNoTracking()
                .Where(x => x.FlightId == flightId)
                .OrderBy(x => x.Number).ThenBy(x => x.Letter)
                .Select(x => new
                {
                    x.Id, x.Number, x.Letter,
                    IsAvailable = !_db.Tickets.Any(t => t.SeatId == x.Id) &&
                        !_db.Reservations.Any(r => r.SeatId == x.Id &&
                            (r.Status == ReservationStatus.Confirmed ||
                             (r.Status == ReservationStatus.Active && r.ExpirationTime > now)))
                })
                .ToListAsync(ct);
            return Ok(seats);
        }
    }
}
