let scrollPosition = window.sessionStorage.getItem("sidebar-scroll");

let currentMessageId = "";

const contextMenu = document.querySelector('.context-menu');

const sendMessageMenu = document.querySelector('.message-menu');

const editMessageMenu = document.querySelector('.edit-message-menu');

const editMessageButton = document.querySelector('.context-menu__editbtn');

const editMessageInput = document.getElementById('edit-message-input');

const deleteForAllButton = document.querySelector('.context-menu__delforevrbtn');

const deleteForSelfButton = document.querySelector('.context-menu__delbtn');

function saveScroll() {
    window.sessionStorage.setItem("sidebar-scroll", window.scrollY);
}

if (scrollPosition !== null) {
    autoScrollTo(scrollPosition);
}

function autoScrollTo(scrollPosition) {

    document.documentElement.style.scrollBehavior = 'auto';

    setTimeout(() => window.scrollTo(0, scrollPosition), 5);
    setTimeout(() => document.documentElement.style.scrollBehavior = 'smooth', 5);
}

function showContextMenu(show = true) {
    contextMenu.style.display = show ? 'block' : 'none';
}

function showSendMessageMenu(show = true) {
    sendMessageMenu.style.display = show ? 'block' : 'none';
}

function showEditMessageMenu(show = true) {
    editMessageMenu.style.display = show ? 'block' : 'none';
}

function configureContextMenu(event) {

    showContextMenu();

    contextMenu.style.top = event.pageY + "px";

    contextMenu.style.left = event.pageX + "px";

    currentMessageId = event.target.id;
}

function refreshMessages() {

    saveScroll();

    location.reload();
}

window.addEventListener('contextmenu', (event) => {
    event.preventDefault();
});

window.addEventListener('click', () => {
    showContextMenu(false);
});

editMessageButton.addEventListener('click', () => {

    $.ajax({
        type: 'POST',
        url: editMessageButton.dataset.requestUrl,
        data: {
            'messageId': currentMessageId
        }
    }).done((result) => {
        showSendMessageMenu(false);

        editMessageInput.value = result;

        showEditMessageMenu(true);
    });
})

deleteForAllButton.addEventListener('click', () => {

    saveScroll();

    $.ajax({
        type: 'POST',
        url: deleteForAllButton.dataset.requestUrl,
        data: {
            'messageId': currentMessageId
        }
    }).done(() => {
        refreshMessages();
    });
});

deleteForSelfButton.addEventListener('click', () => {

    saveScroll();

    $.ajax({
        type: 'POST',
        url: deleteForSelfButton.dataset.requestUrl,
        data: {
            'messageId': currentMessageId
        }
    }).done(() => {
        refreshMessages();
    });
})