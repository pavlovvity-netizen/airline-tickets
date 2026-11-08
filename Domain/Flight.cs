namespace Airline_Tickets.Domain
{
    public class Flight
    {
        private Guid _id;
        private string _flightNumber;
        private string _origin;
        private string _destination;
        private DateTimeOffset _departureTime;
        private DateTimeOffset _arrivalTime;

        public Guid Id => _id;
        public string FlightNumber => _flightNumber;
        public string Origin => _origin;
        public string Destination => _destination;
        public DateTimeOffset DepartureTime => _departureTime;
        public DateTimeOffset ArrivalTime => _arrivalTime;

        public Flight(string flightNumber, string origin, string destination, DateTimeOffset departureTime, DateTimeOffset arrivalTime)
        {
            if (string.IsNullOrWhiteSpace(flightNumber)) throw new ArgumentException("Укажите номер рейса.", nameof(flightNumber));
            if (string.IsNullOrWhiteSpace(origin)) throw new ArgumentException("Укажите аэропорт вылета.", nameof(origin));
            if (string.IsNullOrWhiteSpace(destination)) throw new ArgumentException("Укажите аэропорт прилёта.", nameof(destination));
            if (origin.Trim().Equals(destination.Trim(), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Аэропорты должны отличаться.", nameof(destination));
            if (departureTime.Offset != TimeSpan.Zero || arrivalTime.Offset != TimeSpan.Zero) throw new ArgumentException("Передавайте время в UTC.");
            if (arrivalTime <= departureTime) throw new ArgumentException("Прилёт должен быть позже вылета.", nameof(arrivalTime));
            _id = Guid.NewGuid();
            _flightNumber = flightNumber.Trim().ToUpperInvariant();
            _origin = origin.Trim().ToUpperInvariant();
            _destination = destination.Trim().ToUpperInvariant();
            _departureTime = departureTime;
            _arrivalTime = arrivalTime;
        }
    }
}
