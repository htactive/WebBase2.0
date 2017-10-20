//------------- get-in-touch.js -------------//
$(document).ready(function () {
  //validate login form
  $("#get-in-touch-form").validate({
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
      Name: {
        required: true,
      },
      Phone: {
        required: true,
      },
      Email: {
        required: true,
        email: true
      }
    },
    messages: {
      Name: {
        required: "Please provide your name",
      },
      Phone: {
        required: "Please provide your phone",
      },
      Email: {
        required: "Please provide your email",
        email: "Invalid email"
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
        url: '/user-feedback/post',
        type: 'POST',
        data: $(form).serialize(),
        success: function (data) {
          if (data) {
            alert('Submit successfully, we will contact you soon via your email and your phone number.');
            $(form).find('.c-form-control').val('');
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