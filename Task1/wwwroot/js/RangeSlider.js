$(function () {
    // Initialize values from hidden fields or defaults
    var minVal = $('#minPrice').val() ? parseInt($('#minPrice').val()) : 200000;
    var maxVal = $('#maxPrice').val() ? parseInt($('#maxPrice').val()) : 200000000;

    $('#minPrice').val(minVal);
    $('#maxPrice').val(maxVal);

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
                '<span class="range-value"><sup>₹</sup>' + formatCurrency(ui.values[0]) + '</span>' +
                '<span class="range-divider"></span>' +
                '<span class="range-value"><sup>₹</sup>' + formatCurrency(ui.values[1]) + '</span>'
            );

            // Update hidden fields for form submission
            $('#minPrice').val(ui.values[0]);
            $('#maxPrice').val(ui.values[1]);

            // Show "+" alert if price hits threshold
            $('.range-alert').toggleClass('active', ui.values[1] === 110000);
        }
    });

    // Set initial range display
    $('.range').html(
        '<span class="range-value"><sup>₹</sup>' + formatCurrency(minVal) + '</span>' +
        '<span class="range-divider"></span>' +
        '<span class="range-value"><sup>₹</sup>' + formatCurrency(maxVal) + '</span>'
    );

    // Move display inside slider range area
    $('.ui-slider-range').append($('.range-wrapper'));

    // Prevent accidental drag from clicking inside display
    $('.range, .range-alert').on('mousedown', function (event) {
        event.stopPropagation();
    });
});
