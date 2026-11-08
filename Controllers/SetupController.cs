using Airline_Tickets.Data;
using Airline_Tickets.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airline_Tickets.Controllers
{
    public record CreateFlightRequest(string FlightNumber, string Origin, string Destination, DateTimeOffset DepartureTime, DateTimeOffset ArrivalTime);
    public record CreateSeatRequest(Guid FlightId, int Number, char Letter);
    public record CreatePassengerRequest(string Name, string Surname, string PassportNumber);

    [ApiController]
    [Route("api/setup")]
    public class SetupController : ControllerBase
    {
        private readonly AirlineDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public SetupController(AirlineDbContext db, IWebHostEnvironment environment) { _db = db; _environment = environment; }

        [HttpPost("flights")]
        public async Task<IActionResult> Flight(CreateFlightRequest request, CancellationToken ct)
        {
            if (!_environment.IsDevelopment()) return NotFound();
            try
            {
                var flight = new Flight(request.FlightNumber, request.Origin, request.Destination, request.DepartureTime, request.ArrivalTime);
                _db.Flights.Add(flight);
                await _db.SaveChangesAsync(ct);
                return Ok(new { flight.Id });
            }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("seats")]
        public async Task<IActionResult> Seat(CreateSeatRequest request, CancellationToken ct)
        {
            if (!_environment.IsDevelopment()) return NotFound();
            try
            {
                if (!await _db.Flights.AnyAsync(x => x.Id == request.FlightId, ct)) return NotFound();
                var seat = new Seat(request.FlightId, request.Number, request.Letter);
                _db.Seats.Add(seat);
                await _db.SaveChangesAsync(ct);
                return Ok(new { seat.Id });
            }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("passengers")]
        public async Task<IActionResult> Passenger(CreatePassengerRequest request, CancellationToken ct)
        {
            if (!_environment.IsDevelopment()) return NotFound();
            try
            {
                var passenger = new Passenger(request.Name, request.Surname, request.PassportNumber);
                _db.Passengers.Add(passenger);
                await _db.SaveChangesAsync(ct);
                return Ok(new { passenger.Id });
            }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        }
    }
}
