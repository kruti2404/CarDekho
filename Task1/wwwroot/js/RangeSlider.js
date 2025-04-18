$(function () {
    // Default min/max values
    var minDefault = 200000;
    var maxDefault = 200000000;

    // Get values from hidden fields or use defaults
    var minVal = $('#minPrice').val() ? parseInt($('#minPrice').val()) : minDefault;
    var maxVal = $('#maxPrice').val() ? parseInt($('#maxPrice').val()) : maxDefault;

    // Set values back to hidden fields to ensure they're initialized
    $('#minPrice').val(minVal);
    $('#maxPrice').val(maxVal);

    // Initiate jQuery UI slider
    $('#slider-range').slider({
        range: true,
        min: minDefault,
        max: maxDefault,
        step: 100,
        values: [minVal, maxVal],
        slide: function (event, ui) {
            // Format and show selected range
            $('.range').html(
                '<span class="range-value"><sup>₹</sup>' + formatCurrency(ui.values[0]) + '</span>' +
                '<span class="range-divider"></span>' +
                '<span class="range-value"><sup>₹</sup>' + formatCurrency(ui.values[1]) + '</span>'
            );

            // Update hidden inputs
            $('#minPrice').val(ui.values[0]);
            $('#maxPrice').val(ui.values[1]);

            // Toggle alert if max value hits 110000
            if (ui.values[1] === 110000) {
                $('.range-alert').addClass('active');
            } else {
                $('.range-alert').removeClass('active');
            }
        }
    });

    // Display initial range
    $('.range').html(
        '<span class="range-value"><sup>₹</sup>' + formatCurrency(minVal) + '</span>' +
        '<span class="range-divider"></span>' +
        '<span class="range-value"><sup>₹</sup>' + formatCurrency(maxVal) + '</span>'
    );

    // Move range display into slider
    $('.ui-slider-range').append($('.range-wrapper'));

    // Prevent drag interference when clicking on range display
    $('.range, .range-alert').on('mousedown', function (event) {
        event.stopPropagation();
    });
});

// Utility to format number with commas
function formatCurrency(value) {
    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
