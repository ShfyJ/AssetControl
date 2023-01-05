$(document).ready(function () {
    $('.cellGroupOne').prop('checked', true);
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust().responsive.recalc();
    });

    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
    $('[data-bs-toggle="tooltip"]').tooltip();

    $('#orgId').val($("#org").val());
    $("input").attr("autocomplete", "off");

});

var defaultTable;


$(document).ready(function () {
    defaultTable = $('#myTable').DataTable({
        processing: true,  
        responsive: {
            details: {
                renderer: function (api, rowIdx, columns) {
                    var data = $.map(columns, function (col, i) {
                        return col.hidden ?
                            '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
                            '<td>' + col.title + ':' + '</td> ' +
                            '<td>' + col.data + '</td>' +
                            '</tr>': ""
                            
                    }).join('');

                    return data ?
                        $('<table/>').append(data) :
                        false;
                }
            }
        
        },


        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },

        scrollX: false,
        "dom": '<lf<"toolbar"><rt>ip>',

        columnDefs: [{
            className: 'dtr-control',
            orderable: false,
            targets: 0
        }],
        order: [1, 'asc'],

        "ajax": {
            "url": '/Admin/RealEstates/GetConfirmed/'
        },
        autoWidth: false, 
        columnDefs: [
            { width: '20%', targets: 0 }, //step 2, column 1 out of 4
            { width: '10%', targets: 1 }, //step 2, column 2 out of 4
            { width: '10%', targets: 2 }, //step 2, column 3 out of 4
            { width: '15%', targets: 3 }, //step 2, column 1 out of 4
            { width: '10%', targets: 4 }, //step 2, column 2 out of 4
            { width: '15%', targets: 5 }, //step 2, column 3 out of 4
            { width: '20%', targets: 6, className: 'dt-body-center' }, //step 2, column 3 out of 4
        ],
        
        
        "columns": [
            { "data": "realEstateName" },
            { "data": "cadastreNumber" },
            /*{ "data": "cadastreRegDate" },*/
            { "data": "commisioningDate" },
            { "data": "activity" },
            { "data": "region" },
            { "data": "assetHolderName" },
            
            {
                "data": {},
                "render": function (data) {
                    var fdata = {
                        cadFileLink : data.cadastreFileLink,
                        photo1Link : data.photo1Link,
                        photo2Link : data.photo2Link,
                        photo3Link : data.photo3Link
                    };

                    var fdataStr = encodeURIComponent(JSON.stringify(fdata));

                    var rdata = {
                        realEstateId: data.realEstateId,
                        realEstateName: data.realEstateName,                        
                    }

                    var rDataStr = encodeURIComponent(JSON.stringify(rdata));

                    return `
                            <div >
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="масул шахс" onclick=getUser('${rDataStr}')  class="userC" style="cursor:pointer; width:150px">
                                    <i class="fa fa-user"></i> &nbsp &nbsp
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="батафсил маълумот" onclick=inDetail('${data.realEstateId}')  class="detailC" style="cursor:pointer; width:150px">
                                    <i class="fa fa-reorder"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ҳужжатлар ва расмлар" onclick=showFiles('${fdataStr}')  class="filesC" style="cursor:pointer; width:150px">
                                    <i class="fa fa-folder-open"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="таҳрирлаш" onclick=Edit('${data.realEstateId}')  class="editC" style="cursor:pointer; width:150px">
                                    <i class="fas fa-edit"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" onclick=Delete("/Admin/RealEstates/Delete/${data.realEstateId}") class="deleteC" style="cursor:pointer; width:120px">
                                    <i class="fas fa-trash-alt"></i>  &nbsp &nbsp
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="паспорт" onclick=GetPassport("${data.realEstateId}") class="passC" style="cursor:pointer; width:120px">
                                    <i class="fa fa-file-powerpoint-o"></i>  &nbsp 
                                </a>
                                
                            </div>
                        `

                }
            },
        ],

        "language": {
            "processing": "<span class='fa-stack fa-lg'>\n\
                            <i class='fa fa-spinner fa-spin fa-stack-2x fa-fw'></i>\n\
                       </span>&emsp;Кутиляпти ...",
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "Саҳифа _PAGE_ / _PAGES_",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "searchPlaceholder": "Қидирув..",
            "sSearch": "",
        },
        "lengthMenu": [10,20, 30, 40, 50]
    });

    $('div.toolbar').attr('id', 'refreshMain');

    $('#refreshMain').html('<i class="icofont icofont-refresh"></i>');


    $('#refreshMain').attr('data-bs-toggle', 'tooltip');
    $('#refreshMain').attr('data-bs-placement', 'top');
    $('#refreshMain').attr('data-bs-original-title', 'янгилаш');

    $('#refreshMain').css('margin-top', '5px');
    $('#refreshMain').css('background-color', 'white');
    $('#refreshMain').css('color', '#6c757d');
    $('#refreshMain').css('cursor', 'pointer');

    $('#refreshMain').mouseover(function () {
        $('#refreshMain').css('border-bottom-color', 'green');
        $('#refreshMain').css('color', 'green');
    });

    $('div.toolbar').mouseout(function () {
        $('#refreshMain').css('background-color', 'white');
        $('#refreshMain').css('color', '#6c757d');
    });

    $('[data-bs-toggle="tooltip"]').tooltip();

    $('#refreshMain').click(function () {
        defaultTable.ajax.reload();
    });

});


