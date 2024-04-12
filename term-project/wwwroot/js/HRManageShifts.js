document.addEventListener('DOMContentLoaded', () => {
    load_calender();
     
    // retrieve buttons and add event listeners
    document.getElementById('vc_btn').addEventListener('click', Handle_VC_Click);
    document.getElementById('cs_btn').addEventListener('click', Handle_CS_Click);
    document.getElementById('ms_btn').addEventListener('click', Handle_MS_Click);
});

async function get_all_shifts() {
    try {
        let data = await $.ajax({
            url: '/HR/HRGetAllShiftsAndEmployees',
            type: 'GET'
        });
        console.log("everything loaded");
        return data;
    } catch (error) {
        console.error("Error fetching shifts: ", error);
        throw error;
    }
}

async function load_calender() {
    let calendarEl = document.getElementById('content-box');
    calendarEl.innerHTML = '';
    let shifts = [];
    let shiftData = await get_all_shifts();
    shiftData.forEach(function(shift){
       shifts.push(shift); 
    });
    console.log(shifts);
    
    // Create an event for each shift
    let events = shifts.map(shift => {
        //console.log("date format: ",  shift.shiftDate);
        //console.log("starttime format: ",  shift.startTime);
        let datePart = shift.shiftDate.substring(0,11);
        let startDateTime = `${datePart}${shift.startTime}`;
        let endDateTime = `${datePart}${shift.endTime}`;
        return {
            title: `[${shift.shiftType}]`,
            start: startDateTime,
            end: endDateTime,
            extendedProps: {
                employees: shift.employees
            }
        };
    });
    console.log("EVENTS:\n", events);

    function eventDidMount(arg) {
        let eventElement = arg.el;
        let employees = arg.event.extendedProps.employees;
        if (Array.isArray(employees)) {
            let employeeString = `Employees Assigned:\n ${employees.join('\n')}`;
            let employeeElement = document.createElement('div');
            employeeElement.classList.add('fc-event-employees');
            employeeElement.textContent = employeeString;
            eventElement.appendChild(employeeElement);
        }
    }


    let calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next,today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        events: events,
        eventDidMount: eventDidMount,
        eventClick: function(arg) {
            let employees = arg.event.extendedProps.employees;
            if (Array.isArray(employees)) {
                let employeeString = employees.join(', ');
                alert("Employees Assigned: " + employeeString);
            }
        }
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
        url: '/HR/HRShiftModification',
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
