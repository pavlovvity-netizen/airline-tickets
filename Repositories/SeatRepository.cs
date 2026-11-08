using Airline_Tickets.Data;
using Airline_Tickets.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airline_Tickets.Repositories
{
    public class SeatRepository
    {
        private readonly AirlineDbContext _db;

        public SeatRepository(AirlineDbContext db)
        {
            _db = db;
        }

        public async Task<Seat?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Seats.FirstOrDefaultAsync(seat => seat.Id == id, cancellationToken);
        }
    }
}