$(document).ready(function () {

    $("#org, #region, #balance, #notBalance").on("change", function () {

        var orgId = $("#org").val();
        var regionId = $("#region").val();
        var isBalance = document.getElementById("balance").checked;
        var isNotBalance = document.getElementById("notBalance").checked;

        ChangeMainValuesOfModal(orgId, regionId, isBalance, isNotBalance);

        $('#orgId').val(orgId);
        if (orgId != "") {
            var data = {
                orgId: orgId,
                regionId: regionId,
                balance: isBalance,
                notBalance: isNotBalance,
            }

           
            defaultTable = $('#myTable').DataTable({
                "destroy": true,
                processing: true,
                // processing: true,
                responsive: {
                    details: {
                        renderer: function (api, rowIdx, columns) {
                            var data = $.map(columns, function (col, i) {
                                return col.hidden ?
                                    '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
                                    '<td>' + col.title + ':' + '</td> ' +
                                    '<td>' + col.data + '</td>' +
                                    '</tr>' :
                                    '';
                            }).join('');

                            return data ?
                                $('<table/>').append(data) :
                                false;
                        }
                    }

                },
                scrollX: false,
                "dom": 'lfr<"toolbar"><"toolbar">tip',

                columnDefs: [{
                    className: 'dtr-control',
                    orderable: false,
                    targets: 0
                }],
                order: [1, 'asc'],

                "ajax": {
                    "url": '/Admin/RealEstates/GetFilteredData/',
                    "type": "POST",

                    "data": function () {
                        return JSON.stringify(data);
                    },
                    "contentType": "application/json",

                },
                autoWidth: false,
                columnDefs: [
                    { width: '20%', targets: 0 }, //step 2, column 1 out of 4
                    { width: '10%', targets: 1 }, //step 2, column 2 out of 4
                    { width: '10%', targets: 2 }, //step 2, column 3 out of 4
                    { width: '15%', targets: 3 }, //step 2, column 1 out of 4
                    { width: '10%', targets: 4 }, //step 2, column 2 out of 4
                    { width: '15%', targets: 5 }, //step 2, column 3 out of 4
                    { width: '20%', targets: 6, className: 'dt-body-center' }, //step 2, column 3 out of 4
                ],

                "columns": [
                    { "data": "realEstateName" },
                    { "data": "cadastreNumber" },
                    /*{ "data": "cadastreRegDate" },*/
                    { "data": "commisioningDate" },
                    { "data": "activity" },
                    { "data": "region" },
                    { "data": "assetHolderName" },


                    {
                        "data": {},
                        "render": function (data) {
                            var fdata = {
                                cadFileLink: data.cadastreFileLink,
                                photo1Link: data.photo1Link,
                                photo2Link: data.photo2Link,
                                photo3Link: data.photo3Link
                            };

                            var fdataStr = encodeURIComponent(JSON.stringify(fdata));

                            var rdata = {
                                realEstateId: data.realEstateId,
                                realEstateName: data.realEstateName,
                            }

                            var rDataStr = encodeURIComponent(JSON.stringify(rdata));

                            return `
                            <div >
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="масул шахс" onclick=getUser('${rDataStr}')  class="userC" style="cursor:pointer; width:150px">
                                    <i class="fa fa-user"></i> &nbsp &nbsp
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="батафсил маълумот" onclick=inDetail('${data.realEstateId}')  class="detailC" style="cursor:pointer; width:150px">
                                    <i class="fa fa-reorder"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ҳужжатлар ва расмлар" onclick=showFiles('${fdataStr}')  class="filesC" style="cursor:pointer; width:150px">
                                    <i class="fa fa-folder-open"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="таҳрирлаш" onclick=Edit('${data.realEstateId}')  class="editC" style="cursor:pointer; width:150px">
                                    <i class="fas fa-edit"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" onclick=Delete("/Admin/RealEstates/Delete/${data.realEstateId}") class="deleteC" style="cursor:pointer; width:120px">
                                    <i class="fas fa-trash-alt"></i>  &nbsp &nbsp
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="паспорт" onclick=GetPassport("${data.realEstateId}") class="passC" style="cursor:pointer; width:120px">
                                    <i class="fa fa-file-powerpoint-o"></i>  &nbsp 
                                </a>
                                
                            </div>
                        `

                        }
                    },
                ],

                "language": {
                    "processing": "<span class='fa-stack fa-lg'>\n\
                            <i class='fa fa-spinner fa-spin fa-stack-2x fa-fw'></i>\n\
                       </span>&emsp;Кутиляпти ...",
                    "lengthMenu": "Кўрсатинг _MENU_ ",
                    "zeroRecords": "Ҳеч нима топилмади - узур",
                    "info": "Саҳифа _PAGE_ / _PAGES_",
                    "infoEmpty": "",
                    "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
                    "oPaginate": {
                        "sFirst": "Биринчи",
                        "sLast": "Охирги",
                        "sNext": "Кейинги",
                        "sPrevious": "Аввалги"
                    },
                    "searchPlaceholder": "Қидирув..",
                    "sSearch": "",
                },
                "lengthMenu": [10, 20, 30, 40, 50]
            });

            $('div.toolbar:first').attr('id', 'bulkDelete');
            $('div.toolbar:last').attr('id', 'refresh');
            $("#bulkDelete").attr("hidden", true);

            if (orgId != "0") {
                $("#bulkDelete").attr("hidden", false);
            }

            var orgName = $('#org option:selected').text();

            $('#bulkDelete').html('<i class="fa fa-trash-o fa-lg" ></i>');
            $('#refresh').html('<i class="fa fa-refresh fa-lg"></i>');
           
            
            $('#bulkDelete,#refresh').attr('data-bs-toggle', 'tooltip');
            $('#bulkDelete,#refresh').attr('data-bs-placement', 'top');
            $('#bulkDelete').attr('data-bs-original-title', orgName + ' нинг барча объектларини ўчириш');
            $('#refresh').attr('data-bs-original-title', 'янгилаш');

            $('div.toolbar').css('margin-top', '5px');
            $('#bulkDelete').css('border-bottom-style', 'solid');
            $('#bulkDelete').css('border-bottom-color', '#6c757d');
            $('div.toolbar').css('background-color', 'white');
            $('div.toolbar').css('color', '#6c757d');
            $('div.toolbar').css('cursor', 'pointer');
            $('#bulkDelete').mouseover(function () {
                $('#bulkDelete').css('border-bottom-color', 'red');
                $('#bulkDelete').css('color', 'red');
            });

            $('#refresh').mouseover(function () {
                $('#refresh').css('border-bottom-color', 'green');
                $('#refresh').css('color', 'green');
            });

            $('div.toolbar').mouseout(function () {
                $('#bulkDelete').css('border-bottom-style', 'solid');
                $('#bulkDelete').css('border-bottom-color', '#6c757d');
                $('div.toolbar').css('background-color', 'white');
                $('div.toolbar').css('color', '#6c757d');
            });

            $('[data-bs-toggle="tooltip"]').tooltip();

            $('#refresh').click(function () {
                defaultTable.ajax.reload();
            });

            $('#bulkDelete').click(function () {

                var orgId = $('#org option:selected').val();
                orgName = $('#org option:selected').text();

                var url = "/Admin/RealEstates/DeleteByOrganization/";

                Swal.fire({
                    title: "Ўчиришни тасдиқлайсизми?",

                    html:
                        'Ўчирилгач маълумотларни <b>қайта тиклай олмайсиз!</b>, ' +
                        '<h3 style="color:red">' + orgName+'</h3> ' +
                        'га боғланган барча <b>объектлар ва бу объектларнинг ҳар бирига боғланган барча маълумотлар</b> базадан ўчирилади!',
                    
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Тасдиқлайман!',
                    cancelButtonText: 'Бекор қилиш',
                }).then(function (willDelete) {
                    if (willDelete.isConfirmed) {

                        let timerInterval;
                        Swal.fire({
                            title: 'Сўнги Огоҳлантириш',
                            html: 'Ўчирилади: <b></b> миллисониядан кейин!',
                            timer: 6000,
                            timerProgressBar: true,
                            showCancelButton: true,
                            cancelButtonText: 'Бекор қилиш',
                            didOpen: () => {
                                Swal.showLoading()
                                const b = Swal.getHtmlContainer().querySelector('b')
                                timerInterval = setInterval(() => {
                                    b.textContent = Swal.getTimerLeft()
                                }, 100)
                            },
                            willClose: () => {
                                clearInterval(timerInterval)
                            },
                            
                        }).then((result) => {
                            /* Read more about handling dismissals below */
                            if (result.dismiss === Swal.DismissReason.timer) {
                                $("#loader").attr('hidden', false);
                                $.ajax({
                                    type: "POST",
                                    url: url,
                                    data: JSON.stringify(orgId),
                                    contentType: "application/json",
                                    success: function (data) {
                                        if (data.success) {
                                            $("#loader").attr('hidden', true);
                                            Swal.fire({
                                                title: "Ўчирилди!",
                                                text: data.message,
                                                icon: "success",
                                            });
                                            /* toastr.success(data.message);*/
                                            defaultTable.ajax.reload();
                                        }
                                        else {
                                            $("#loader").attr('hidden', true);
                                            Swal.fire({
                                                title: data.message,
                                                icon: "error",
                                            });

                                        }
                                    }
                                });
                            }
                        })

                        
                    }
                });
            });
        }
        else {

        }

       
    })

});



