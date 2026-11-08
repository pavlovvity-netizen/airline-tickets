# Airline Tickets

Учебный ASP.NET Core сервис бронирования мест на рейсах с PostgreSQL и EF Core.

## Локальный запуск

1. Запустите PostgreSQL: `docker compose up -d`.
2. Создайте `appsettings.Development.json` со строкой подключения `ConnectionStrings:AirlineDatabase`. Для локального `compose.yaml`: `Host=localhost;Port=5433;Database=airline_tickets;Username=app;Password=app`.
3. Примените миграции: `dotnet ef database update` или `Update-Database` в Visual Studio.
4. Запустите проект через Visual Studio или `dotnet run` и откройте корневой адрес приложения.

Форма добавления рейса и мест пока доступна через `/api/setup` только в режиме Development. Страница бронирования находится в `wwwroot`.

`compose.yaml` содержит демонстрационные учётные данные только для локальной разработки. Для реального развёртывания задайте отдельный пароль через секреты окружения.
