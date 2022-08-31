const loginform = document.getElementById('login-form');
const username = document.getElementById('username');
const password = document.getElementById('password');

loginform.addEventListener('submit', (e) => {

    e.preventDefault();

    var request = $.ajax({
        url: '/api/Authenticate/login',
        contentType: "application/json; charset=utf-8",
        data: { 'email': username.value, 'password': password.value },
        type: 'POST',
        cache: false,
        success: function (result) {
            alert(result);
        }
    });

    request.fail(function (jqXHR, textStatus) {
        alert("Request failed: " + textStatus);
    });
});