function ChangeMainValuesOfModal(orgId, regionId, isBalance, isNotBalance) {
    $("#org_").val(orgId).change();
    $("#region_").val(regionId).change();
    $('#balance_').prop('checked', isBalance);
    $('#notBalance_').prop('checked', isNotBalance);
}

$("#balance_, #notBalance_").on("change", function () {

    var isBalance = document.getElementById("balance_").checked;
    var isNotBalance = document.getElementById("notBalance_").checked;

    if (isBalance && isNotBalance) {

        document.querySelector('#create-btn').disabled = false;

        $('#create-btn').css({ cursor: "pointer" });

        $("#case1").slideDown();
        $("#case2").slideUp();
        $("#case3").slideUp();

        

        $('.caseOne').prop('checked', true);
        $('.cellGroupOne').prop('checked', true);

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeIn();
        $("#inline-2").prop("checked", true);
        $('.blockTwo').prop('checked', true);
        $("#top-block2").fadeIn();
        $("#tab-block2").fadeIn();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeIn();
        $("#inline-4").prop("checked", true);
        $('.blockFour').prop('checked', true);
        $("#top-block4").fadeIn();
        $("#tab-block4").fadeIn();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeIn();
        $("#inline-5").prop("checked", true);
        $('.blockFive').prop('checked', true);
        $("#top-block5").fadeIn();
        $("#tab-block5").fadeIn();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeIn();
        $("#inline-6").prop("checked", true);
        $('.blockSix').prop('checked', true);
        $("#top-block6").fadeIn();
        $("#tab-block6").fadeIn();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeIn();
        $("#inline-7").prop("checked", true);
        $('.blockSeven').prop('checked', true);
        $("#top-block7").fadeIn();
        $("#tab-block7").fadeIn();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });
    }

    else if (isBalance && !isNotBalance) {

        document.querySelector('#create-btn').disabled = false;

        $('#create-btn').css({ cursor: "pointer" });

        $("#case1").slideDown();
        $("#case2").slideUp();
        $("#case3").slideDown();

        var isBlock1 = document.getElementById("c3-block1").checked;
        var isBlock4 = document.getElementById("c3-block4").checked;

        IsBalanceFunc(isBlock1, isBlock4);
        
    }

    else if (!isBalance && isNotBalance) {

        document.querySelector('#create-btn').disabled = false;

        $('#create-btn').css({ cursor: "pointer" });

        $("#case1").slideDown();
        $("#case3").slideUp();
        $("#case2").slideDown();

        var isBlock2 = document.getElementById("c2-block2").checked;
        var isBlock6 = document.getElementById("c2-block6").checked;
        var isBlock7 = document.getElementById("c2-block7").checked;

        IsNotBalanceFunc(isBlock2, isBlock6, isBlock7);
        
    }

    else if (!isBalance && !isNotBalance) {

        document.querySelector('#create-btn').disabled = true;

        $('#create-btn').css({ cursor: "not-allowed" });

        $("#case1").slideUp();

        $("#inline-1").prop("checked", false);
        $('.blockOne').prop('checked', false);

        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);

        $("#inline-3").prop("checked", false);
        $('.blockThree').prop('checked', false);

        $("#inline-4").prop("checked", false);
        $('.blockFour').prop('checked', false);       

        $("#inline-5").prop("checked", false);
        $('.blockFive').prop('checked', false);

        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);

        $("#inline-7").prop("checked", false);
        $('.blockSeven').prop('checked', false);       

        $("#inline-8").prop("checked", false);
        $('.blockEight').prop('checked', false);

    }


});

