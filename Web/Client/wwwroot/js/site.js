function HideModal(id) {
    $('#' + id).modal('hide')
}

function ShowModal(id) {
    $('#' + id).modal('show')
}

function ShowNavdrawer(id) {
    $('#' + id).navdrawer('show')
}

function HideNavdrawer(id) {
    $('#' + id).navdrawer('hide')
}

function ConfirmAlert(title, content, icon) {
    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            text: content,
            icon: icon,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirm',
        }).then((result) => {
            if (result.value) {
                resolve(true);
            } else {
                resolve(false);
            }
        });
    });
}

function NotifyAlert(title, icon, duration) {
    Swal.fire({
        position: 'top-end',
        icon: icon,
        title: title,
        showConfirmButton: false,
        heightAuto: false,
        timer: duration
    })
}