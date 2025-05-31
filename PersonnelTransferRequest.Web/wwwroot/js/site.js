
// get transfer request partial with ajax
function getTransferRequestPartial() {
    $.ajax({
        url: "/TransferRequest/_GetTransferRequestPartial",
        type: "POST",
        dataType: "html",
        success: function (response) {          
            $("#newTransferRequestModal").html(response);

            //open modal if flag is set
            if ($('#openModalFlag').length && $('#openModalFlag').val() === 'true') {
                var myModal = new bootstrap.Modal($('#exampleModal')[0]);
                myModal.show();
            }

        },
        error: function () {
            const Toast = Swal.mixin({
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });
            Toast.fire({
                icon: "error",
                title: "Birşeyler ters gitti."
            });
        }
    });
}       

//call in transfer request index.cshtml
getTransferRequestPartial()

////////////////////////////////////////


let preferenceIndex = 0;
const maxPreferences = 5;

// Update the priority order numbers and labels on each preference card
function updatePriorityOrders() {
    $('#preferencesContainer .card').each(function (i, card) {
        $(card).find('input[name$=".PriorityOrder"]').val(i + 1);
        $(card).find('.preference-label').text(`Adalet Sarayı (Tercih ${i + 1})`);
    });
}

// Show or hide the Add button depending on the number of preferences
function toggleAddButton() {
    $('#addPreferenceBtn').toggle($('#preferencesContainer .card').length < maxPreferences);
}

// Create the HTML for a preference card with a select dropdown of cities
function createPreferenceCard(index) {
    const optionsCities = citiesArray.map(city => {
        const text = city === "" ? "İl Seçiniz" : city;
        return `<option value="${city}">${text}</option>`;
    }).join('');

    return `
        <div class="card p-3 mb-2 position-relative">
            <input type="hidden" name="Preferences[${index}].PriorityOrder" value="${index + 1}" />
            <div class="form-group mb-2">
                <label class="form-label preference-label">Adalet Sarayı (Tercih ${index + 1})</label>
                <select name="Preferences[${index}].CourtHouse" class="form-select select2Preference">
                    ${optionsCities}
                </select>
                <span asp-validation-for="Preferences[${index}].CourtHouse" class="text-danger"></span>
            </div>
            <button type="button" class="btn btn-danger btn-sm position-absolute top-0 end-0 m-2 remove-btn">
                <i class="bi bi-dash-lg"></i>
            </button>
        </div>
    `;
}


// Disable already selected cities in other selects to avoid duplicates
function refreshSelectOptions() {
    let selectedCities = [];

    // Collect all selected values
    $('.select2Preference').each(function () {
        const val = $(this).val();
        if (val) selectedCities.push(val);
    });

    // Loop through all selects and disable options that are already selected elsewhere
    $('.select2Preference').each(function () {
        const currentVal = $(this).val();

        $(this).find('option').each(function () {
            const optionVal = $(this).val();

            if (optionVal !== "" && optionVal !== currentVal && selectedCities.includes(optionVal)) {
                $(this).attr('disabled', 'disabled');
            } else {
                $(this).removeAttr('disabled');
            }
        });

        // Notify select2 to refresh disabled options
        $(this).trigger('change.select2');
    });
}

// Initialize select2 for all select elements that are not already initialized
function initializeSelect2() {
    $('.select2Preference').each(function () {
        if (!$(this).hasClass("select2-hidden-accessible")) {
            $(this).select2({
                placeholder: "İl Seçiniz",
                width: '100%',
                allowClear: true,
                dropdownParent: $('#exampleModalLabel') // Attach dropdown to modal container for proper positioning
            });
        }
    });
}


