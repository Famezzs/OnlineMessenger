function redirectToChatCreate() {
    window.location.href = window.location.href + '/CreateChatForm'
}

function setChatId(chatId) {
    $.ajax({
        type: 'POST',
        url: document.getElementById('add-new-chat').dataset.requestUrl,
        data: {
            'chatId': chatId
        }
    }).done(function (e) {
        window.location.href = window.location.href + '/ViewChat'
    })
}