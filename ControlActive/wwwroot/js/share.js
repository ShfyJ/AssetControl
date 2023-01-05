
var dataTable1;
var dataTable2;

$(document).ready(function () {

    loadDataTable();

    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust().responsive.recalc();
    });

    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();

});



function loadDataTable() {
    dataTable1 = $('#myTable').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/Shares/GetUnSent"
        },
        "columns": [
            { "data": "share.businessEntityName", "width": "15%" },
            { "data": "share.idRegNumber", "width": "15%" },
            { "data": "share.parentOrganization", "width": "15%" },
            { "data": "share.activities", "width": "15%" },
            { "data": "share.activityShare", "width": "15%" },
            { "data": "share.foundationYearStr", "width": "15%" },
            { "data": "share.stateRegistrationDateStr", "width": "15%" },
            { "data": "share.regionOfObject.regionName", "width": "15%" },
            { "data": "share.districtOfObject.districtName", "width": "15%" },
            { "data": "share.address", "width": "15%" },
            { "data": "share.authorizedCapital", "width": "15%" },
            { "data": "shareholders", "width": "15%" },
            { "data": "shareAmount", "width": "15%" },
          
            { "data": "sharePercentage", "width": "15%" },
            { "data": "share.numberOfShares", "width": "15%" },
            { "data": "share.parValueOfShares", "width": "15%" },
            { "data": "share.administrativeStaff", "width": "15%" },
            { "data": "share.productionPersonal", "width": "15%" },
            { "data": "share.allStaff", "width": "15%" },
            { "data": "share.averageMonthlySalary", "width": "15%" },
            { "data": "share.maintanenceCostForYear", "width": "15%" },
            { "data": "share.productionArea", "width": "15%" },
            { "data": "share.buildingsArea", "width": "15%" },
            { "data": "share.allArea", "width": "15%" },
            { "data": "share.amountPayable", "width": "15%" },
            { "data": "share.amountReceivable", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p><span class="badge badge-dark">' + data.share.year1 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear1 + '</span></p>'
                }

            },
            {
                "data": {},
                "render": function (data) {
                    return '<p><span class="badge badge-dark">' + data.share.year2 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear2 + '</span></p>'
                }

            },
            {
                "data": {},
                "render": function (data) {
                    return '<p><span class="badge badge-dark">' + data.share.year3 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear3 + '</span></p>'
                }

            },
            { "data": "share.comments", "width": "15%" },
            

            {
                "data": {},

                "render": function (data) {
                    return '  <div class="row">' +
                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.regCertificateId + ') style="cursor: pointer; font-size:12px;">Сертификат <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +

                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.orgCharterId + ') style="cursor: pointer; font-size:12px;">Ташкилот устави <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +
                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.balanceSheetId + ') style="cursor: pointer; font-size:12px;">Бухгалтерия баланси <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +
                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.financialResultId + ') style="cursor: pointer; font-size:12px;">Молиявий натижа'+'<br/>'+'ҳисоботи <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +
                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.auditConclusionId +') style="cursor: pointer; font-size:12px;">Аудиторлик хулосаси <i class="fa fa-download"></i></a>' +

                            '</div>' +
                            '</div>' +

                            '</div>' +
                            '</div>' +
                            '</div >'
                }
            },

            {
                "data": {},
                "render": function (data) {
                    return `
                            <div >
                                <a onclick=Edit('${data.share.shareId}') id="edit" class="btn btn-warning-gradien" style="cursor:pointer; width:140px">
                                    <i class="fas fa-edit"></i> Таҳрирлаш
                                </a>

                                <a onclick=Delete('/SimpleUser/Shares/Delete/${data.share.shareId}') id="delete" class="btn btn-danger-gradien" style="cursor:pointer; width:120px">
                                    <i class="fas fa-trash"></i> Ўчириш
                                </a>

                                <a onclick=Send("${data.share.shareId}") id="send" class="btn btn-success-gradien" style="cursor:pointer; width:140px">
                                    <i class="fas fa-paper-plane"></i> Тасдиқлаш
                                </a>

                            </div>
                        `

                }

            }

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "Саҳифа _PAGE_ / _PAGES_",
            "infoEmpty": "Ҳеч қандай дата мавжуд эмас",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "Қидирув:",
        },
        "lengthMenu": [20, 30, 40, 50]
    });

    dataTable2 = $('#myTable2').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/Shares/GetSent"
        },
        "columns": [
            { "data": "share.businessEntityName", "width": "15%" },
            { "data": "share.idRegNumber", "width": "15%" },
            { "data": "share.parentOrganization", "width": "15%" },
            { "data": "share.activities", "width": "15%" },
            { "data": "share.activityShare", "width": "15%" },
            { "data": "share.foundationYearStr", "width": "15%" },
            { "data": "share.stateRegistrationDateStr", "width": "15%" },
            { "data": "share.regionOfObject.regionName", "width": "15%" },
            { "data": "share.districtOfObject.districtName", "width": "15%" },
            { "data": "share.address", "width": "15%" },
            { "data": "share.authorizedCapital", "width": "15%" },
            { "data": "shareholders", "width": "15%" },
            { "data": "shareAmount", "width": "15%" },

            { "data": "sharePercentage", "width": "15%" },
            { "data": "share.numberOfShares", "width": "15%" },
            { "data": "share.parValueOfShares", "width": "15%" },
            { "data": "share.administrativeStaff", "width": "15%" },
            { "data": "share.productionPersonal", "width": "15%" },
            { "data": "share.allStaff", "width": "15%" },
            { "data": "share.averageMonthlySalary", "width": "15%" },
            { "data": "share.maintanenceCostForYear", "width": "15%" },
            { "data": "share.productionArea", "width": "15%" },
            { "data": "share.buildingsArea", "width": "15%" },
            { "data": "share.allArea", "width": "15%" },
            { "data": "share.amountPayable", "width": "15%" },
            { "data": "share.amountReceivable", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p><span class="badge badge-dark">' + data.share.year1 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear1 + '</span></p>'
                }

            },
            {
                "data": {},
                "render": function (data) {
                    return '<p><span class="badge badge-dark">' + data.share.year2 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear2 + '</span></p>'
                }

            },
            {
                "data": {},
                "render": function (data) {
                    return '<p><span class="badge badge-dark">' + data.share.year3 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear3 + '</span></p>'
                }

            },
            { "data": "share.comments", "width": "15%" },


            {
                "data": {},

                "render": function (data) {
                    return '  <div class="row">' +
                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.regCertificateId + ') style="cursor: pointer; font-size:12px;">Сертификат <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +

                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.orgCharterId + ') style="cursor: pointer; font-size:12px;">Ташкилот устави <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +
                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.balanceSheetId + ') style="cursor: pointer; font-size:12px;">Бухгалтерия баланси <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +
                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.financialResultId + ') style="cursor: pointer; font-size:12px;">Молиявий натижа' + '<br/>' + 'ҳисоботи <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +
                        '<div class="col-sm-2" >' +
                        '<div class="thumbnail">' +
                        '<div class="image view view-first">' +
                        '<img style="width: 100%; display: block;" src="/images/document.png" alt="image" />' +
                        '<div class="mask">' +
                        '<a onclick=DownloadFile(' + data.share.auditConclusionId + ') style="cursor: pointer; font-size:12px;">Аудиторлик хулосаси <i class="fa fa-download"></i></a>' +

                        '</div>' +
                        '</div>' +

                        '</div>' +
                        '</div>' +
                        '</div >'
                }
            },

            {
                "data": {},
                "render": function (data) {
                    return `
                            <div >
                                <a onclick=AskToEdit('${data.share.shareId}') id="askEdit" class="btn btn-primary-gradien" style="cursor:pointer; width:300px">
                                    <i class="fa fa-exchange"></i> Таҳрирлаш учун рухсат олиш
                                </a>                              
                            </div>
                        `

                }

            },
           
        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "Саҳифа _PAGE_ / _PAGES_",
            "infoEmpty": "Ҳеч қандай дата мавжуд эмас",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "Қидирув:",
        },
        "lengthMenu": [20, 30, 40, 50]
    });
}

