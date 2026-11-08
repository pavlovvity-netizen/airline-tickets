namespace Airline_Tickets.Domain
{
    public enum ReservationStatus
    {
        Active,
        Confirmed,
        Cancelled,
        Expired
    }
    public class Reservation
    {
        private Guid _id;
        private Guid _flightId;
        private Guid _passengerId;
        private Guid _seatId;
        private ReservationStatus _status;
        private DateTimeOffset _expirationTime;

        public Guid Id => _id;
        public Guid FlightId => _flightId;
        public Guid PassengerId => _passengerId;
        public Guid SeatId => _seatId;
        public ReservationStatus Status => _status;
        public DateTimeOffset ExpirationTime => _expirationTime;

        public Reservation(Guid flightId, Guid passengerId, Guid seatId, DateTimeOffset expirationTime)
        {
            if (flightId == Guid.Empty) throw new ArgumentException("Укажите рейс.", nameof(flightId));
            if (passengerId == Guid.Empty) throw new ArgumentException("Укажите пассажира.", nameof(passengerId));
            if (seatId == Guid.Empty) throw new ArgumentException("Укажите место.", nameof(seatId));
            if (expirationTime <= DateTimeOffset.UtcNow) throw new ArgumentException("Срок брони должен быть в будущем.", nameof(expirationTime));
            _id = Guid.NewGuid();
            _flightId = flightId;
            _passengerId = passengerId;
            _seatId = seatId;
            _status = ReservationStatus.Active;
            _expirationTime = expirationTime;
        }

        private Reservation() { }

        public void Cancel()
        {
            if (_status != ReservationStatus.Active) throw new InvalidOperationException("Отменить можно только активную бронь.");
            _status = ReservationStatus.Cancelled;
        }

        public void Expire(DateTimeOffset now)
        {
            if (_status != ReservationStatus.Active)
                throw new InvalidOperationException("Завершить по сроку можно только активную бронь.");

            if (now < _expirationTime)
                throw new InvalidOperationException("Бронь не может быть завершена до истечения срока.");

            _status = ReservationStatus.Expired;
        }

        public void Confirm(DateTimeOffset now)
        {
            if (_status != ReservationStatus.Active)
                throw new InvalidOperationException(
                    "Подтвердить можно только активную бронь.");

            if (now >= _expirationTime)
                throw new InvalidOperationException("Срок брони истёк.");

            _status = ReservationStatus.Confirmed;
        }
    }
}
