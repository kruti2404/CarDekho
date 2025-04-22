// jQuery version of your Navbar and Link Active script
$(document).ready(function () {

    // Function to show or hide the navbar
    function showNavbar(toggleId, navId, bodyId, headerId) {
        const $toggle = $('#' + toggleId),
            $nav = $('#' + navId),
            $bodypd = $('#' + bodyId),
            $headerpd = $('#' + headerId);

        if ($toggle.length && $nav.length && $bodypd.length && $headerpd.length) {
            $toggle.on('click', function () {
                // Toggle navbar visibility
                $nav.toggleClass('show');
                // Change toggle icon
                $toggle.toggleClass('bx-x');
                // Add padding to body and header
                $bodypd.toggleClass('body-pd');
                $headerpd.toggleClass('body-pd');
            });
        }
    }

    // Initialize the navbar toggle
    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header');

    // Active link highlight
    $('.nav_link').on('click', function () {
        $('.nav_link').removeClass('active');
        $(this).addClass('active');
    });

});
