namespace Airline_Tickets.Domain
{
    public class Passenger
    {
        private Guid _id;
        private string _name;
        private string _surname;
        private string _passportNumber;

        public Guid Id => _id;
        public string Name => _name;
        public string Surname => _surname;
        public string PassportNumber => _passportNumber;

        public Passenger(string name, string surname, string passportNumber)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Укажите имя.", nameof(name));
            if (string.IsNullOrWhiteSpace(surname)) throw new ArgumentException("Укажите фамилию.", nameof(surname));
            if (string.IsNullOrWhiteSpace(passportNumber)) throw new ArgumentException("Укажите документ.", nameof(passportNumber));
            _id = Guid.NewGuid();
            _name = name.Trim();
            _surname = surname.Trim();
            _passportNumber = passportNumber.Trim();
        }
    }
}
