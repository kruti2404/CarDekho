$(function () {
    // Helper to format currency with commas
    function formatCurrency(value) {
        return value.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    }

    // Initialize values from hidden fields or defaults
    var minVal = $('#minPrice').val() ? parseInt($('#minPrice').val()) : 200000;
    var maxVal = $('#maxPrice').val() ? parseInt($('#maxPrice').val()) : 200000000;

    $('#minPrice').val(minVal);
    $('#maxPrice').val(maxVal);

    // Gear rotation angles
    var gearOneAngle = 0;
    var gearTwoAngle = 0;

    $('.gear-one').css('transform', 'rotate(0deg)');
    $('.gear-two').css('transform', 'rotate(0deg)');

    // Handle gear rotation
    function handleGearRotation($slider, ui) {
        var previousVal = parseInt($slider.data('value')) || ui.value;
        $slider.data('value', ui.value);

        if (ui.values[0] === ui.value) {
            // Left handle moved
            gearOneAngle += (ui.value > previousVal) ? 7 : -7;
            $('.gear-one').css('transform', 'rotate(' + gearOneAngle + 'deg)');
        } else {
            // Right handle moved
            gearTwoAngle += (ui.value > previousVal) ? 7 : -7;
            $('.gear-two').css('transform', 'rotate(' + gearTwoAngle + 'deg)');
        }
    }

    // Initialize jQuery UI slider
    $('#slider-range').slider({
        range: true,
        min: 200000,
        max: 200000000,
        step: 100,
        values: [minVal, maxVal],
        slide: function (event, ui) {
            // Update displayed values
            $('.range').html(
                '<span class="range-value"><sup>$</sup>' + formatCurrency(ui.values[0]) + '</span>' +
                '<span class="range-divider"></span>' +
                '<span class="range-value"><sup>$</sup>' + formatCurrency(ui.values[1]) + '</span>'
            );

            // Update hidden fields for form submission
            $('#minPrice').val(ui.values[0]);
            $('#maxPrice').val(ui.values[1]);

            // Rotate gears
            handleGearRotation($(this), ui);

            // Show "+" alert if price hits threshold
            $('.range-alert').toggleClass('active', ui.values[1] === 110000);
        }
    });

    // Set initial range display
    $('.range').html(
        '<span class="range-value"><sup>$</sup>' + formatCurrency(minVal) + '</span>' +
        '<span class="range-divider"></span>' +
        '<span class="range-value"><sup>$</sup>' + formatCurrency(maxVal) + '</span>'
    );

    // Move display inside slider range area
    $('.ui-slider-range').append($('.range-wrapper'));

    // Show gears on drag
    $('.ui-slider-handle, .ui-slider-range').on('mousedown', function () {
        $('.gear-large').addClass('active');
    });

    // Hide gears on release
    $(document).on('mouseup', function () {
        $('.gear-large').removeClass('active');
    });

    // Prevent accidental drag from clicking inside display
    $('.range, .range-alert').on('mousedown', function (event) {
        event.stopPropagation();
    });
});
