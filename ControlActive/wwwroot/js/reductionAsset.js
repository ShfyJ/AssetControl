var dataTable1;
var dataTable2;
var dataTable3;
var dataTable4;
var dataTable5;
var dataTable6;

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
            "url": "/SimpleUser/ReductionInAssets/GetRealEstates/",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (d) {
                d = 0;
                return JSON.stringify(d);
            },
        },
        "columns": [
            {
                "data": { name: "name" },
                "render": function (data) {
                    return '<h6 class="task_title_0 loader-box" style="height:10px">' + data.name + '<span style="width:30px; height:30px" class="loader-6"></span></h6>'
                        
                        
                }
            },
            {
                "data":"reductionInAsset.status",

                "render": function (data) {
                    
                    return ' <p class="badge badge-danger">тасдиқланмаган</p>'
                   
                }

            },
            { "data": "reductionInAsset.assetValueAfterDecline", "width": "15%" },
            { "data": "reductionInAsset.governingBodyName", "width": "15%" },
            { "data": "reductionInAsset.solutionNumber", "width": "15%" },
            { "data": "reductionInAsset.solutionDateStr", "width": "15%" },
            { "data": "reductionInAsset.percentage", "width": "15%" },
            
            { "data": "reductionInAsset.amount", "width": "15%" },
            { "data": "reductionInAsset.numberOfSteps", "width": "15%" },


            {
                "data": { solutionFileid: "reductionInAsset.solutionFileId" },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.reductionInAsset.solutionFileId}') style="cursor: pointer;">Қарор <i class="fa fa-download"></i></a>

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
                    reductionInAssetId: "reductionInAsset.reductionInAssetId",
                    target: "target"
                },
                "render": function (data) {
                    return `
                            <div >
                                <a onclick=Edit(${data.reductionInAsset.reductionInAssetId}` + `,` + `${data.target}` + `)` + ` id="edit" class="btn btn-warning-gradien" style="cursor:pointer; width:150px">
                                    <i class="fas fa-edit"></i> Таҳрирлаш
                                </a>

                                <a onclick=Delete(${data.reductionInAsset.reductionInAssetId}` + `,` + `${data.target}` + `) id="delete" class="btn btn-danger-gradien" style="cursor:pointer; width:140px">
                                    <i class="fas fa-trash"></i> Ўчириш
                                </a>

                                <a onclick=Send(${data.reductionInAsset.reductionInAssetId}` + `,` + `${data.target}` + `)` + ` id="send" class="btn btn-success-gradien" style="cursor:pointer; width:140px">
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
        "lengthMenu": [5, 10, 20, 30, 40]
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
            "url": "/SimpleUser/ReductionInAssets/GetRealEstates/",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function(d) {
                d = 1;
                return JSON.stringify(d);
            },
            
        },
        "columns": [
            { "data": "name", "width": "15%" },
            {
                "data": "reductionInAsset.status",

                "render": function (data) {

                    return ' <p class="badge badge-success">Амалда</p><span style="font-size:10px; color:green" >тасдиқланган</span>'

                }

            },
            { "data": "reductionInAsset.assetValueAfterDecline", "width": "15%" },
            { "data": "reductionInAsset.governingBodyName", "width": "15%" },
            { "data": "reductionInAsset.solutionNumber", "width": "15%" },
            { "data": "reductionInAsset.solutionDateStr", "width": "15%" },
            { "data": "reductionInAsset.percentage", "width": "15%" },

            { "data": "reductionInAsset.amount", "width": "15%" },
            { "data": "reductionInAsset.numberOfSteps", "width": "15%" },


            {
                "data": { solutionFileid: "reductionInAsset.solutionFileId" },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.reductionInAsset.solutionFileId}') style="cursor: pointer;">Қарор <i class="fa fa-download"></i></a>

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
            "url": "/SimpleUser/ReductionInAssets/GetRealEstates/",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data":function(d) {
                d = 2;
                return JSON.stringify(d);
            },
          
        },
        "columns": [
            { "data": "name", "width": "15%" },
            {
                "data": "reductionInAsset.status",

                "render": function (data) {

                    return ' <p class="badge badge-danger">нофаол</p>'

                }

            },
            { "data": "reductionInAsset.assetValueAfterDecline", "width": "15%" },
            { "data": "reductionInAsset.governingBodyName", "width": "15%" },
            { "data": "reductionInAsset.solutionNumber", "width": "15%" },
            { "data": "reductionInAsset.solutionDateStr", "width": "15%" },
            { "data": "reductionInAsset.percentage", "width": "15%" },

            { "data": "reductionInAsset.amount", "width": "15%" },
            { "data": "reductionInAsset.numberOfSteps", "width": "15%" },


            {
                "data": { solutionFileid: "reductionInAsset.solutionFileId" },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.reductionInAsset.solutionFileId}') style="cursor: pointer;">Қарор <i class="fa fa-download"></i></a>

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
            "url": "/SimpleUser/ReductionInAssets/GetShares/",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (d) {
                d = 0;
                return JSON.stringify(d);
            },
            
        },
        "columns": [
            {
                "data": { name: "name" },
                "render": function (data) {
                    return '<h6 class="task_title_0 loader-box" style="height:10px">' + data.name + '<span style="width:30px; height:30px" class="loader-6"></span></h6>'


                }
            },
            {
                "data": "reductionInAsset.status",

                "render": function (data) {

                    return ' <p class="badge badge-danger">тасдиқланмаган</p>'

                }

            },
            { "data": "reductionInAsset.assetValueAfterDecline", "width": "15%" },
            { "data": "reductionInAsset.governingBodyName", "width": "15%" },
            { "data": "reductionInAsset.solutionNumber", "width": "15%" },
            { "data": "reductionInAsset.solutionDateStr", "width": "15%" },
            { "data": "reductionInAsset.percentage", "width": "15%" },

            { "data": "reductionInAsset.amount", "width": "15%" },
            { "data": "reductionInAsset.numberOfSteps", "width": "15%" },


            {
                "data": { solutionFileid: "reductionInAsset.solutionFileId" },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.reductionInAsset.solutionFileId}') style="cursor: pointer;">Қарор <i class="fa fa-download"></i></a>

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
                    reductionInAssetId: "reductionInAsset.reductionInAssetId",
                    target: "target"
                },
                "render": function (data) {
                    return `
                            <div >
                                <a onclick=Edit(${data.reductionInAsset.reductionInAssetId}` + `,` + `${data.target}` + `)` + ` id="edit" class="btn btn-warning-gradien" style="cursor:pointer; width:150px">
                                    <i class="fas fa-edit"></i> Таҳрирлаш
                                </a>

                                <a onclick=Delete(${data.reductionInAsset.reductionInAssetId}` + `,` + `${data.target}` + `) id="delete" class="btn btn-danger-gradien" style="cursor:pointer; width:140px">
                                    <i class="fas fa-trash"></i> Ўчириш
                                </a>

                                <a onclick=Send(${data.reductionInAsset.reductionInAssetId}` + `,` + `${data.target}` + `)` + ` id="send" class="btn btn-success-gradien" style="cursor:pointer; width:140px">
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
    dataTable5 = $('#myTable5').DataTable({
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
            "url": "/SimpleUser/ReductionInAssets/GetShares/",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (d) {
                d = 1;
                return JSON.stringify(d);
            },
        },
        "columns": [
            { "data": "name", "width": "15%" },
            {
                "data": "reductionInAsset.status",

                "render": function (data) {

                    return ' <p class="badge badge-success">Амалда</p><span style="font-size:10px; color:green" >тасдиқланган</span>'

                }

            },
            { "data": "reductionInAsset.assetValueAfterDecline", "width": "15%" },
            { "data": "reductionInAsset.governingBodyName", "width": "15%" },
            { "data": "reductionInAsset.solutionNumber", "width": "15%" },
            { "data": "reductionInAsset.solutionDateStr", "width": "15%" },
            { "data": "reductionInAsset.percentage", "width": "15%" },

            { "data": "reductionInAsset.amount", "width": "15%" },
            { "data": "reductionInAsset.numberOfSteps", "width": "15%" },


            {
                "data": { solutionFileid: "reductionInAsset.solutionFileId" },

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.reductionInAsset.solutionFileId}') style="cursor: pointer;">Қарор <i class="fa fa-download"></i></a>

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
    dataTable6 = $('#myTable6').DataTable({
        
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
            "url": "/SimpleUser/ReductionInAssets/GetShares/",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (d) {
                d = 2;
                return JSON.stringify(d);
            },
        },
        "columns": [
            { "data": "name", "width": "15%" },
            {
                "data": "reductionInAsset.status",

                "render": function (data) {

                    return ' <p class="badge badge-danger">нофаол</p>'

                }

            },
            { "data": "reductionInAsset.assetValueAfterDecline", "width": "15%" },
            { "data": "reductionInAsset.governingBodyName", "width": "15%" },
            { "data": "reductionInAsset.solutionNumber", "width": "15%" },
            { "data": "reductionInAsset.solutionDateStr", "width": "15%" },
            { "data": "reductionInAsset.percentage", "width": "15%" },

            { "data": "reductionInAsset.amount", "width": "15%" },
            { "data": "reductionInAsset.numberOfSteps", "width": "15%" },


            {
                "data": { solutionFileid: "reductionInAsset.solutionFileId"},

                "render": function (data) {
                    return `
                            <div class="row">
                                <div class="col-sm-2" >
                                    <div class="thumbnail">
                                        <div class="image view view-first">
                                            <img style="width: 100%; display: block;" src="/images/document.png" alt="image" />
                                            <div class="mask">
                                                <a onclick=DownloadFile('${data.reductionInAsset.solutionFileId}') style="cursor: pointer;">Қарор <i class="fa fa-download"></i></a>

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

    window.location.href = "/SimpleUser/ReductionInAssets/DownloadFile/" + id;


}
function Edit(id, target) {

    window.location.href = "/SimpleUser/ReductionInAssets/Edit?id=" + id + "&target=" + target;

}

function Details(id) {

    window.location.href = "/SimpleUser/ReductionInAssets/Details/" + id;

}


function Delete(id, target) {
    Swal.fire({
        title: "Ўчиришни тасдиқлайсизми?",
        text: "Ўчирилгач маълумотларни қайта тиклай олмайсиз! Бу объектга боғланган барча маълумотлар базадан ўчирилади!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (willDelete) {
        if (willDelete.isConfirmed) {
            var urlCall = '/SimpleUser/ReductionInAssets/Delete/';
            $.ajax({
                type: "DELETE",
                url: urlCall + '?' + $.param({ "Id": id, "target": target }),
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
                        dataTable5.ajax.reload();
                        dataTable6.ajax.reload();
                        
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
            var data = {
                id: id,
                target: target
            };
            $.ajax({
                type: "POST",
                url: '/SimpleUser/ReductionInAssets/Send/',
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
                        dataTable5.ajax.reload();
                        dataTable6.ajax.reload();
                       
                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });
}