$("#c2-block2, #c2-block6, #c2-block7").on("change", function () {

    var isBlock2 = document.getElementById("c2-block2").checked;
    var isBlock6 = document.getElementById("c2-block6").checked;
    var isBlock7 = document.getElementById("c2-block7").checked;

    IsNotBalanceFunc(isBlock2, isBlock6, isBlock7);

});

function IsNotBalanceFunc(isBlock2, isBlock6, isBlock7) {

    if (isBlock2 && isBlock6 && isBlock7) {

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeIn();
        $("#inline-2").prop("checked", true);
        $('.blockTwo').prop('checked', true);
        $("#top-block2").fadeIn();
        $("#tab-block2").fadeIn();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeIn();
        $("#inline-4").prop("checked", true);
        $('.blockFour').prop('checked', true);
        $("#top-block4").fadeIn();
        $("#tab-block4").fadeIn();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeIn();
        $("#inline-5").prop("checked", true);
        $('.blockFive').prop('checked', true);
        $("#top-block5").fadeIn();
        $("#tab-block5").fadeIn();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeIn();
        $("#inline-6").prop("checked", true);
        $('.blockSix').prop('checked', true);
        $("#top-block6").fadeIn();
        $("#tab-block6").fadeIn();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeIn();
        $("#inline7").prop("checked", true);
        $('.blockSeven').prop('checked', true);
        $("#top-block7").fadeIn();
        $("#tab-block7").fadeIn();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });
    }

    if (!isBlock2 && isBlock6 && isBlock7) {

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeOut();
        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeIn();
        $("#inline-4").prop("checked", true);
        $('.blockFour').prop('checked', true);
        $("#top-block4").fadeIn();
        $("#tab-block4").fadeIn();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeIn();
        $("#inline-5").prop("checked", true);
        $('.blockFive').prop('checked', true);
        $("#top-block5").fadeIn();
        $("#tab-block5").fadeIn();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeIn();
        $("#inline-6").prop("checked", true);
        $('.blockSix').prop('checked', true);
        $("#top-block6").fadeIn();
        $("#tab-block6").fadeIn();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeIn();
        $("#inline-7").prop("checked", true);
        $('.blockSeven').prop('checked', true);
        $("#top-block7").fadeIn();
        $("#tab-block7").fadeIn();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });
    }

    if (isBlock2 && !isBlock6 && isBlock7) {

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeIn();
        $("#inline-2").prop("checked", true);
        $('.blockTwo').prop('checked', true);
        $("#top-block2").fadeIn();
        $("#tab-block2").fadeIn();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeOut();
        $("#inline-4").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block4").fadeOut();
        $("#tab-block4").fadeOut();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeOut();
        $("#inline-5").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block5").fadeOut();
        $("#tab-block5").fadeOut();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeOut();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeIn();
        $("#inline-7").prop("checked", true);
        $('.blockSeven').prop('checked', true);
        $("#top-block7").fadeIn();
        $("#tab-block7").fadeIn();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });
    }

    if (isBlock2 && isBlock6 && !isBlock7) {

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeIn();
        $("#inline-2").prop("checked", true);
        $('.blockTwo').prop('checked', true);
        $("#top-block2").fadeIn();
        $("#tab-block2").fadeIn();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeIn();
        $("#inline-4").prop("checked", true);
        $('.blockFour').prop('checked', true);
        $("#top-block4").fadeIn();
        $("#tab-block4").fadeIn();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeIn();
        $("#inline-5").prop("checked", true);
        $('.blockFive').prop('checked', true);
        $("#top-block5").fadeIn();
        $("#tab-block5").fadeIn();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeIn();
        $("#inline-6").prop("checked", true);
        $('.blockSix').prop('checked', true);
        $("#top-block6").fadeIn();
        $("#tab-block6").fadeIn();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeOut();
        $("#inline-7").prop("checked", false);
        $('.blockSeven').prop('checked', false);
        $("#top-block7").fadeOut();
        $("#tab-block7").fadeOut();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });


    }

    if (!isBlock2 && !isBlock6 && isBlock7) {

        document.querySelector('#create-btn').disabled = false;

        $('#create-btn').css({ cursor: "pointer" });

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeOut();
        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeOut();
        $("#inline-4").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block4").fadeOut();
        $("#tab-block4").fadeOut();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeOut();
        $("#inline-5").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block5").fadeOut();
        $("#tab-block5").fadeOut();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeOut();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeIn();
        $("#inline-7").prop("checked", true);
        $('.blockSeven').prop('checked', true);
        $("#top-block7").fadeIn();
        $("#tab-block7").fadeIn();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });

    }

    if (isBlock2 && !isBlock6 && !isBlock7) {

        document.querySelector('#create-btn').disabled = false;

        $('#create-btn').css({ cursor: "pointer" });

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeIn();
        $("#inline-2").prop("checked", true);
        $('.blockTwo').prop('checked', true);
        $("#top-block2").fadeIn();
        $("#tab-block2").fadeIn();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeOut();
        $("#inline-4").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block4").fadeOut();
        $("#tab-block4").fadeOut();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeOut();
        $("#inline-5").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block5").fadeOut();
        $("#tab-block5").fadeOut();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeOut();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeOut();
        $("#inline-7").prop("checked", false);
        $('.blockSeven').prop('checked', false);
        $("#top-block7").fadeOut();
        $("#tab-block7").fadeOut();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });

    }

    if (!isBlock2 && isBlock6 && !isBlock7) {

        document.querySelector('#create-btn').disabled = false;

        $('#create-btn').css({ cursor: "pointer" });

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeOut();
        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeIn();
        $("#inline-4").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block4").fadeOut();
        $("#tab-block4").fadeOut();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeIn();
        $("#inline-5").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block5").fadeOut();
        $("#tab-block5").fadeOut();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeIn();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeOut();
        $("#inline-7").prop("checked", true);
        $('.blockSeven').prop('checked', true);
        $("#top-block7").fadeIn();
        $("#tab-block7").fadeIn();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });

    }

    if (!isBlock2 && !isBlock6 && !isBlock7) {

        document.querySelector('#create-btn').disabled = true;

        $('#create-btn').css({ cursor: "not-allowed" });

        $("#inline1").fadeOut();
        $("#inline-1").prop("checked", false);
        $('.blockOne').prop('checked', false);
        $("#top-block1").fadeOut();
        $("#tab-block1").fadeOut();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeOut();
        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeOut();
        $("#inline-3").prop("checked", false);
        $('.blockThree').prop('checked', false);
        $("#top-block3").fadeOut();
        $("#tab-block3").fadeOut();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeOut();
        $("#inline-4").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block4").fadeOut();
        $("#tab-block4").fadeOut();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeOut();
        $("#inline-5").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block5").fadeOut();
        $("#tab-block5").fadeOut();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeOut();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeOut();
        $("#inline-7").prop("checked", false);
        $('.blockSeven').prop('checked', false);
        $("#top-block7").fadeOut();
        $("#tab-block7").fadeOut();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeOut();
        $("#inline-8").prop("checked", false);
        $('.blockEight').prop('checked', false);
        $("#top-block8").fadeOut();
        $("#tab-block8").fadeOut();
        $("#tab-block8").css({ 'display': '' });

    }

}

