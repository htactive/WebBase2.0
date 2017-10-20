$(document).ready(function () {

    var bLazy = new Blazy();

    menuwidthcheck();
    $('.btn-menu-drop').click(function () {
        $(this).toggleClass('open-menu');
        $('#Hmenu-mobile').slideToggle();
    });

    $(window).on('resize', function () {
        menuwidthcheck();
    });

    $("body").on("click", ".rss_feed_item", function () {
        window.location = $(this).find("h3 a").attr("href");
        return false;
    });

    $("body").on("click", ".simple-box", function () {
        window.location = $(this).find("a").attr("href");
        return false;
    });

    ////vendor slider
    $('.vendor-slider').slick({
        infinite: true,
        slidesToShow: 5,
        slidesToScroll: 1,
        prevArrow: ".arrow-prev",
        nextArrow: ".arrow-next",        
        responsive: [
            {
                breakpoint: 1080,
                settings: {
                    slidesToShow: 4
                }
            },
          {
              breakpoint: 991,
              settings: {
                  slidesToShow: 3
              }
          },
          {
              breakpoint: 767,
              settings: {
                  slidesToShow: 2
              }
          },
          {
              breakpoint: 551,
              settings: {
                  slidesToShow: 1
              }
          }
        ]
    });
    //getInTouchValidate();
});

function menuwidthcheck() {
    var browserwidth = $(window).innerWidth();
    if (browserwidth > 991) {
        $('#Hmenu-mobile').hide();
        $('#Hmenu').show();
        $('.btn-menu-drop').removeClass('open-menu');
    }
    else if ($('.btn-menu-drop').hasClass('open-menu')) {
        $('#Hmenu-mobile').show();
    }
    if (browserwidth > 835) {
        $('#mini').removeClass('mini-bottom');
    }
    else {
        $('#mini').addClass('mini-bottom');
    }
}