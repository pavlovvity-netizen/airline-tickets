namespace Airline_Tickets.Domain
{
    public class Seat
    {
        private Guid _id;
        private Guid _flightId;
        private int _number;
        private char _letter;

        public Guid Id => _id;
        public Guid FlightId => _flightId;
        public int Number => _number;
        public char Letter => _letter;

        public Seat(Guid flightId, int number, char letter)
        {
            if (flightId == Guid.Empty) throw new ArgumentException("Укажите рейс.", nameof(flightId));
            if (number <= 0) throw new ArgumentOutOfRangeException(nameof(number));
            letter = char.ToUpperInvariant(letter);
            if (letter is < 'A' or > 'Z') throw new ArgumentException("Недопустимая буква места.", nameof(letter));
            _id = Guid.NewGuid();
            _flightId = flightId;
            _number = number;
            _letter = letter;
        }
    }
}
