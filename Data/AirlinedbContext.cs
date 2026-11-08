using Airline_Tickets.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airline_Tickets.Data;

public class AirlineDbContext : DbContext
{
    public AirlineDbContext(DbContextOptions<AirlineDbContext> options)
        : base(options)
    {
    }

    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasField("_id");
            entity.Property(x => x.FlightNumber).HasField("_flightNumber");
            entity.Property(x => x.Origin).HasField("_origin");
            entity.Property(x => x.Destination).HasField("_destination");
            entity.Property(x => x.DepartureTime).HasField("_departureTime");
            entity.Property(x => x.ArrivalTime).HasField("_arrivalTime");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasField("_id");
            entity.Property(x => x.FlightId).HasField("_flightId");
            entity.Property(x => x.Number).HasField("_number");
            entity.Property(x => x.Letter).HasField("_letter");

            entity.HasIndex(x => new { x.FlightId, x.Number, x.Letter })
                .IsUnique();

            entity.HasOne<Flight>()
                .WithMany()
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasField("_id");
            entity.Property(x => x.Name).HasField("_name");
            entity.Property(x => x.Surname).HasField("_surname");
            entity.Property(x => x.PassportNumber).HasField("_passportNumber");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasField("_id");
            entity.Property(x => x.FlightId).HasField("_flightId");
            entity.Property(x => x.PassengerId).HasField("_passengerId");
            entity.Property(x => x.SeatId).HasField("_seatId");
            entity.Property(x => x.ExpirationTime).HasField("_expirationTime");

            entity.Property(x => x.Status)
                .HasField("_status")
                .IsConcurrencyToken();

            entity.HasIndex(x => x.SeatId)
                .IsUnique()
                .HasFilter("\"Status\" IN (0, 1)");

            entity.HasOne<Flight>()
                .WithMany()
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Passenger>()
                .WithMany()
                .HasForeignKey(x => x.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Seat>()
                .WithMany()
                .HasForeignKey(x => x.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasField("_id");
            entity.Property(x => x.FlightId).HasField("_flightId");
            entity.Property(x => x.PassengerId).HasField("_passengerId");
            entity.Property(x => x.SeatId).HasField("_seatId");

            entity.HasIndex(x => x.SeatId).IsUnique();

            entity.HasOne<Flight>()
                .WithMany()
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Passenger>()
                .WithMany()
                .HasForeignKey(x => x.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Seat>()
                .WithMany()
                .HasForeignKey(x => x.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}