$("#c3-block1, #c3-block4").on("change", function () {

    var isBlock1 = document.getElementById("c3-block1").checked;
    var isBlock4 = document.getElementById("c3-block4").checked;
    
    IsBalanceFunc(isBlock1, isBlock4);

});

function IsBalanceFunc(isBlock1, isBlock4) {

    if (isBlock1 && isBlock4) {

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeOut();
        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeIn();
        $("#inline-4").prop("checked", true);
        $('.blockFour').prop('checked', true);
        $("#top-block4").fadeIn();
        $("#tab-block4").fadeIn();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeIn();
        $("#inline-5").prop("checked", true);
        $('.blockFive').prop('checked', true);
        $("#top-block5").fadeIn();
        $("#tab-block5").fadeIn();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeOut();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeOut();
        $("#inline-7").prop("checked", false);
        $('.blockSeven').prop('checked', false);
        $("#top-block7").fadeOut();
        $("#tab-block7").fadeOut();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });
    }

    if (isBlock1 && !isBlock4) {

        document.querySelector('#create-btn').disabled = false;

        $('#create-btn').css({ cursor: "pointer" });

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeOut();
        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeOut();
        $("#inline-4").prop("checked", false);
        $('.blockFour').prop('checked', false);
        $("#top-block4").fadeOut();
        $("#tab-block4").fadeOut();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeOut();
        $("#inline-5").prop("checked", false);
        $('.blockFive').prop('checked', false);
        $("#top-block5").fadeOut();
        $("#tab-block5").fadeOut();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeOut();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeOut();
        $("#inline-7").prop("checked", false);
        $('.blockSeven').prop('checked', false);
        $("#top-block7").fadeOut();
        $("#tab-block7").fadeOut();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });

        var isNotBalance = document.getElementById("inline2").checked;
        console.log("inline2: " + isNotBalance)
    }

    if (!isBlock1 && isBlock4) {

        document.querySelector('#create-btn').disabled = false;

        $('#create-btn').css({ cursor: "pointer" });

        $("#inline1").fadeIn();
        $("#inline-1").prop("checked", true);
        $('.blockOne').prop('checked', true);
        $("#top-block1").fadeIn();
        $("#tab-block1").fadeIn();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeOut();
        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeIn();
        $("#inline-3").prop("checked", true);
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeIn();
        $("#inline-4").prop("checked", true);
        $('.blockFour').prop('checked', true);
        $("#top-block4").fadeIn();
        $("#tab-block4").fadeIn();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeIn();
        $("#inline-5").prop("checked", true);
        $('.blockFive').prop('checked', true);
        $("#top-block5").fadeIn();
        $("#tab-block5").fadeIn();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeOut();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeOut();
        $("#inline-7").prop("checked", false);
        $('.blockSeven').prop('checked', false);
        $("#top-block7").fadeOut();
        $("#tab-block7").fadeOut();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeIn();
        $("#inline-8").prop("checked", true);
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });
    }

    if (!isBlock1 && !isBlock4) {

        document.querySelector('#create-btn').disabled = true;

        $('#create-btn').css({ cursor: "not-allowed" });

        $("#inline1").fadeOut();
        $("#inline-1").prop("checked", false);
        $('.blockOne').prop('checked', false);
        $("#top-block1").fadeOut();
        $("#tab-block1").fadeOut();
        $("#tab-block1").css({ 'display': '' });

        $("#inline2").fadeOut();
        $("#inline-2").prop("checked", false);
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
        $("#tab-block2").css({ 'display': '' });

        $("#inline3").fadeOut();
        $("#inline-3").prop("checked", false);
        $('.blockThree').prop('checked', false);
        $("#top-block3").fadeOut();
        $("#tab-block3").fadeOut();
        $("#tab-block3").css({ 'display': '' });

        $("#inline4").fadeOut();
        $("#inline-4").prop("checked", false);
        $('.blockFour').prop('checked', false);
        $("#top-block4").fadeOut();
        $("#tab-block4").fadeOut();
        $("#tab-block4").css({ 'display': '' });

        $("#inline5").fadeOut();
        $("#inline-5").prop("checked", false);
        $('.blockFive').prop('checked', false);
        $("#top-block5").fadeOut();
        $("#tab-block5").fadeOut();
        $("#tab-block5").css({ 'display': '' });

        $("#inline6").fadeOut();
        $("#inline-6").prop("checked", false);
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
        $("#tab-block6").css({ 'display': '' });

        $("#inline7").fadeOut();
        $("#inline-7").prop("checked", false);
        $('.blockSeven').prop('checked', false);
        $("#top-block7").fadeOut();
        $("#tab-block7").fadeOut();
        $("#tab-block7").css({ 'display': '' });

        $("#inline8").fadeOut();
        $("#inline-8").prop("checked", false);
        $('.blockEight').prop('checked', false);
        $("#top-block8").fadeOut();
        $("#tab-block8").fadeOut();
        $("#tab-block8").css({ 'display': '' });
    }
}


