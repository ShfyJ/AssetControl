$('#submit').on('click', function (e) {

    e.preventDefault();

    var file = document.getElementById('TemplateFileIn');
    //var form = $(this).parents('form');
    var form = $('#template-form')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }


    if (!file.value) {
        var errorFileMsg = document.getElementById("errorFileMsg");
        //$("#display-templateFile").css('borderColor', 'red');
        errorFileMsg.classList.add("fa");
        errorFileMsg.classList.add("fa-exclamation-circle");
        errorFileMsg.style.color = "#f27474";
        $('#errorFileMsg').show();

        return false;
    }

    var validExts = new Array(".xlsx", ".xls", ".xlsm");
    var fileExt = file.value;
    fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
    if (validExts.indexOf(fileExt) < 0) {
        alert("Invalid file selected, valid files are of " +
            validExts.toString() + " types.");
        return false;
    }

    else {
        //$("#display-templateFile").css('borderColor', 'green');
    }

    $("#loader").attr('hidden', false);
    console.log(file.value);
    $.ajax({
        type: "POST",
        url: "/Admin/AssetImport/ExcelImport/",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.success) {
                $("#loader").attr('hidden', true);
                Swal.fire({
                    title: data.message,
                    icon: "success"
                });
            }

            else {
                $("#loader").attr('hidden', true);
                Swal.fire({
                    title: "Хатолик",
                    text: data.message,
                    icon: "error"
                });
            }
        }
    });

});

function getTemplateFile(file) {

    var validExts = new Array(".xlsx", ".xls", ".xlsm");
    var fileExt = file.value;
    fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
    if (validExts.indexOf(fileExt) < 0) {
        alert("Invalid file selected, valid files are of " +
            validExts.toString() + " types.");
        $('#errorFileMsg').show();
    }

    else {

        console.log(file.value);
        //var name = file.value.replace(/^.*[\\\/]/, '');
        $("#display-templateFile").val(file.value);
        $("#display-templateFile").css('borderColor', 'green');
        $('#errorFileMsg').hide();

    }
}

$('#block2').click(function () {

    if ($('#block2').prop('checked')) {
        $('#block4').prop('checked', false);
        $('#block4').attr('disabled', true);
        $('#block5').prop('checked', false);
        $('#block5').attr('disabled', true);
        $('#block6').prop('checked', false);
        $('#block6').attr('disabled', true);
        $('#block7').prop('checked', false);
        $('#block7').attr('disabled', true);

        if (!$('#lblock4').hasClass('stroked')) {
            $('#lblock4').addClass('stroked')
        }

        if (!$('#lblock5').hasClass('stroked')) {
            $('#lblock5').addClass('stroked')
        }

        if (!$('#lblock6').hasClass('stroked')) {
            $('#lblock6').addClass('stroked')
        }

        if (!$('#lblock7').hasClass('stroked')) {
            $('#lblock7').addClass('stroked')
        }


    }
    else {

        $('#block4').attr('disabled', false);
        $('#block5').attr('disabled', false);
        $('#block6').attr('disabled', false);
        $('#block7').attr('disabled', false);


        if ($('#lblock4').hasClass('stroked')) {
            $('#lblock4').removeClass('stroked')
        }

        if ($('#lblock5').hasClass('stroked')) {
            $('#lblock5').removeClass('stroked')
        }

        if ($('#lblock6').hasClass('stroked')) {
            $('#lblock6').removeClass('stroked')
        }

        if ($('#lblock7').hasClass('stroked')) {
            $('#lblock7').removeClass('stroked')
        }
    }
});

$('#block4').click(function () {

    if ($('#block4').prop('checked')) {
        $('#block2').prop('checked', false);
        $('#block2').attr('disabled', true);

        $('#block7').prop('checked', false);
        $('#block7').attr('disabled', true);

        if (!$('#lblock2').hasClass('stroked')) {
            $('#lblock2').addClass('stroked')
        }

        if (!$('#lblock7').hasClass('stroked')) {
            $('#lblock7').addClass('stroked')
        }



    }
    else {

        $('#block2').attr('disabled', false);
        $('#block7').attr('disabled', false);


        if ($('#lblock2').hasClass('stroked')) {
            $('#lblock2').removeClass('stroked')
        }

        if ($('#lblock7').hasClass('stroked')) {
            $('#lblock7').removeClass('stroked')
        }

    }

});

$('#block6').click(function () {

    if ($('#block6').prop('checked')) {
        $('#block2').prop('checked', false);
        $('#block2').attr('disabled', true);

        $('#block7').prop('checked', false);
        $('#block7').attr('disabled', true);

        if (!$('#lblock2').hasClass('stroked')) {
            $('#lblock2').addClass('stroked')
        }

        if (!$('#lblock7').hasClass('stroked')) {
            $('#lblock7').addClass('stroked')
        }



    }
    else {

        $('#block2').attr('disabled', false);
        $('#block7').attr('disabled', false);

        if ($('#lblock2').hasClass('stroked')) {
            $('#lblock2').removeClass('stroked')
        }

        if ($('#lblock7').hasClass('stroked')) {
            $('#lblock7').removeClass('stroked')
        }


    }


});

$('#block7').click(function () {

    if ($('#block7').prop('checked')) {
        $('#block2').prop('checked', false);
        $('#block2').attr('disabled', true);

        $('#block4').prop('checked', false);
        $('#block4').attr('disabled', true);

        $('#block5').prop('checked', false);
        $('#block5').attr('disabled', true);

        $('#block6').prop('checked', false);
        $('#block6').attr('disabled', true);

        if (!$('#lblock2').hasClass('stroked')) {
            $('#lblock2').addClass('stroked')
        }

        if (!$('#lblock4').hasClass('stroked')) {
            $('#lblock4').addClass('stroked')
        }

        if (!$('#lblock5').hasClass('stroked')) {
            $('#lblock5').addClass('stroked')
        }

        if (!$('#lblock6').hasClass('stroked')) {
            $('#lblock6').addClass('stroked')
        }

    }
    else {

        $('#block2').attr('disabled', false);
        $('#block4').attr('disabled', false);
        $('#block5').attr('disabled', false);
        $('#block6').attr('disabled', false);

        if ($('#lblock2').hasClass('stroked')) {
            $('#lblock2').removeClass('stroked')
        }

        if ($('#lblock4').hasClass('stroked')) {
            $('#lblock4').removeClass('stroked')
        }

        if ($('#lblock5').hasClass('stroked')) {
            $('#lblock5').removeClass('stroked')
        }

        if ($('#lblock6').hasClass('stroked')) {
            $('#lblock6').removeClass('stroked')
        }

    }

});