export function setupDatePickers(){
    $.datepicker.setDefaults( $.datepicker.regional[ $('html').attr('lang') ] );
    
    $(".datepicker").datepicker();
    $(document).on('click', '.date-time-picker', function () {
        const that = $(this);
        if (!that.hasClass('hasDatepicker')) {
            let timeFormat = $.datepicker._defaults.timeFormat;
            if (!timeFormat) {
                timeFormat = 'HH:mm';
            }
            that.datetimepicker({
                timeFormat: timeFormat
            }).blur().focus(); 
        }
    });

}