$("#inline-1").on("change", function () {

    var isChecked = document.getElementById("inline-1").checked;
    if (isChecked) {
        $('.blockOne').prop('checked', true);
        $(".blockOneDiv").fadeIn();
    }
    else{
        $('.blockOne').prop('checked', false);
        $(".blockOneDiv").fadeOut();
    }

});

$("#inline-2").on("change", function () {

    var isChecked = document.getElementById("inline-2").checked;
    if (isChecked) {
        $('.blockTwo').prop('checked', true);
        $("#top-block2").fadeIn();
        $("#tab-block2").fadeIn();
        $("#tab-block2").css({'display':''});
        
    }
    else {
        $('.blockTwo').prop('checked', false);
        $("#top-block2").fadeOut();
        $("#tab-block2").fadeOut();
    }

});

$("#inline-3").on("change", function () {

    var isChecked = document.getElementById("inline-3").checked;
    if (isChecked) {
        $('.blockThree').prop('checked', true);
        $("#top-block3").fadeIn();
        $("#tab-block3").fadeIn();
        $("#tab-block3").css({ 'display': '' });
    }
    else {
        $('.blockThree').prop('checked', false);
        $("#top-block3").fadeOut();
        $("#tab-block3").fadeOut();
    }

});

$("#inline-4").on("change", function () {

    var isChecked = document.getElementById("inline-4").checked;
    if (isChecked) {
        $('.blockFour').prop('checked', true);
        $("#top-block4").fadeIn();
        $("#tab-block4").fadeIn();
        $("#tab-block4").css({ 'display': '' });
    }
    else {
        $('.blockFour').prop('checked', false);
        $("#top-block4").fadeOut();
        $("#tab-block4").fadeOut();
    }

});

