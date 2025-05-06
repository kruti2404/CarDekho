$(function () {

    //FiltterForm Submission 
    $("#FilterBtn").on('click', function (e) {
        e.preventDefault();

        SendRequest("Vehicles/Index");
    });

    $("#RemoveFilter").on('click', function (e) {
        e.preventDefault();
        console.log("Remove Filter is applied");
        window.location.href = '/Vehicles';

    });

    //Sorting filtter handle
    $(document).on("click", ".sorting-filter", function () {

        const column = $(this).data("sort-column");
        let direction = $("#SortDirection").val() || "ASC";

        const currentSortColumn = $("#SortColumn").val();
        if (currentSortColumn === column) {
            direction = (direction === "ASC") ? "DESC" : "ASC";
        } else {
            direction = "ASC";
        }

        $("#SortColumn").val(column);
        $("#SortDirection").val(direction);

        SendRequest("Vehicles/Index");


    });

    //SearchFilter submission
    $("#SearchFilter").on("click", function (e) {
        console.log("The search is entered ");
        e.preventDefault();

        SendRequest("Vehicles/Index");
    });

    // initialization function
    function initializeFilters() {
        setupFormSubmission();
        setupModalHandling();
        updateColorsHidden();

    }

    // Handle form submission preparation
    function setupFormSubmission() {
        const $multiSelect = $("#MultiSelectFilter");

        $multiSelect.select2({
            placeholder: "Select Brands",
            allowClear: true,
        });

        const selectedValueString = $("#MultiFilterHidden").val();
        if (selectedValueString && selectedValueString.length > 0) {
            const valuesArray = selectedValueString.split(",").map(item => item.trim());
            $multiSelect.val(valuesArray).trigger('change.select2');
        }
        //Multi select change to reflect hidden field
        $multiSelect.on('change', function () {
            const selectedBrands = $("#MultiSelectFilter").val();
            $("#MultiFilterHidden").val(selectedBrands ? selectedBrands.join(",") : "");
        });

        // hidden field on checkbox change
        $('.color-checkbox').on('change', function () {
            updateColorsHidden();
        });
    }

    // Update colors hidden field
    function updateColorsHidden() {
        const selectedColours = [];
        $(".color-checkbox:checked").each(function () {
            selectedColours.push($(this).val());
        });
        $("#ColoursListHidden").val(selectedColours.join(","));
    }

    // Handle modal interactions
    function setupModalHandling() {
        $(document).on('click', '.open-modal', function (e) {
            e.preventDefault();
            const vehicleId = $(this).data('vehicleid');
            openVehicleModal(vehicleId);
        });
    }
    function openVehicleModal(vehicleId) {
        const modal = $('#vehicleDetailsModal');
        const modalBody = modal.find('.modal-body');

        modalBody.html('<p>Loading details...</p>');
        modal.modal('show');

        sendAjaxRequest("Vehicles/Details", "GET", { id: vehicleId }, function (data) {
            setTimeout(function () {
                modalBody.html(data);
            }, 1000);

        }, function () {
            setTimeout(function () {
                modalBody.html('<p class="text-danger">Error loading vehicle details.</p>');
            }, 1000);
        });
    }

    function QueryData() {
        const queryData = {
            "PageSize": parseInt($("input[name='Query.PageSize']").val()) || 10,
            "SearchTerm": $("#SearchHolder").val() || "",
            "SingleFilter": $("select[name='Query.SingleFilter']").val() || "",
            "MultiFilter": $("#MultiFilterHidden").val() || "",
            "StockAvail": $("input[name='Query.StockAvail']:checked").val() || "",
            "ColoursList": $("#ColoursListHidden").val() || "",
            "MinPrice": parseInt($("#minPrice").val()) || null,
            "MaxPrice": parseInt($("#maxPrice").val()) || null,
            "Rating": parseInt($("#RatingValue").val()) || null,
            "SortColumn": $("#SortColumn").val() || "",
            "SortDirection": $("#SortDirection").val() || "ASC",
        };
        return queryData;
    }
    // AJAX Request
    function sendAjaxRequest(url, method = "GET", data = {}, successCallback = null, errorCallback = null) {
        console.log("The ajax is call for the url ", url);
        console.log("SearchTerm is :", data.SearchTerm);

        $.ajax({
            url: url,
            type: method,
            data: data,
            success: function (response) {
                console.log("AJAX data fetched successfully.");
                if (typeof successCallback === 'function') successCallback(response);
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
                console.error("Response Text:", xhr.responseText);
                if (typeof errorCallback === 'function') errorCallback(xhr, status, error);
            }

        });
    }

    function buildQueryString(params) {
        return Object.keys(params)
            .filter(key => params[key] !== null && params[key] !== "" && params[key] !== undefined)
            .map(key => encodeURIComponent(key) + '=' + encodeURIComponent(params[key]))
            .join('&');
    }

    function SendRequest(url) {
        const queryData = QueryData();
        const queryString = buildQueryString(queryData);

        //Update URL in browser
        const newUrl = window.location.pathname + '?' + queryString;
        window.history.pushState(null, '', newUrl);
        $("body").addClass("loading");


        sendAjaxRequest(url, "GET", queryData, function (data) {
            console.log("The data is loaded", data);
            $("#vehicleResultsContainer").html(data);

            $("body").removeClass("loading");
        }, function () {
            console.log("Something went wrong");

            $("body").removeClass("loading");
        });
    }

    // Init on ready
    initializeFilters();
});