function DownloadFile(id) {

    window.location.href = "/SimpleUser/Shares/DownloadFile/" + id;


}
function Edit(id) {

    window.location.href = "/SimpleUser/Shares/Edit/" + id;

}

function Details(id) {

    window.location.href = "/SimpleUser/Shares/Details/" + id;

}


function AskToEdit(id) {

    Swal.fire({
        title: 'Тасдиқланган маълумотларини таҳрирламоқчимисиз?',
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Cўров юбориш',
        cancelButtonText: 'Бекор қилиш',

    }).then(function (willSend) {
        if (willSend.isConfirmed) {
            var notification;
            $.ajax({
                type: "GET",
                url: "/SimpleUser/Shares/GetUsersIds/",
                success: function (data) {
                    if (!data.success) {
                        Swal.fire("Хатолик", data.message, {
                            icon: "error",
                        });

                    }
                    else {

                        notification = {
                            MessageType: 1, //ma'lumotni o'zgartirishga dostup
                            ToUserId: data.data[1],
                            FromUserId: data.data[0],
                            ObjectId: parseInt(id),
                            ObjectType: 2
                        };


                        connection.invoke("Notify", notification).then(function () {
                            toastr.options = {

                                "progressBar": true,

                                "timeOut": "6000",

                            }
                            toastr.success("Рухсат берилишини кутинг!", "Cўровнома жўнатилди!");
                        }).catch(function (err) {
                            toastr.options = {

                                "progressBar": true,

                                "timeOut": "3000",

                            }
                            toastr.error("Cўровнома юборишда хатолик юз берди!", err.toString());
                            setTimeout(function () {
                                connection.start().then(function () {
                                    toastr.success("Сервер билан алоқа ўрнатилди!", "Сўровни қайтадан жўнатинг");
                                }).catch(function (err) {
                                    toastr.error("Серверга уланишда хатолик юз берди!", err.toString());
                                });
                            }, 3000);


                        });


                    }
                }
            });




        }
    });
}

function Delete(url) {
    Swal.fire({
        title: "Ўчиришни хоҳлайсизми?",
        text: "Ўчирилгач маълумотларни қайта тиклай олмайсиз! Бу объектга боғланган барча маълумотлар базадан ўчирилади!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (willDelete) {
        if (willDelete.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        Swal.fire("Ўчирилди!", data.message, {
                            icon: "success",
                        });
                        /* toastr.success(data.message);*/
                        dataTable1.ajax.reload();
                        dataTable2.ajax.reload();
                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });

}

function Send(id) {
    Swal.fire({
        title: "Актив маълумотларини тасдиқлайсизми?",
        text: "Маълумотларни тасдиқлагач, уларни ўзгартира олмайсиз!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (willSend) {
        if (willSend.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/SimpleUser/Shares/Send/',
                data: JSON.stringify(id),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {
                        Swal.fire("Тасдиқланди!", data.message, {
                            icon: "success",
                        });
                        /* toastr.success(data.message);*/
                        dataTable1.ajax.reload();
                        dataTable2.ajax.reload();
                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });
}



