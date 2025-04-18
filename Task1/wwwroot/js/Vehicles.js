$(document).ready(function () {

    // --- Modal Opening Logic ---
    $('.open-modal').click(function (e) {
        e.preventDefault(); // Prevent default link behavior

        // Get the vehicle ID from the data attribute
        var vehicleId = $(this).data('vehicleid');
        console.log('Vehicle id clicked:', vehicleId);

        // Get references to the modal and its body
        var modal = $('#vehicleDetailsModal');
        var modalBody = modal.find('.modal-body');

        // Clear previous content (optional, good practice)
        modalBody.html('<p>Loading details...</p>'); // Show a loading message

        // Show the modal
        modal.modal('show');

        // Make AJAX request to get vehicle details
        $.get('@Url.Action("Details", "Vehicles")', { id: vehicleId })
            .done(function (data) {
                // Success: Load the returned HTML into the modal body
                modalBody.html(data);
            })
            .fail(function () {

                modalBody.html('<p class="text-danger">Error loading vehicle details. Please try again later.</p>');
                console.error('AJAX request failed for vehicle details:', vehicleId);
            });
    });



    // Initialize Multi-Select Filter (Select2)
    var multiSelectFilter = $("#MultiSelectFilter");

    if (multiSelectFilter.length) { // Check if the element exists
        // Set initial values from the Model (runs on page load)
        const selectedInitialBrands = "@Html.Raw(Model.Query.MultiFilter)";
        console.log('Initial MultiFilter value from Model:', selectedInitialBrands);

        if (selectedInitialBrands) {
            const values = selectedInitialBrands.split(",");
            console.log('Setting initial MultiSelect values:', values);
            multiSelectFilter.val(values);
        }

        // Initialize Select2
        multiSelectFilter.select2({
            placeholder: "Select Brands",
            allowClear: true
        });

        // Ensure initial values are fully processed by Select2 after initialization
        if (selectedInitialBrands) {
            multiSelectFilter.trigger('change.select2');
        }
    }


    // Handle Filter Form Submission
    $("#filterForm").on("submit", function () {
        // 1. Update Hidden Field for Multi-Select Brands
        var selectedBrands = $("#MultiSelectFilter").val();
        $("#MultiFilterHidden").val(selectedBrands ? selectedBrands.join(",") : "");

        // 2. Update Hidden Field for Selected Colors
        const selectedColours = [];
        $(".color-checkbox:checked").each(function () {
            selectedColours.push($(this).val());
        });
        $("#ColoursListHidden").val(selectedColours.join(","));


        $("#SearchTerm").val($("#SearchBox").val());

    });

}); 