const $ = id => document.getElementById(id);
const state = { flights: [], seats: [], selected: null, reservation: null };

async function api(path, options = {}) {
  const response = await fetch(path, { headers: { 'Content-Type': 'application/json' }, ...options });
  const body = await response.json().catch(() => ({}));
  if (!response.ok) throw new Error(body.error || `Ошибка ${response.status}`);
  return body;
}

function message(text, success = false) {
  const box = $('message');
  box.textContent = text;
  box.classList.toggle('success', success);
  box.hidden = false;
}

function renderSeats() {
  const root = $('seats');
  root.replaceChildren();
  root.classList.toggle('empty', state.seats.length === 0);
  if (!state.seats.length) { root.textContent = 'Для этого рейса места пока не добавлены.'; return; }
  for (const seat of state.seats) {
    const button = document.createElement('button');
    button.type = 'button';
    button.className = 'seat' + (state.selected?.id === seat.id ? ' selected' : '');
    button.textContent = `${seat.number}${seat.letter}`;
    button.disabled = !seat.isAvailable || Boolean(state.reservation);
    button.title = seat.isAvailable ? 'Выбрать место' : 'Место занято';
    button.addEventListener('click', () => {
      state.selected = seat;
      $('selected-seat').textContent = button.textContent;
      $('book').disabled = false;
      renderSeats();
    });
    root.append(button);
  }
}

async function loadFlights() {
  try {
    state.flights = await api('/api/flights');
    const select = $('flight');
    select.replaceChildren(new Option(state.flights.length ? 'Выберите рейс' : 'Рейсов пока нет', ''));
    for (const flight of state.flights) select.add(new Option(`${flight.flightNumber} · ${flight.origin} → ${flight.destination}`, flight.id));
  } catch (error) { $('flight').replaceChildren(new Option('Не удалось загрузить рейсы', '')); message(error.message); }
}

async function loadSeats() {
  const flightId = $('flight').value;
  state.selected = null;
  $('selected-seat').textContent = '—';
  $('book').disabled = true;
  const details = $('flight-details');
  if (!flightId) { state.seats = []; $('seats').textContent = 'Выберите рейс, чтобы увидеть места.'; details.hidden = true; return; }
  const flight = state.flights.find(x => x.id === flightId);
  details.textContent = `${flight.origin} → ${flight.destination} · Вылет ${new Date(flight.departureTime).toLocaleString('ru-RU')}`;
  details.hidden = false;
  try { state.seats = await api(`/api/flights/${encodeURIComponent(flightId)}/seats`); renderSeats(); }
  catch (error) { state.seats = []; renderSeats(); message(error.message); }
}

$('flight').addEventListener('change', loadSeats);
$('refresh').addEventListener('click', loadSeats);
$('booking-form').addEventListener('submit', async event => {
  event.preventDefault();
  if (!state.selected || state.reservation) return;
  const button = $('book'); button.disabled = true; button.textContent = 'Бронируем…';
  try {
    const passenger = await api('/api/setup/passengers', { method: 'POST', body: JSON.stringify({ name: $('name').value, surname: $('surname').value, passportNumber: $('passport').value }) });
    state.reservation = await api('/api/reservations', { method: 'POST', body: JSON.stringify({ seatId: state.selected.id, passengerId: passenger.id }) });
    $('expires').textContent = new Date(state.reservation.expirationTime).toLocaleString('ru-RU');
    $('reservation-id').textContent = `Бронь: ${state.reservation.id}`;
    $('reservation-panel').hidden = false;
    message('Место успешно забронировано.', true);
    renderSeats();
  } catch (error) { message(error.message); button.disabled = false; await loadSeats(); }
  finally { button.textContent = 'Забронировать место →'; }
});

$('issue-ticket').addEventListener('click', async () => {
  if (!state.reservation) return;
  const button = $('issue-ticket'); button.disabled = true; button.textContent = 'Оформляем…';
  try {
    const ticket = await api(`/api/reservations/${encodeURIComponent(state.reservation.id)}/ticket`, { method: 'POST' });
    $('ticket-id').textContent = `Номер билета: ${ticket.id}`;
    $('ticket-panel').hidden = false;
    $('reservation-panel').hidden = true;
    message('Билет успешно оформлен.', true);
  } catch (error) { message(error.message); button.disabled = false; button.textContent = 'Выпустить билет'; }
});

loadFlights();
