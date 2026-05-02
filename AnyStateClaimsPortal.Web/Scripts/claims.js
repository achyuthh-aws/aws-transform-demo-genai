$(document).ready(function () {
    // Table row hover highlight
    $('table tbody tr').hover(
        function () { $(this).addClass('info'); },
        function () { $(this).removeClass('info'); }
    );

    // Form submit: validate denial reason when status is Denied
    $('form').on('submit', function () {
        var status = $('#Status').val() || $('#NewStatus').val();
        if (status === 'Denied') {
            var reason = $('#DenialReason').val();
            if (!reason || reason.trim() === '') {
                alert('A denial reason is required when denying a claim.');
                return false;
            }
        }
        return true;
    });

    // Date input: validate not in future
    $('input[type="date"]').on('change', function () {
        var selected = new Date($(this).val());
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        if (selected > today) {
            alert('Date cannot be in the future.');
            $(this).val('');
        }
    });

    // Status change: show/hide denial reason field
    $('#Status, #NewStatus').on('change', function () {
        var isDenied = $(this).val() === 'Denied';
        $('#DenialReasonGroup').toggle(isDenied);
    });

    // Confirm before ProcessBatch
    $('#ProcessBatch').on('click', function () {
        return confirm('Are you sure you want to process this batch?');
    });

    // Auto-dismiss alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Format currency inputs on blur
    $('input.currency').on('blur', function () {
        var val = parseFloat($(this).val());
        if (!isNaN(val)) {
            $(this).val(val.toFixed(2));
        }
    });
});
