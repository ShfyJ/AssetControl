var dataTable1;
var dataTable2;



$(document).ready(function () {

    loadDataTable();

    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust().responsive.recalc();
    });

    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();

});



//$(document).ready(function () {

//});


function loadDataTable() {
    dataTable1 = $('#table1').DataTable({
        
        "dom": '<lf<rt>ip>',
        responsive: true,
        "autoWidth": false,
        columnDefs: [
            { width: '10%', targets: 0 }, //step 2, column 1 out of 4
            { width: '5%', targets: 1 },
            { width: '30%', targets: 2}, //step 2, column 2 out of 4
            { width: '5%', targets: 3}, //step 2, column 3 out of 4
            { width: '10%', targets: 4 }, //step 2, column 3 out of 4
            

        ],

        "order": [],

        "ajax": {
            "url": "/Admin/Audit/GetAssetLogs"
        },
        
        deferRender: true,
        "columns": [
            {
                "data": {},
                "render": function (data) {
                    if(data.actionName == "Create")
                        return '<span class="badge rounded-pill badge-success">Яратилди</span>'
                    if (data.actionName == "Update")
                        return '<span class="badge rounded-pill badge-warning">Таҳрирланди</span>'
                    if (data.actionName == "Delete")
                        return '<span class="badge rounded-pill badge-danger">Ўчирилди</span>'
                }
            },
            { "data": "userFullName", "width": "15%" },
            { "data": "assetName", "width": "15%" },
            { "data": "tableName", "width": "15%" },
            { "data": "dateTime", "width": "15%" },
            { "data": "oldValue", "width": "15%" },
            { "data": "newValue", "width": "15%" },
            { "data": "affectedColumn", "width": "15%" },

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
        "lengthMenu": [30, 40, 50]
    });


    dataTable2 = $('#table2').DataTable({
        
        "dom": '<lf<rt>ip>',
        responsive: true,
        "autoWidth": false,
        columnDefs: [
            { width: '10%', targets: 0 }, //step 2, column 1 out of 4
            { width: '5%', targets: 1 },
            { width: '30%', targets: 2 }, //step 2, column 2 out of 4
            { width: '5%', targets: 3 }, //step 2, column 3 out of 4
            { width: '10%', targets: 4 }, //step 2, column 3 out of 4


        ],

        "order": [],

        "ajax": {
            "url": "/Admin/Audit/GetAllLogs"
        },
        deferRender: true,
        "columns": [
            {
                "data": {},
                "render": function (data) {
                    if (data.actionName == "Create")
                        return '<span class="badge rounded-pill badge-success">Яратилди</span>'
                    if (data.actionName == "Update")
                        return '<span class="badge rounded-pill badge-warning">Таҳрирланди</span>'
                    if (data.actionName == "Delete")
                        return '<span class="badge rounded-pill badge-danger">Ўчирилди</span>'
                }
            },
            { "data": "userFullName", "width": "15%" },
            { "data": "tableName", "width": "15%" },
            { "data": "dateTime", "width": "15%" },
            { "data": "oldValue", "width": "15%" },
            { "data": "newValue", "width": "15%" },
            { "data": "affectedColumn", "width": "15%" },
            { "data": "primaryKey", "width": "15%" },
            
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
        "lengthMenu": [30, 40, 50]
    });

}




