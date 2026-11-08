namespace Airline_Tickets.Domain
{
    public class Ticket
    {
        private Guid _id;
        private Guid _flightId;
        private Guid _passengerId;
        private Guid _seatId;

        public Guid Id => _id;
        public Guid FlightId => _flightId;
        public Guid PassengerId => _passengerId;
        public Guid SeatId => _seatId;

        public Ticket(Guid flightId, Guid passengerId, Guid seatId)
        {
            if (flightId == Guid.Empty) throw new ArgumentException("Укажите рейс.", nameof(flightId));
            if (passengerId == Guid.Empty) throw new ArgumentException("Укажите пассажира.", nameof(passengerId));
            if (seatId == Guid.Empty) throw new ArgumentException("Укажите место.", nameof(seatId));
            _id = Guid.NewGuid();
            _flightId = flightId;
            _passengerId = passengerId;
            _seatId = seatId;
        }
    }
}
