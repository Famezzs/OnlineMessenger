function redirectToGroupCreate() {
    window.location.href = window.location.href + '/CreateGroupForm'
}

function setGroupId(groupId) {
    $.ajax({
        type: 'POST',
        url: document.getElementById('add-new-group').dataset.requestUrl,
        data: {
            'groupId': groupId
        }
    }).done(function (e) {
        window.location.href = window.location.href + '/ViewGroup'
    })
}