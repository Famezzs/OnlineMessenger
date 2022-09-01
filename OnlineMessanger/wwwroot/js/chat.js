function redirectToChatCreate() {
    window.location.href = window.location.href + '/CreateChatForm'
}

function setChatId(chatId) {
    $.ajax({
        url: '@Url.Action("GetLinkHandler","Storage")'
    }).done(function (e)
}