document.addEventListener('DOMContentLoaded', function() {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next,today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        events: [
            {
                title: 'Shift 1',
                start: '2024-04-01T08:00:00',
                end: '2024-04-01T16:00:00'
            },
            {
                title: 'Shift 2',
                start: '2024-04-02T12:00:00',
                end: '2024-04-02T20:00:00'
            }
        ]
    });
    calendar.render();
});