$("#inline-5").on("change", function () {

    var isChecked = document.getElementById("inline-5").checked;
    if (isChecked) {
        $('.blockFive').prop('checked', true);
        $("#top-block5").fadeIn();
        $("#tab-block5").fadeIn();
        $("#tab-block5").css({ 'display': '' });
    }
    else {
        $('.blockFive').prop('checked', false);
        $("#top-block5").fadeOut();
        $("#tab-block5").fadeOut();
    }

});

$("#inline-6").on("change", function () {

    var isChecked = document.getElementById("inline-6").checked;
    if (isChecked) {
        $('.blockSix').prop('checked', true);
        $("#top-block6").fadeIn();
        $("#tab-block6").fadeIn();
        $("#tab-block6").css({ 'display': '' });
    }
    else {
        $('.blockSix').prop('checked', false);
        $("#top-block6").fadeOut();
        $("#tab-block6").fadeOut();
    }

});

$("#inline-7").on("change", function () {

    var isChecked = document.getElementById("inline-7").checked;
    if (isChecked) {
        $('.blockSeven').prop('checked', true);
        $("#top-block7").fadeIn();
        $("#tab-block7").fadeIn();
        $("#tab-block7").css({ 'display': '' });
    }
    else {
        $('.blockSeven').prop('checked', false);
        $("#top-block7").fadeOut();
        $("#tab-block7").fadeOut();
    }

});

$("#inline-8").on("change", function () {

    var isChecked = document.getElementById("inline-8").checked;
    if (isChecked) {
        $('.blockEight').prop('checked', true);
        $("#top-block8").fadeIn();
        $("#tab-block8").fadeIn();
        $("#tab-block8").css({ 'display': '' });
    }
    else {
        $('.blockEight').prop('checked', false);
        $("#top-block8").fadeOut();
        $("#tab-block8").fadeOut();
    }

});

function getUser(rdata) {
    var rData = JSON.parse(decodeURIComponent(rdata))
    var id = rData.realEstateId;
    $("#rEstateName").text(rData.realEstateName);
    $("#rEstateId").text(rData.realEstateId);

    $.ajax({
        type: "POST",
        url: "/Admin/RealEstates/GetUser/",
        data: JSON.stringify(id),
        contentType: "application/json",

        success: function (data) {
            if (data != null) {

                $("#userFName").text(data.data.fullName);
                $("#userOrg").text(data.data.organizationName);
                $("#userPosition").text(data.data.position);
                $("#telegram").attr("href","https://t.me/" + data.data.phoneNumber);
                $("#phone").attr("href", "tel:" + data.data.phoneNumber);
                $("#mail").attr("href", "mailto:" + data.data.phoneNumber);
                $("#realEstateCounter").text(data.data.realEstateCount);
                $("#sharesCounter").text(data.data.shareCount);
                $("#soldCounter").text(data.data.soldAssetCount);
                
                setTimeout(
                    function () {
                        $('#userModal').modal('show');
                    }, 100);
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: data.message
                });
            }
        },

        failure: function () {
            Swal.fire(
                "Internal Error",
                "Афсуски амалиётни бажариш имкони бўлмади!", 
                "error"
            )
        }       

    })
}

