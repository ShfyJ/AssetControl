$(document).ready(function () {
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
    });
});




$(document).ready(function () {
    $('#myTable').DataTable({
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
        dom: 'Bfrtip',
        scrollX: false,
       "dom": '<lf<Brt>ip>',
       
        buttons: [
            //{
            //    extend: 'print',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="glyphicon glyphicon-print" data-toggle="tooltip" data-placement="left" title="Печать"></span>', "className": 'btn btn-default btn-xs',

            //    customize: function (win) {
            //        $(win.document.body)
            //            .css('font-size', '10pt')
            //            .prepend(
            //                '<img src="http://datatables.net/media/images/logo-fade.png" style="position:absolute; top:0; left:0;" />'
            //            );

            //        $(win.document.body).find('table')
            //            .addClass('compact')
            //            .css('font-size', 'inherit');
            //    }
            //},
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-copy" style="color:grey" data-toggle="tooltip" data-placement="left" title="Копировать"></span>', "className": 'btn btn-default btn-xs',
            //},
            //{
            //    extend: 'excel',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-excel" style="color:green" data-toggle="tooltip" data-placement="left" title="Excel"></span>',
            //    className: 'btn btn-default btn-xs',
            //},
            //{
            //    extend: 'pdf',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-pdf" style="color:#960606" data-toggle="tooltip" data-placement="left" title="PDF"></span>', "className": 'btn btn-default btn-xs',
            //},

            //{
            //    extend: 'colvis',
            //    text: '<span class="fas fa-check-square" style="color:blue" data-toggle="tooltip" data-placement="left" title="Видимость столбца"> Лист</span>', "className": 'btn btn-default btn-xs',

            //}


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
});

$(document).ready(function () {
    $('#myTable2').DataTable({
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
        dom: 'Bfrtip',
         "dom": '<lf<Brt>ip>',
        buttons: [
        //    {
        //        extend: 'print',
        //        exportOptions: {
        //            columns: ':visible'
        //        },
        //        text: '<span class="glyphicon glyphicon-print" data-toggle="tooltip" data-placement="left" title="Печать"></span>', "className": 'btn btn-default btn-xs',

        //        customize: function (win) {
        //            $(win.document.body)
        //                .css('font-size', '10pt')
        //                .prepend(
        //                    '<img src="http://datatables.net/media/images/logo-fade.png" style="position:absolute; top:0; left:0;" />'
        //                );

        //            $(win.document.body).find('table')
        //                .addClass('compact')
        //                .css('font-size', 'inherit');
        //        }
        //    },
        //    {
        //        extend: 'copy',
        //        exportOptions: {
        //            columns: ':visible'
        //        },
        //        text: '<span class="fas fa-copy" style="color:grey" data-toggle="tooltip" data-placement="left" title="Копировать"></span>', "className": 'btn btn-default btn-xs',
        //    },
        //    {
        //        extend: 'excel',
        //        exportOptions: {
        //            columns: ':visible'
        //        },
        //        text: '<span class="fas fa-file-excel" style="color:green" data-toggle="tooltip" data-placement="left" title="Excel"></span>',
        //        className: 'btn btn-default btn-xs',
             
        //    },
        //    {
        //        extend: 'pdf',
        //        exportOptions: {
        //            columns: ':visible'
        //        },
        //        text: '<span class="fas fa-file-pdf" style="color:#960606" data-toggle="tooltip" data-placement="left" title="PDF"></span>', "className": 'btn btn-default btn-xs',
        //    },

        //    {
        //        extend: 'colvis',
        //        text: '<span class="fas fa-check-square" style="color:blue" data-toggle="tooltip" data-placement="left" title="Видимость столбца"> Лист</span>', "className": 'btn btn-default btn-xs',

        //    }


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
});

$(document).ready(function () {
    $('#myTable_1').DataTable({
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
        dom: 'Bfrtip',
        "dom": '<lf<Brt>ip>',

        buttons: [
            //{
            //    extend: 'print',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="glyphicon glyphicon-print" data-toggle="tooltip" data-placement="left" title="Печать"></span>', "className": 'btn btn-default btn-xs',

            //    customize: function (win) {
            //        $(win.document.body)
            //            .css('font-size', '10pt')
            //            .prepend(
            //                '<img src="http://datatables.net/media/images/logo-fade.png" style="position:absolute; top:0; left:0;" />'
            //            );

            //        $(win.document.body).find('table')
            //            .addClass('compact')
            //            .css('font-size', 'inherit');
            //    }
            //},
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-copy" style="color:grey" data-toggle="tooltip" data-placement="left" title="Копировать"></span>', "className": 'btn btn-default btn-xs',
            //},
            //{
            //    extend: 'excel',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-excel" style="color:green" data-toggle="tooltip" data-placement="left" title="Excel"></span>',
            //    className: 'btn btn-default btn-xs',
               
            //},
            //{
            //    extend: 'pdf',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-pdf" style="color:#960606" data-toggle="tooltip" data-placement="left" title="PDF"></span>', "className": 'btn btn-default btn-xs',
            //},

            //{
            //    extend: 'colvis',
            //    text: '<span class="fas fa-check-square" style="color:blue" data-toggle="tooltip" data-placement="left" title="Видимость столбца"> Лист</span>', "className": 'btn btn-default btn-xs',

            //}


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
});

$(document).ready(function () {
    $('#myTable1_').DataTable({
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
        dom: 'Bfrtip',
        "dom": '<lf<Brt>ip>',

        buttons: [
            //{
            //    extend: 'print',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="glyphicon glyphicon-print" data-toggle="tooltip" data-placement="left" title="Печать"></span>', "className": 'btn btn-default btn-xs',

            //    customize: function (win) {
            //        $(win.document.body)
            //            .css('font-size', '10pt')
            //            .prepend(
            //                '<img src="http://datatables.net/media/images/logo-fade.png" style="position:absolute; top:0; left:0;" />'
            //            );

            //        $(win.document.body).find('table')
            //            .addClass('compact')
            //            .css('font-size', 'inherit');
            //    }
            //},
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-copy" style="color:grey" data-toggle="tooltip" data-placement="left" title="Копировать"></span>', "className": 'btn btn-default btn-xs',
            //},
            //{
            //    extend: 'excel',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-excel" style="color:green" data-toggle="tooltip" data-placement="left" title="Excel"></span>',
            //    className: 'btn btn-default btn-xs',
              
            //},
            //{
            //    extend: 'pdf',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-pdf" style="color:#960606" data-toggle="tooltip" data-placement="left" title="PDF"></span>', "className": 'btn btn-default btn-xs',
            //},

            //{
            //    extend: 'colvis',
            //    text: '<span class="fas fa-check-square" style="color:blue" data-toggle="tooltip" data-placement="left" title="Видимость столбца"> Лист</span>', "className": 'btn btn-default btn-xs',

            //}


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
});
$(document).ready(function () {
    $('#myTable2_').DataTable({
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
        dom: 'Bfrtip',
        "dom": '<lf<Brt>ip>',

        buttons: [
            //{
            //    extend: 'print',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="glyphicon glyphicon-print" data-toggle="tooltip" data-placement="left" title="Печать"></span>', "className": 'btn btn-default btn-xs',

            //    customize: function (win) {
            //        $(win.document.body)
            //            .css('font-size', '10pt')
            //            .prepend(
            //                '<img src="http://datatables.net/media/images/logo-fade.png" style="position:absolute; top:0; left:0;" />'
            //            );

            //        $(win.document.body).find('table')
            //            .addClass('compact')
            //            .css('font-size', 'inherit');
            //    }
            //},
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-copy" style="color:grey" data-toggle="tooltip" data-placement="left" title="Копировать"></span>', "className": 'btn btn-default btn-xs',
            //},
            //{
            //    extend: 'excel',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-excel" style="color:green" data-toggle="tooltip" data-placement="left" title="Excel"></span>',
            //    className: 'btn btn-default btn-xs',
                
            //},
            //{
            //    extend: 'pdf',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-pdf" style="color:#960606" data-toggle="tooltip" data-placement="left" title="PDF"></span>', "className": 'btn btn-default btn-xs',
            //},

            //{
            //    extend: 'colvis',
            //    text: '<span class="fas fa-check-square" style="color:blue" data-toggle="tooltip" data-placement="left" title="Видимость столбца"> Лист</span>', "className": 'btn btn-default btn-xs',

            //}


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
});

$(document).ready(function () {
    $('#myTable_2').DataTable({
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
        dom: 'Bfrtip',
        "dom": '<lf<Brt>ip>',

        buttons: [
            //{
            //    extend: 'print',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="glyphicon glyphicon-print" data-toggle="tooltip" data-placement="left" title="Печать"></span>', "className": 'btn btn-default btn-xs',

            //    customize: function (win) {
            //        $(win.document.body)
            //            .css('font-size', '10pt')
            //            .prepend(
            //                '<img src="http://datatables.net/media/images/logo-fade.png" style="position:absolute; top:0; left:0;" />'
            //            );

            //        $(win.document.body).find('table')
            //            .addClass('compact')
            //            .css('font-size', 'inherit');
            //    }
            //},
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-copy" style="color:grey" data-toggle="tooltip" data-placement="left" title="Копировать"></span>', "className": 'btn btn-default btn-xs',
            //},
            //{
            //    extend: 'excel',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-excel" style="color:green" data-toggle="tooltip" data-placement="left" title="Excel"></span>',
            //    className: 'btn btn-default btn-xs',
                
            //},
            //{
            //    extend: 'pdf',
            //    exportOptions: {
            //        columns: ':visible'
            //    },
            //    text: '<span class="fas fa-file-pdf" style="color:#960606" data-toggle="tooltip" data-placement="left" title="PDF"></span>', "className": 'btn btn-default btn-xs',
            //},

            //{
            //    extend: 'colvis',
            //    text: '<span class="fas fa-check-square" style="color:blue" data-toggle="tooltip" data-placement="left" title="Видимость столбца"> Лист</span>', "className": 'btn btn-default btn-xs',

            //}


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
});
