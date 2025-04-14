$(function () {
	const selected = "@Html.Raw(Model.Query.MultiFilter)";
	if (selected) {
		const values = selected.split(",");
		$("#MultiSelectFilter").val(values).trigger("change");
	}

	$("#MultiSelectFilter").select2({
		placeholder: "Select Brands",
		allowClear: true
	});

	// Multi-select hidden value
	$("#filterForm").on("submit", function () {
		var selectedBrands = $("#MultiSelectFilter").val();
		$("#MultiFilterHidden").val(selectedBrands ? selectedBrands.join(",") : "");

		// Search text
		$("#SearchTerm").val($("#SearchBox").val());
	});


});
