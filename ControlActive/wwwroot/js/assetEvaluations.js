var dataTable1;
var dataTable2;
var dataTable3;
var dataTable4;

$(document).ready(function () {

    loadDataTable();

    $('#accordionoc').on('shown.bs.collapse', function () {
        $.each($.fn.dataTable.tables(true), function () {
            $(this).DataTable().columns.adjust().draw();
        });
    });

    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust().responsive.recalc();
    });



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
        "dom": '<lf<rt>ip>',
        scrollX: false,

        "ajax": {
            "url": "/SimpleUser/AssetEvaluations/GetUnSentRealEstates/"
        },
        "columns": [
            { "data": "realEstate.realEstateName", "width": "15%" },
            {
                "data": {
                    "reportStatusStr": "assetEvaluation.reportStatusStr",
                    "reportStatus": "assetEvaluation.reportStatus"
                },



                "render": function (data) {

                    if (data.assetEvaluation.reportStatus)
                        return ' <p class="badge badge-success">' + data.assetEvaluation.reportStatusStr + '</p>'

                    else
                        return ' <p class="badge badge-danger">' + data.assetEvaluation.reportStatusStr + '</p>'
                }


            },
            { "data": "assetEvaluation.evaluatingOrgName", "width": "15%" },
            { "data": "assetEvaluation.reportDateStr", "width": "15%" },
            { "data": "assetEvaluation.reportRegNumber", "width": "15%" },
            { "data": "assetEvaluation.marketValue", "width": "15%" },
            { "data": "assetEvaluation.examiningOrgName", "width": "15%" },
            { "data": "assetEvaluation.examReportDateStr", "width": "15%" },
            { "data": "assetEvaluation.examReportRegNumber", "width": "15%" },
            

            {
                "data": {

                    reportFileId: "assetEvaluation.reportFileId",
                    examReportFileId: "assetEvaluation.examReportFileId",
                    
                },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.assetEvaluation.reportFileId}') style="cursor: pointer;">Баҳолаш ҳисоботи <i class="fa fa-download"></i></a>

                                            </div>
                                        </div>

                                    </div>
                                  </div>

                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.assetEvaluation.examReportFileId}') style="cursor: pointer;">Экспертиза ҳулосаси <i class="fa fa-download"></i></a>

                                            </div>
                                        </div>

                                    </div>
                                  </div>

                                
                            </div >
                            
                       `
                }
            },


            {
                "data": {
                    assetId: "assetEvaluation.assetEvaluationId",
                    realEstateId: "realEstate.realEstateId",
                    target: "target"
                },
                "render": function (data) {
                    return `
                            <div >
                                <a onclick=Edit(${data.assetEvaluation.assetEvaluationId}` + `,` + `${data.realEstate.realEstateId}` + `,` + `${data.target}` + `)` + ` id="edit" class="btn btn-warning-gradien" style="cursor:pointer; width:150px">
                                    <i class="fas fa-edit"></i> Ўзгартириш
                                </a>

                                <a onclick=Delete('/SimpleUser/AssetEvaluations/Delete/${data.assetEvaluation.assetEvaluationId}') id="delete" class="btn btn-danger-gradien" style="cursor:pointer; width:140px">
                                    <i class="fas fa-trash"></i> Ўчириш
                                </a>

                                <a onclick=Send(${data.assetEvaluation.assetEvaluationId}` + `,` + `${data.target}) id="send" class="btn btn-success-gradien" style="cursor:pointer; width:140px">
                                    <i class="fas fa-paper-plane"></i> Тасдиқлаш
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
        "lengthMenu": [15, 20, 30, 40, 50]
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
        "dom": '<lf<rt>ip>',
        scrollX: false,


        "ajax": {
            "url": "/SimpleUser/AssetEvaluations/GetSentRealEstates/"
        },
        "columns": [
            { "data": "realEstate.realEstateName", "width": "15%" },
            {
                "data": {
                    "reportStatusStr": "assetEvaluation.reportStatusStr",
                    "reportStatus": "assetEvaluation.reportStatus"
                },



                "render": function (data) {

                    if (data.assetEvaluation.reportStatus)
                        return ' <p class="badge badge-success">' + data.assetEvaluation.reportStatusStr + '</p>'

                    else
                        return ' <p class="badge badge-danger">' + data.assetEvaluation.reportStatusStr + '</p>'
                }


            },
            { "data": "assetEvaluation.evaluatingOrgName", "width": "15%" },
            { "data": "assetEvaluation.reportDateStr", "width": "15%" },
            { "data": "assetEvaluation.reportRegNumber", "width": "15%" },
            { "data": "assetEvaluation.marketValue", "width": "15%" },
            { "data": "assetEvaluation.examiningOrgName", "width": "15%" },
            { "data": "assetEvaluation.examReportDateStr", "width": "15%" },
            { "data": "assetEvaluation.examReportRegNumber", "width": "15%" },
            


            {

                "data": {

                    reportFileId: "assetEvaluation.reportFileId",
                    examReportFileId: "assetEvaluation.examReportFileId",

                },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.assetEvaluation.reportFileId}') style="cursor: pointer;"> Баҳолаш ҳисоботи <i class="fa fa-download"></i></a>

                                            </div>
                                        </div>

                                    </div>
                                  </div>

                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.assetEvaluation.examReportFileId}') style="cursor: pointer;"> Экспертиза ҳулосаси <i class="fa fa-download"></i></a>

                                            </div>
                                        </div>

                                    </div>
                                  </div>


                            </div >
                            
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
        "lengthMenu": [15, 20, 30, 40, 50]
    });

    dataTable3 = $('#myTable3').DataTable({
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
        "dom": '<lf<rt>ip>',
        scrollX: false,


        "ajax": {
            "url": "/SimpleUser/AssetEvaluations/GetUnSentShares/"
        },
        "columns": [
            { "data": "share.businessEntityName", "width": "15%" },
            {
                "data": {
                    "reportStatusStr": "assetEvaluation.reportStatusStr",
                    "reportStatus": "assetEvaluation.reportStatus"
                },



                "render": function (data) {

                    if (data.assetEvaluation.reportStatus)
                        return ' <p class="badge badge-success">' + data.assetEvaluation.reportStatusStr + '</p>'

                    else
                        return ' <p class="badge badge-danger">' + data.assetEvaluation.reportStatusStr + '</p>'
                }


            },
            { "data": "assetEvaluation.evaluatingOrgName", "width": "15%" },
            { "data": "assetEvaluation.reportDateStr", "width": "15%" },
            { "data": "assetEvaluation.reportRegNumber", "width": "15%" },
            { "data": "assetEvaluation.marketValue", "width": "15%" },
            { "data": "assetEvaluation.examiningOrgName", "width": "15%" },
            { "data": "assetEvaluation.examReportDateStr", "width": "15%" },
            { "data": "assetEvaluation.examReportRegNumber", "width": "15%" },
            


            {

                "data": {

                    reportFileId: "assetEvaluation.reportFileId",
                    examReportFileId: "assetEvaluation.examReportFileId",

                },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.assetEvaluation.reportFileId}') style="cursor: pointer;"> Баҳолаш ҳисоботи <i class="fa fa-download"></i></a>

                                            </div>
                                        </div>

                                    </div>
                                  </div>

                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.assetEvaluation.examReportFileId}') style="cursor: pointer;"> Экспертиза ҳулосаси <i class="fa fa-download"></i></a>

                                            </div>
                                        </div>

                                    </div>
                                  </div>


                            </div >
                            
                       `
                }
            },

            {
                "data": {
                    assetEvaluationId: "assetEvaluation.assetEvaluationId",
                    shareId: "share.shareId",
                    target: "target"
                },
                "render": function (data) {
                    return `
                            <div >
                                <a onclick=Edit(${data.assetEvaluation.assetEvaluationId}` + `,` + `${data.share.shareId}` + `,` + `${data.target}` + `)` + ` id="edit" class="btn btn-warning-gradien" style="cursor:pointer; width:150px">
                                    <i class="fas fa-edit"></i> Ўзгартириш
                                </a>

                                <a onclick=Delete('/SimpleUser/AssetEvaluations/Delete/${data.assetEvaluation.assetEvaluationId}') id="delete" class="btn btn-danger-gradien" style="cursor:pointer; width:140px">
                                    <i class="fas fa-trash"></i> Ўчириш
                                </a>

                                <a onclick=Send(${data.assetEvaluation.assetEvaluationId}` + `,` + `${data.target}) id="send" class="btn btn-success-gradien" style="cursor:pointer; width:140px">
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
        "lengthMenu": [15, 20, 30, 40, 50]
    });
    dataTable4 = $('#myTable4').DataTable({
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
        "dom": '<lf<rt>ip>',
        scrollX: false,


        "ajax": {
            "url": "/SimpleUser/assetEvaluations/GetSentShares/"
        },
        "columns": [
            { "data": "assetEvaluation.share.businessEntityName", "width": "15%" },
            {
                "data": {
                    "reportStatusStr": "assetEvaluation.reportStatusStr",
                    "reportStatus": "assetEvaluation.reportStatus"
                },



                "render": function (data) {

                    if (data.assetEvaluation.reportStatus)
                        return ' <p class="badge badge-success">' + data.assetEvaluation.reportStatusStr + '</p>'

                    else
                        return ' <p class="badge badge-danger">' + data.assetEvaluation.reportStatusStr + '</p>'
                }


            },
            { "data": "assetEvaluation.evaluatingOrgName", "width": "15%" },
            { "data": "assetEvaluation.reportDateStr", "width": "15%" },
            { "data": "assetEvaluation.reportRegNumber", "width": "15%" },
            { "data": "assetEvaluation.marketValue", "width": "15%" },
            { "data": "assetEvaluation.examiningOrgName", "width": "15%" },
            { "data": "assetEvaluation.examReportDateStr", "width": "15%" },
            { "data": "assetEvaluation.examReportRegNumber", "width": "15%" },

            


            {

                "data": {

                    reportFileId: "assetEvaluation.reportFileId",
                    examReportFileId: "assetEvaluation.examReportFileId",

                },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.assetEvaluation.reportFileId}') style="cursor: pointer;"> Баҳолаш ҳисоботи <i class="fa fa-download"></i></a>

                                            </div>
                                        </div>

                                    </div>
                                  </div>

                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.assetEvaluation.examReportFileId}') style="cursor: pointer;"> Экспертиза ҳулосаси <i class="fa fa-download"></i></a>

                                            </div>
                                        </div>

                                    </div>
                                  </div>


                            </div >
                            
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
        "lengthMenu": [15, 20, 30, 40, 50]
    });

}