function openUserChangeModal() {    

    $.ajax({
        type: "GET",
        url: "/Admin/Users/GetAll",

        success: function (data) {
            if (data != null) {

                for (var i = 0; i < data.data.length; i++) {
                    var optionText = data.data[i]["fullname"];
                    var optionValue = data.data[i]["id"];

                    $('#users').append(`<option value="${optionValue}">
                                       ${optionText}
                                  </option>`);
                }

                $("#changeUserModal").modal("show");
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: data.message
                });
            }
        },

        failure: function () {
            Swal.fire(
                "Internal Error",
                "Афсуски амалиётни бажариш имкони бўлмади!",
                "error"
            )
        }

    })

}

function changeUserOfAsset() {

    var assetId = $("#rEstateId").text();
    var target = "1";
    var userId = $('#users').find(":selected").val();

    data = {
        assetId: assetId,
        target: target,
        userId: userId
    }

    $.ajax({

        type: "POST",
        url: "/Admin/Users/ChangeUserForAsset/",
        data: JSON.stringify(data),
        contentType: "application/json",

        success: function (data) {
            if (data.success) {

                Swal.fire({
                    icon: 'success',
                    title: data.message
                });
            }

            else {
                Swal.fire({
                    icon: 'error',
                    title: data.message
                });
            }
        },

        failure: function () {
            Swal.fire(
                "Internal Error",
                "Афсуски амалиётни бажариш имлони бўлмади!",
                "error"
            )
        }

    })

}

function inDetail(id) {
    var data = {
        id: id,
        forDetails: false
    }
    $.ajax({
        type: "POST",
        url: "/Admin/RealEstates/GetRealEstate/",
        data: JSON.stringify(data),
        contentType: "application/json",

        success: function (data) {
            if (data.success) {
                                
                $("#_rName").text(data.data.realEstateName);
                $("#_orgName").text(data.data.assetHolderName);
                $("#_cadNum").text(data.data.cadastreNumber);
                $("#_cadDate").text(data.data.cadasterRegDateStr);
                $("#_comDate").text(data.data.commisioningDateStr);
                $("#_activity").text(data.data.activity);
                $("#_reg").text(data.data.regionOfObject.regionName);
                $("#_dist").text(data.data.districtOfObject.districtName);
                $("#_address").text(data.data.address);
                $("#_building").text(data.data.buildingArea);
                $("#_allArea").text(data.data.fullArea);
                $("#_infra").text(data.data.infrastructureNames);
                $("#_numberEm").text(data.data.numberOfEmployee);
                $("#_maintCost").text(data.data.maintenanceCostForYear);
                $("#_initCost").text(data.data.initialCostOfObject);
                $("#_wear").text(data.data.wear);
                $("#_resCost").text(data.data.residualValueOfObject);
                $("#_proposal").text(data.data.proposal.proposalName);
                $("#_comment").text(data.data.comment);

                $('#detailModal').modal('show');
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: data.message
                });
            }
        },

        failure: function () {
            Swal.fire(
                "Internal Error",
                "Афсуски амалиётни бажариш имлони бўлмади!",
                "error"
            )
        }

    })

}

function showFiles(fdata) {
    fdata = JSON.parse(decodeURIComponent(fdata));
    $("#file1").attr("href", fdata.cadFileLink);
    $("#file2").attr("href", fdata.photo1Link);
    $("#file3").attr("href", fdata.photo2Link);
    $("#file4").attr("href", fdata.photo3Link);
    $('#filesModal').modal('show');

}

function GetPassport(id) {

    window.location.href = "/Admin/RealEstates/GetPassport/" + id;

}

function DownloadFile(id) {

    window.location.href = "/Admin/RealEstates/DownloadFile/" + id;

}

function Edit(id) {

    window.location.href = "/Admin/RealEstates/Edit/" + id;

}

function Delete(url) {
    Swal.fire({
        title: "Ўчиришни тасдиқлайсизми?",
        text: "Ўчирилгач маълумотларни қайта тиклай олмайсиз! Бу объектга боғланган барча маълумотлар базадан ўчирилади!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (willDelete)  {
        if (willDelete.isConfirmed) {
            let timerInterval;
            Swal.fire({
                title: 'Сўнги Огоҳлантириш',
                html: 'Ўчирилади: <b></b> миллисониядан кейин!',
                timer: 6000,
                timerProgressBar: true,
                showCancelButton: true,
                cancelButtonText: 'Бекор қилиш',
                didOpen: () => {
                    Swal.showLoading()
                    const b = Swal.getHtmlContainer().querySelector('b')
                    timerInterval = setInterval(() => {
                        b.textContent = Swal.getTimerLeft()
                    }, 100)
                },
                willClose: () => {
                    clearInterval(timerInterval)
                },

            }).then((result) => {
                /* Read more about handling dismissals below */
                if (result.dismiss === Swal.DismissReason.timer) {
                    $("#loader").attr('hidden', false);
                    $.ajax({
                        type: "DELETE",
                        url: url,
                        success: function (data) {
                            if (data.success) {
                                $("#loader").attr('hidden', true);
                                Swal.fire({
                                    title: "Ўчирилди!",
                                    icon: "success",
                                });
                                /* toastr.success(data.message);*/
                                defaultTable.ajax.reload();
                            }
                            else {
                                $("#loader").attr('hidden', true);
                                Swal.fire(data.message);
                            }
                        }
                    });
                }
            })

            
            
        }
    });

}
