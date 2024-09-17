$(document).ready(function() {
    $('#notie-success').on('click', function() {
        //notie.alert(1, 'Success!', 2);
        $.notieSuccess('Success!');
    });

    $('#notie-warning').on('click', function() {
        //notie.alert(2, 'Warning<br><b>with</b><br><i>HTML</i><br><u>included.</u>', 2);
        $.notieWarning('Warning<br><b>with</b><br><i>HTML</i><br><u>included.</u>');
    });

    $('#notie-error').on('click', function() {
        //notie.alert(3, 'Error.', 2);
        $.notieError('Error.');
    });

    $('#notie-info').on('click', function() {
        //notie.alert(4, 'Information.', 2);
        $.notieInfo('Information.');
    });

    $('#notie-confirm').on('click', function() {
        notie.confirm('Are you sure you want to do that?<br><b>That\'s a bold move...</b>', 'Yes', 'Cancel', function() {
            notie.alert(1, 'Good choice!', 2);
        });
    });

    $('#notie-input').on('click', function() {
        notie.input('Please enter your email address:', 'Submit', 'Cancel', 'email', 'name@example.com', function(value_entered) {
            notie.alert(1, 'You entered: ' + value_entered, 2);
        });
    });

    jQuery.notieSuccess = function notieSuccess(val) {
        notie.alert(1, val, 2);
    }

    jQuery.notieWarning = function notieWarning(val) {
        notie.alert(2, val, 2);
    }

    jQuery.notieError = function notieError(val) {
        notie.alert(3, val, 2);
    }

    jQuery.notieInfo = function notieInfo(val) {
        notie.alert(4, val, 2);
    }

});
