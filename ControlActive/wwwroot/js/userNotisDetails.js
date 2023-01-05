var dataTable1;
var type;
var objectId;
var url = "";
var fromUserId;
var toUserId;
var isPermitted;
var notiId;

$(document).ready(function () {
    objectId = $("#objectId").text();
    type = $("#type").text();
    fromUserId = $('#fromUserId').text();
    toUserId = $('#toUserId').text();
    isPermitted = ($('#isPermitted').text() === 'True');
    console.log($('#isPermitted').text());
    notiId = $('#notiId').text();

    if (isPermitted) {
        $("#permissionDiv").removeAttr('hidden');
        // $("#dateTime").text('Рухсат берилган вақт: '+permittedTime);
        $('#permitBtn').attr("disabled", true);
    }

    if (type == 1) {
        loadRealEstateTable();
    }

    if (type == 2) {
        loadShareTable();
    }
    if (type == 3 || type == 4) {
        loadTransfAssetTable();
    }

    if (type == 5 || type == 6) {
        loadAuksionTable();
    }
    if (type == 7 || type == 8 || type == 9 || type == 10 || type == 11 || type == 12) {
        loadOneTimePTable();
    }
    if (type == 13 || type == 14 || type == 15 || type == 16) {
        loadInstAssetTable();
    }

    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust().responsive.recalc();
    });

    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
    $('[data-bs-toggle="tooltip"]').tooltip();


});


function loadRealEstateTable() {

    var data = {
        id: objectId,
        forDetails: true
    }
    dataTable1 = $('#myTable').DataTable({

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');

        },

        scrollX: false,

        "ajax": {
            "url": '/SimpleUser/RealEstates/GetRealEstate/',
            "type": "POST",

            "data": function () {
                return JSON.stringify(data);
            },
            "contentType": "application/json",

        },
        autoWidth: false,
        "dom": '',
        "order": [],

        "columns": [
            { "data": "realEstateName" },

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Сизда хабарлар мавжуд эмас",
            "info": "Саҳифа _PAGE_ / _PAGES_",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "Қидирув:",
        },
        "lengthMenu": [15, 20, 30, 40, 50]
    });
    //console.log($('#myTable').DataTable().rows().count());
}

function loadShareTable() {

    var data = {
        id: objectId,
        forDetails: true
    }
    dataTable1 = $('#myTable').DataTable({

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');

        },

        scrollX: false,

        "ajax": {
            "url": '/SimpleUser/Shares/GetShare/',
            "type": "POST",

            "data": function () {
                return JSON.stringify(data);
            },
            "contentType": "application/json",

        },
        autoWidth: false,
        "dom": '',
        "order": [],

        "columns": [
            { "data": "businessEntityName" },

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Сизда хабарлар мавжуд эмас",
            "info": "Саҳифа _PAGE_ / _PAGES_",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "Қидирув:",
        },
        "lengthMenu": [15, 20, 30, 40, 50]
    });
    //console.log($('#myTable').DataTable().rows().count());
}

function GivePermission() {

    var data = {
        id: objectId,
        type: type,
        notiId: notiId
    }
    var notification;

    $.ajax({
        type: "POST",
        url: "/SimpleUser/Users/GivePermissionToEdit/",
        data: JSON.stringify(data),
        contentType: "application/json",

        success: function (data) {
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: data.message
                });

                $("#permissionDiv").removeAttr('hidden');
                //$("#dateTime").text(new Date());
                $('#permitBtn').attr("disabled", true);

                notification = {
                    ToUserId: fromUserId,
                    FromUserId: toUserId,
                    MessageType: 6,
                    ObjectType: 1,
                    ObjectId: parseInt(objectId)

                };

                connection.invoke("Notify", notification).then(function () {
                    toastr.options = {

                        "progressBar": true,

                        "timeOut": "3000",

                    }
                    toastr.success("Фойдаланувчига хабар юборилди!");
                }).catch(function (err) {
                    toastr.options = {

                        "progressBar": true,

                        "timeOut": "3000",

                    }
                    toastr.error("Фойдаланувчига хабар юборилмади!", err.toString());
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
                "Афсуски амалиётни бажариш имкони бўлмади!",
                "error"
            )
        }

    })
}