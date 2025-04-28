// Add this to initialize Select2 in Blazor
window.initializeSelect2 = function () {
    // Check if jQuery and Select2 are available
    if (typeof $ !== 'undefined' && typeof $.fn.select2 !== 'undefined') {
        // Initialize Select2 on all elements with select2 class
        try {
            $('.select2').select2({
                placeholder: "Select players for this tournament",
                width: '100%'
            });
            console.log('Select2 initialized successfully');
        } catch (e) {
            console.error('Error initializing Select2:', e);
        }
    } else {
        console.warn('jQuery or Select2 is not loaded. Select2 cannot be initialized.');
    }
}