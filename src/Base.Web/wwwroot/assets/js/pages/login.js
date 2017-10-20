//------------- login.js -------------//
$(document).ready(function () {
    $.validator.addMethod('confirmPassword', function (value, element, param) {
        return value === param; // return bool here if valid or not.
    }, 'Your error message!');


    //validate login form 
    $("#login-form").validate({
        ignore: null,
        ignore: 'input[type="hidden"]',
        errorPlacement: function (error, element) {
            var place = element.closest('.input-group');
            if (!place.get(0)) {
                place = element;
            }
            if (place.get(0).type === 'checkbox') {
                place = element.parent();
            }
            if (error.text() !== '') {
                place.after(error);
            }
        },
        errorClass: 'help-block',
        rules: {
            UserName: {
                required: true,
            },
            Password: {
                required: true,
                minlength: 5
            }
        },
        messages: {
            Password: {
                required: "M?t kh?u kh�ng ???c ?? tr?ng",
                minlength: "M?t kh?u �t nh?t 5 k� t?"
            },
            UserName: {
                required: "T�n ??ng nh?p kh�ng ???c ?? tr?ng",
            },
        },
        highlight: function (label) {
            $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function (label) {
            $(label).closest('.form-group').removeClass('has-error');
            label.remove();
        }
    });

});