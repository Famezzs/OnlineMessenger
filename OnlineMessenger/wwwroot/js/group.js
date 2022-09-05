let scrollPosition = window.sessionStorage.getItem("sidebar-scroll");

let currentMessageId = "";

const contextMenu = document.querySelector('.context-menu');

const replyMenu = document.getElementById('reply-menu');

const replyButton = document.getElementById('reply');

const replyPrivatelyButton = document.getElementById('reply-privately');

const sendMessageMenu = document.querySelector('.message-menu');

const sendMessageMenuSubmit = document.getElementById('message-menu-submit');

const inviteForm = document.querySelector('.universal-form');

const manageUsersFormInput = document.getElementById('manage-users-input');

const inviteSubmitButton = document.getElementById('invite-submit-button');

const inviteButton = document.querySelector('.manage-users-button');

const closeManageUsersMenu = document.getElementById('close-manage-users-menu');

const editMessageMenu = document.querySelector('.edit-message-menu');

const editMessageButton = document.querySelector('.context-menu__editbtn');

const editMessageInput = document.getElementById('edit-message-input');

const deleteForAllButton = document.querySelector('.context-menu__delforevrbtn');

const deleteForSelfButton = document.querySelector('.context-menu__delbtn');

const errorNotification = document.querySelector('.universal-error-message');

const removeUserButton = document.querySelector('.remove-user-button');

const memberList = document.getElementById('member-list');

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

function showReplyMenu(show = true) {
    replyMenu.style.display = show ? 'block' : 'none';
}

function showSendMessageMenu(show = true) {
    sendMessageMenu.style.display = show ? 'inline-block' : 'none';
}

function showEditMessageMenu(show = true) {
    editMessageMenu.style.display = show ? 'inline-block' : 'none';
}

function showInviteForm(show = true) {

    if (!show) {

        manageUsersFormInput.value = null;

        inviteForm.style.display = show ? 'table' : 'none';

    } else {

        $.ajax({
            type: 'GET',
            url: inviteForm.dataset.requestUrl

        }).done((result) => {

            memberList.innerHTML = result;

            inviteForm.style.display = show ? 'table' : 'none';
        });
    }
}

function showErrorNotification(show = true) {
    errorNotification.style.display = show ? 'block' : 'none';
}

function configureContextMenu(event) {

    showReplyMenu(false);

    showContextMenu();

    contextMenu.style.top = event.pageY + "px";

    contextMenu.style.left = event.pageX + "px";

    currentMessageId = event.target.id;
}

function configureReplyMenu(event) {

    showContextMenu(false);

    showReplyMenu();

    replyMenu.style.top = event.pageY + "px";

    replyMenu.style.left = event.pageX + "px";

    currentMessageId = event.target.id;
}

function refreshMessages() {

    saveScroll();

    location.reload();
}

function switchSubmitMessageButton() {
    sendMessageMenuSubmit.value = "Reply";

    sendMessageMenuSubmit.style.backgroundColor = 'blue';

    sendMessageMenuSubmit.style.color = 'white';

    sendMessageMenuSubmit.style.border = '1px solid black';

    sendMessageMenuSubmit.style.borderRadius = '2px';
}

window.addEventListener('contextmenu', (event) => {
    event.preventDefault();
});

window.addEventListener('click', () => {
    showContextMenu(false);
    showReplyMenu(false);
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
});

inviteButton.addEventListener('click', () => {
    showInviteForm();
});

inviteSubmitButton.addEventListener('click', () => {

    saveScroll();

    $.ajax({
        type: 'POST',
        url: inviteSubmitButton.dataset.requestUrl,
        data: {
            'email': manageUsersFormInput.value
        }
    }).done((error) => {

        if (error) {

            errorNotification.innerHTML = error;

            showErrorNotification();
        } else {

            alert('Success!');

            errorNotification.innerHTML = null;

            showErrorNotification(false);

            showInviteForm(false);
        }
    });
});

replyButton.addEventListener('click', () => {

    saveScroll();

    switchSubmitMessageButton();

    $.ajax({
        type: 'POST',
        url: replyButton.dataset.requestUrl,
        data: {
            'messageId': currentMessageId,
            'replyMode': 'pub'
        }
    }).done(() => {

    });
});

replyPrivatelyButton.addEventListener('click', () => {

    saveScroll();

    switchSubmitMessageButton();

    $.ajax({
        type: 'POST',
        url: replyPrivatelyButton.dataset.requestUrl,
        data: {
            'messageId': currentMessageId,
            'replyMode': 'pri'
        }
    }).done(() => {

    });
});

closeManageUsersMenu.addEventListener('click', () => {
    showInviteForm(false);
});

if (removeUserButton != null) {
    removeUserButton.addEventListener('click', () => {

        saveScroll();

        $.ajax({
            type: 'POST',
            url: removeUserButton.dataset.requestUrl,
            data: {
                'email': manageUsersFormInput.value
            }
        }).done((error) => {

            if (error) {

                errorNotification.innerHTML = error;

                showErrorNotification();
            } else {

                alert('Success!');

                errorNotification.innerHTML = null;

                showErrorNotification(false);

                showInviteForm(false);
            }
        });
    });
}