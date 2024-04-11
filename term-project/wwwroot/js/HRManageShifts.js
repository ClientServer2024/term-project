document.addEventListener('DOMContentLoaded', () => {
    load_calender();
     
    // retrieve buttons and add event listeners
    const vc_btn = document.getElementById('vc_btn');
    const cs_btn = document.getElementById('cs_btn');
    const ms_btn = document.getElementById('ms_btn');
    vc_btn.addEventListener('click', Handle_VC_Click);
    cs_btn.addEventListener('click', Handle_CS_Click);
    ms_btn.addEventListener('click', Handle_MS_Click);
});

function load_calender() {
    let calendarEl = document.getElementById('content-box');
    calendarEl.innerHTML = '';
    let calendar = new FullCalendar.Calendar(calendarEl, {
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
}

function Handle_VC_Click() {
    load_calender();
}

function Handle_CS_Click() {
    $.ajax({
        url: '/HR/HRCreateShift',
        type: 'GET',
        success: function(data) {
            $('#content-box').html('');
            $('#content-box').html(data);
        },
        error: function(xhr, status, error) {
            console.error("Error loading page: ", error)
        }
    });
}

function Handle_MS_Click() {
    $.ajax({
        url: '/HR/HRCreateShift',
        type: 'GET',
        success: function(data) {
            $('#content-box').html('');
            $('#content-box').html(data);
        },
        error: function(xhr, status, error) {
            console.error("Error loading page: ", error)
        }
    });
}
