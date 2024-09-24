$(document).ready(function () {
    $('#registerUser').submit(function (event) {
        var password = $('#password').val();
        var confirmPassword = $('#confirmPassword').val();

        if (password !== confirmPassword) {
            event.preventDefault();
            $('#confirmPassword').addClass("is-invalid");
        }
        else {
            $('#confirmPassword').removeClass("is-invalid");
        }
    });
});