function DownloadFile(id) {

    window.location.href = "/SimpleUser/AssetEvaluations/DownloadFile/" + id;


}
function Edit(id, id1, target) {

    window.location.href = "/SimpleUser/AssetEvaluations/Edit?id=" + id + "&target=" + target;

}

function Details(id) {

    window.location.href = "/SimpleUser/AssetEvaluations/Details/" + id;

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
    }).then(function(willDelete) {
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
                        dataTable3.ajax.reload();
                        dataTable4.ajax.reload();
                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });

}

function Send(id, target) {
    Swal.fire({
        title: 'Киритилган маълумотларини тасдиқлайсизми?',
        text: "Маълумотларни тасдиқлагач, уларни ўзгартира олмайсиз!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',

    }).then(function (willSend) {
        if (willSend.isConfirmed) {

            data = {
                id: id,
                target:target
            }

            $.ajax({
                type: "POST",
                url: '/SimpleUser/AssetEvaluations/Confirm/',
                data: JSON.stringify(data),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {
                        Swal.fire("Тасдиқланди!", data.message, {
                            icon: "success",
                        });
                        /* toastr.success(data.message);*/
                        dataTable1.ajax.reload();
                        dataTable2.ajax.reload();
                        dataTable3.ajax.reload();
                        dataTable4.ajax.reload();
                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });
}

