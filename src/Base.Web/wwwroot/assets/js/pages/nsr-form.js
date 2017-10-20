//------------- get-in-touch.js -------------//
$(document).ready(function () {
  //validate login form
  $("#nsr-form").validate({
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
      FirstName: {
        required: true,
      },
      LastName: {
        required: true,
      },
      Email: {
        required: true,
        email: true
      },
      Address: {
        required: true,
      },
      City: {
        required: true,
      },
    },
    messages: {
      FirstName: {
        required: 'Please provide your first name',
      },
      LastName: {
        required: 'Please provide your last name',
      },
      Email: {
        required: 'Please provide your email address',
        email: 'This email is invalid'
      },
      Address: {
        required: 'Address is required',
      },
      City: {
        required: 'City is required',
      },
    },
    highlight: function (label) {
      $(label).closest('.c-form-group').removeClass('has-success').addClass('has-error');
    },
    success: function (label) {
      $(label).closest('.c-form-group').removeClass('has-error');
      label.remove();
    },
    submitHandler: function (form) {
      if ($(form).find('.form-btn').hasClass('loading')) {
        return;
      }
      $(form).find('.form-btn').addClass('loading');
      $(form).find('.form-btn').val('Loading...');
      $.ajax({
        url: '/nsr-request/post',
        type: 'POST',
        data: $(form).serialize(),
        success: function (data) {
          if (data) {
            alert('Submit successfully, we will contact you soon via your email and your phone number.');
            $(form).find('input').val('');
            $(form).find('select').val('');
            $(form).find('textarea').val('');
            $(form).find('.form-btn').removeClass('loading');
            $(form).find('.form-btn').val('Submit');
          }
        },
        error: function (data) {
          alert('error');
          $(form).find('.form-btn').removeClass('loading');
          $(form).find('.form-btn').val('Submit');
        }
      });
    }
  });
});