$(function () {
    const $multiSelect = $("#SelectedColours");

    $multiSelect.select2({
        placeholder: "Select Brands",
        allowClear: true,
    });

    const selectedValueString = $("#SelectedColoursHidden").val();
    if (selectedValueString && selectedValueString.length > 0) {
        const valuesArray = selectedValueString.split(",").map(item => item.trim());
        $multiSelect.val(valuesArray).trigger('change.select2');
    }
    //Multi select change to reflect hidden field
    $multiSelect.on('change', function () {
        const selectedBrands = $("#SelectedColours").val();
        $("#SelectedColoursHidden").val(selectedBrands ? selectedBrands.join(",") : "");
    });
});