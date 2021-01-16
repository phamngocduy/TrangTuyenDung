$(document).ready(function () {
    //-- Check Navbar extend Seach-heading
    headerExtendSearchSection();
    function headerExtendSearchSection() {
        const withSearching = $('body#header_searching').length >= 1;
        const navbar = $('nav.navbar');

        if (withSearching) {
            navbar.addClass('navbar_absolute');
        }
        else {
            navbar.addClass('bg-dark');
        }
    }

    //-- Navbar fixed when scrolling
    function navbarScrolling() {
        const navbar = $('nav.navbar');

        if ($(window).scrollTop() >= 200) {
            navbar.addClass('fixed');
        } else {
            navbar.removeClass("fixed");
        }
    }

    $(window).on('scroll', function () {
        navbarScrolling();
    });
});
