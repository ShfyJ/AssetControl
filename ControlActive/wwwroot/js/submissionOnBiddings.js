
var dataTable1;
var dataTable2;
var dataTable3;
var dataTable4;
var dataTable5;
var dataTable6;

$(document).ready(function () {

    loadDataTable();
    
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust().responsive.recalc();
    });

    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
    $('[data-bs-toggle="tooltip"]').tooltip();
    
});





function loadDataTable() {
    dataTable1 = $('#myTable1').DataTable({
        columnDefs: [
            {
                targets: [0],
                orderData: [0, 1],
            },
        ],
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

        drawCallback: function (settings, json) {
          
            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },
        
        
        "dom": '<l<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/SubmissionOnBiddings/GetOnSaleRealEstates"
        },
        "columns": [
            {
                "data": {
                    "id":"submissionOnBidding.submissionOnBiddingId"

                },
                "render": function (data) {
                    return '<h5>#' + data.submissionOnBidding.submissionOnBiddingId + '</h5>'
                }
            },
            {
                "data": {},

                "render": function (data) {

                    if (data.submissionOnBidding.confirmed && data.submissionOnBidding.daysLeft < 0)
                        return ' <h6 class="task_title_0">' + data.realEstate.realEstateName + ' <i style="color: green" class="fa fa-check-circle-o fa-lg"></i><a style="cursor:pointer; color: #fc6b03" onclick=SendResult(' + data.submissionOnBidding.submissionOnBiddingId + ','+data.target+')  data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион натижасини юбориш"><i class="fa fa-star-o fa-spin fa-lg"></i></a></h6 ><p class="project_name_0 badge badge-success" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'

                    if (data.submissionOnBidding.confirmed)
                        return ' <h6 class="task_title_0">' + data.realEstate.realEstateName + ' <a style="cursor:pointer; color: green"><i class="fa fa-check-circle-o fa-lg"></i></a></h6 ><p class="project_name_0 badge badge-success" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'
                    else {
                        return ' <h6 style="margin-bottom:-17px;" class="task_title_0">' + data.realEstate.realEstateName + ' <a onclick=Send(' + data.submissionOnBidding.submissionOnBiddingId + ',' + '1)  data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="тасдиқлаш" class="switch" style="cursor:pointer;"><i id="icon_on"  class="fa fa-check-circle-o fa-lg"></i><i id="icon_off" style="color:red" class="fa fa-times-circle fa-spin fa-lg"></i></a></h6 ><span style="font-size:10px; color:red" >тасдиқланмаган</span></br><p class="project_name_0 badge badge-success" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'
                    }
                }

            },
            

            { "data": "submissionOnBidding.biddingExposureDateStr", "width": "15%" },
            { "data": "submissionOnBidding.tradingPlatformName", "width": "15%" },

            {
                "data":
                {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "holdDate": "submissionOnBidding.biddingHoldDateStr"
                },
                
                "render": function (data) {
                    if (data.submissionOnBidding.daysLeft > 0)
                        return '<h3 style="color:#6362e7">' + data.submissionOnBidding.daysLeft + '<span style="font-size:14px"> кун қолди</span></h3 ><p class="task_desc_0" style="color:#7dc006"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft == 0)
                        return '<h3 style="color:green">Аукцион куни</h3 ><p class="task_desc_0" style="color:blue"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft < 0)
                        return '<h4 style="color:#black">Аукцион тугади</h4 ><p class="task_desc_0" style="color:#fc5a03"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                }
            },

            {
                "data": {
                    "activeValue" : "submissionOnBidding.activeValue"
                },
                "render": function (data) {
                    return '<h5><mark>' + data.submissionOnBidding.activeValue +' сўм</mark ></h5 > '
                }
            },

            {
                "data": {},
                "render": function (data) {

                    if (data.submissionOnBidding.confirmed)
                    return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                    else return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                        + '&nbsp&nbsp<span ><a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўзгартириш" style="cursor:pointer" onclick=Edit(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.realEstate.realEstateId + ',' + data.target + ')><i style="color:#eb8c21; " class="fa fa-edit fa-lg"></i></a></span>'
                        + '&nbsp&nbsp<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" style="cursor:pointer" onclick=Delete(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.target +')><i  style="color:#fc5a03" class="fa fa-trash-o fa-lg"></i></a>'

                }
            },

        ],

        "language": {
            "lengthMenu": "",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "",
        },
        "lengthMenu": [4, 20, 30, 40, 50]
    });

    dataTable2 = $('#myTable2').DataTable({
        columnDefs: [
            {
                targets: [0],
                orderData: [0, 1],
            },
        ],
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
                },
                display: $.fn.dataTable.Responsive.display.childRowImmediate,
                type: 'none',
                target: ''
            }

        },

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },


        "dom": '<l<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/SubmissionOnBiddings/GetOnSaleShares"
        },
        "columns": [
            {
                "data": {
                    "id": "submissionOnBidding.submissionOnBiddingId"

                },
                "render": function (data) {
                    return '<h5>#' + data.submissionOnBidding.submissionOnBiddingId + '</h5>'
                }
            },
            {
                "data": {},

                "render": function (data) {

                    if (data.submissionOnBidding.confirmed && data.submissionOnBidding.daysLeft < 0)
                        return ' <h6 class="task_title_0">' + data.share.businessEntityName + ' <i style="color: green" class="fa fa-check-circle-o fa-lg"></i><a style="cursor:pointer; color: #fc6b03" onclick=SendResult(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.target + ')  data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион натижасини юбориш"><i class="fa fa-star-o fa-spin fa-lg"></i></a></h6 ><p class="project_name_0 badge badge-success" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'

                    if (data.submissionOnBidding.confirmed)
                        return ' <h6 class="task_title_0">' + data.share.businessEntityName + ' <a style="cursor:pointer; color: green"><i class="fa fa-check-circle-o fa-lg"></i></a></h6 ><p class="project_name_0 badge badge-success" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'
                    else {
                        return ' <h6 style="margin-bottom:-17px;" class="task_title_0">' + data.share.businessEntityName + ' <a onclick=Send(' + data.submissionOnBidding.submissionOnBiddingId +","+'2)  data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="тасдиқлаш" class="switch" style="cursor:pointer;"><i id="icon_on"  class="fa fa-check-circle-o fa-lg"></i><i id="icon_off" style="color:red" class="fa fa-times-circle fa-spin fa-lg"></i></a></h6 ><span style="font-size:10px; color:red" >тасдиқланмаган</span></br><p class="project_name_0 badge badge-success" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'
                    }
                }

            },


            { "data": "submissionOnBidding.biddingExposureDateStr", "width": "15%" },
            { "data": "submissionOnBidding.tradingPlatformName", "width": "15%" },

            {
                "data":
                {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "holdDate": "submissionOnBidding.biddingHoldDateStr"
                },

                "render": function (data) {
                    if (data.submissionOnBidding.daysLeft > 0)
                        return '<h3 style="color:#6362e7">' + data.submissionOnBidding.daysLeft + '<span style="font-size:14px"> кун қолди</span></h3 ><p class="task_desc_0" style="color:#7dc006"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft == 0)
                        return '<h3 style="color:green">Аукцион куни</h3 ><p class="task_desc_0" style="color:blue"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft < 0)
                        return '<h4 style="color:#black">Аукцион тугади</h4 ><p class="task_desc_0" style="color:#fc5a03"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                }
            },

            {
                "data": {
                    "activeValue": "submissionOnBidding.activeValue"
                },
                "render": function (data) {
                    return '<h5><mark>' + data.submissionOnBidding.activeValue + ' сўм</mark ></h5 > '
                }
            },

            {
                "data": {
                    "url": "submissionOnBidding.url",
                    "submissionOnBiddingId": "submissionOnBidding.submissionOnBiddingId",
                    "confirmed": "submissionOnBidding.confirmed"
                },
                "render": function (data) {

                    if (data.submissionOnBidding.confirmed)
                        return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                    else return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                        + '&nbsp&nbsp<span data-bs-toggle="modal" data-bs-target="#exampleModal3"><a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўзгартириш" style="cursor:pointer" onclick=Edit(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.share.shareId + ',' + data.target + ')><i style="color:#eb8c21; " class="fa fa-edit fa-lg"></i></a></span>'
                        + '&nbsp&nbsp<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" style="cursor:pointer" onclick=Delete(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.target+')><i  style="color:#fc5a03" class="fa fa-trash-o fa-lg"></i></a>'

                }
            },



        ],




        "language": {
            "lengthMenu": "",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "",
        },
        "lengthMenu": [4, 20, 30, 40, 50]
    });
    
    dataTable3 = $('#myTable3').DataTable({
        columnDefs: [
            {
                targets: [0],
                orderData: [0, 1],
            },
        ],

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

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },


        "dom": '<l<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/SubmissionOnBiddings/GetSoldRealEstates"
        },
        "columns": [
            {
                "data": {
                    "id": "submissionOnBidding.submissionOnBiddingId"

                },
                "render": function (data) {
                    return '<h5>#' + data.submissionOnBidding.submissionOnBiddingId + '</h5>'
                }
            },
            {
                "data": {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "realEstateName": "realEstate.realEstateName",
                    "NumberOfAssets": "submissionOnBidding.amountOnBidding",
                    "confirmed": "submissionOnBidding.confirmed",
                    "submissionOnBiddingId": "submissionOnBidding.submissionOnBiddingId",
                },

                "render": function (data) {

                    if (data.submissionOnBidding.confirmed && data.submissionOnBidding.daysLeft < 0)
                        return ' <h6 class="task_title_0">' + data.realEstate.realEstateName + ' <i style="color: green" class="fa fa-check-square-o fa-lg"></i> </h6 ><p class="project_name_0 badge badge-success" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'

                    
                }

            },


            { "data": "submissionOnBidding.biddingExposureDateStr", "width": "15%" },
            { "data": "submissionOnBidding.tradingPlatformName", "width": "15%" },

            {
                "data":
                {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "holdDate": "submissionOnBidding.biddingHoldDateStr"
                },

                "render": function (data) {
                    if (data.submissionOnBidding.daysLeft > 0)
                        return '<h3 style="color:#6362e7">' + data.submissionOnBidding.daysLeft + '<span style="font-size:14px"> кун қолди</span></h3 ><p class="task_desc_0" style="color:#7dc006"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft == 0)
                        return '<h3 style="color:green">Аукцион куни</h3 ><p class="task_desc_0" style="color:blue"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft < 0)
                        return '<h4 style="color:#black">Аукцион тугади</h4 ><p class="task_desc_0" style="color:#fc5a03"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                }
            },

            {
                "data": {
                    "activeValue": "submissionOnBidding.activeValue"
                },
                "render": function (data) {
                    return '<h5><mark>' + data.submissionOnBidding.activeValue + ' сўм</mark ></h5 > '
                }
            },

            {
                "data": {
                    "url": "submissionOnBidding.url",
                    "submissionOnBiddingId": "submissionOnBidding.submissionOnBiddingId",
                    "confirmed": "submissionOnBidding.confirmed"
                },
                "render": function (data) {

                    if (data.submissionOnBidding.confirmed)
                        return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                    else return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                        + '&nbsp&nbsp<span data-bs-toggle="modal" data-bs-target="#exampleModal3"><a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўзгартириш" style="cursor:pointer" onclic=Edit(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.realEstate.realEstateId + ',' + data.target + ')><i style="color:#eb8c21; " class="fa fa-edit fa-lg"></i></a></span>'
                        + '&nbsp&nbsp<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" style="cursor:pointer" onclick=Delete("/SimpleUser/SubmissionOnBiddings/Delete/' + data.submissionOnBidding.submissionOnBiddingId + '")><i  style="color:#fc5a03" class="fa fa-trash-o fa-lg"></i></a>'

                }
            },



        ],




        "language": {
            "lengthMenu": "",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "",
        },
        "lengthMenu": [4, 20, 30, 40, 50]
    });

    dataTable4 = $('#myTable4').DataTable({
        order: [[0, 'desc']],
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
                },
                display: $.fn.dataTable.Responsive.display.childRowImmediate,
                type: 'none',
                target: ''
            }

        },

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },


        "dom": '<l<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/SubmissionOnBiddings/GetSoldShares"
        },
        "columns": [
            {
                "data": {
                    "id": "submissionOnBidding.submissionOnBiddingId"

                },
                "render": function (data) {
                    return '<h5>#' + data.submissionOnBidding.submissionOnBiddingId + '</h5>'
                }
            },
            {
                "data": {},

                "render": function (data) {

                    if (data.submissionOnBidding.confirmed && data.submissionOnBidding.daysLeft < 0)
                        return ' <h6 class="task_title_0">' + data.share.businessEntityName + ' <i style="color: green" class="fa fa-check-square-o fa-lg"></i> </h6 ><p class="project_name_0 badge badge-success" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'

                }

            },


            { "data": "submissionOnBidding.biddingExposureDateStr", "width": "15%" },
            { "data": "submissionOnBidding.tradingPlatformName", "width": "15%" },

            {
                "data":
                {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "holdDate": "submissionOnBidding.biddingHoldDateStr"
                },

                "render": function (data) {
                    if (data.submissionOnBidding.daysLeft > 0)
                        return '<h3 style="color:#6362e7">' + data.submissionOnBidding.daysLeft + '<span style="font-size:14px"> кун қолди</span></h3 ><p class="task_desc_0" style="color:#7dc006"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft == 0)
                        return '<h3 style="color:green">Аукцион куни</h3 ><p class="task_desc_0" style="color:blue"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft < 0)
                        return '<h4 style="color:#black">Аукцион тугади</h4 ><p class="task_desc_0" style="color:#fc5a03"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                }
            },

            {
                "data": {
                    "activeValue": "submissionOnBidding.activeValue"
                },
                "render": function (data) {
                    return '<h5><mark>' + data.submissionOnBidding.activeValue + ' сўм</mark ></h5 > '
                }
            },

            {
                "data": {
                    "url": "submissionOnBidding.url",
                    "submissionOnBiddingId": "submissionOnBidding.submissionOnBiddingId",
                    "confirmed": "submissionOnBidding.confirmed"
                },
                "render": function (data) {

                    if (data.submissionOnBidding.confirmed)
                        return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                    else return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                        + '&nbsp&nbsp<span ><a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўзгартириш" style="cursor:pointer" onclick=Edit(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.share.shareId + ',' + data.target + ')><i style="color:#eb8c21; " class="fa fa-edit fa-lg"></i></a></span>'
                        + '&nbsp&nbsp<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" style="cursor:pointer" onclick=Delete("/SimpleUser/SubmissionOnBiddings/Delete/' + data.submissionOnBidding.submissionOnBiddingId + '")><i  style="color:#fc5a03" class="fa fa-trash-o fa-lg"></i></a>'

                }
            },



        ],




        "language": {
            "lengthMenu": "",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "",
        },
        "lengthMenu": [4, 20, 30, 40, 50]
    });

    dataTable5 = $('#myTable5').DataTable({
        order: [[0, 'desc']],
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
                },
                display: $.fn.dataTable.Responsive.display.childRowImmediate,
                type: 'none',
                target: ''
            }

        },

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },


        "dom": '<l<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/SubmissionOnBiddings/GetNotSoldRealEstates"
        },
        "columns": [
            {
                "data": {
                    "id": "submissionOnBidding.submissionOnBiddingId"

                },
                "render": function (data) {
                    return '<h5>#' + data.submissionOnBidding.submissionOnBiddingId + '</h5>'
                }
            },
            {
                "data": {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "realEstateName": "realEstate.realEstateName",
                    "NumberOfAssets": "submissionOnBidding.amountOnBidding",
                    "confirmed": "submissionOnBidding.confirmed",
                    "submissionOnBiddingId": "submissionOnBidding.submissionOnBiddingId",
                },

                "render": function (data) {

                    if (data.submissionOnBidding.confirmed && data.submissionOnBidding.daysLeft < 0)
                        return ' <h6 class="task_title_0">' + data.realEstate.realEstateName + ' <a style=" color: black"  ><i class="fa fa-times fa-lg"></i></a></h6 ><p class="project_name_0 badge badge-danger" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'

                    
                }

            },


            { "data": "submissionOnBidding.biddingExposureDateStr", "width": "15%" },
            { "data": "submissionOnBidding.tradingPlatformName", "width": "15%" },

            {
                "data":
                {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "holdDate": "submissionOnBidding.biddingHoldDateStr"
                },

                "render": function (data) {
                    if (data.submissionOnBidding.daysLeft > 0)
                        return '<h3 style="color:#6362e7">' + data.submissionOnBidding.daysLeft + '<span style="font-size:14px"> кун қолди</span></h3 ><p class="task_desc_0" style="color:#7dc006"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft == 0)
                        return '<h3 style="color:green">Аукцион куни</h3 ><p class="task_desc_0" style="color:blue"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft < 0)
                        return '<h4 style="color:#black">Аукцион тугади</h4 ><p class="task_desc_0" style="color:#fc5a03"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                }
            },

            {
                "data": {
                    "activeValue": "submissionOnBidding.activeValue"
                },
                "render": function (data) {
                    return '<h5><mark>' + data.submissionOnBidding.activeValue + ' сўм</mark ></h5 > '
                }
            },

            {
                "data": {
                    "url": "submissionOnBidding.url",
                    "submissionOnBiddingId": "submissionOnBidding.submissionOnBiddingId",
                    "confirmed": "submissionOnBidding.confirmed"
                },
                "render": function (data) {

                    if (data.submissionOnBidding.confirmed)
                        return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                    else return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                        + '&nbsp&nbsp<span data-bs-toggle="modal" data-bs-target="#exampleModal3"><a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўзгартириш" style="cursor:pointer" onclic=Edit(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.realEstate.realEstateId + ',' + data.target + ')><i style="color:#eb8c21; " class="fa fa-edit fa-lg"></i></a></span>'
                        + '&nbsp&nbsp<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" style="cursor:pointer" onclick=Delete("/SimpleUser/SubmissionOnBiddings/Delete/' + data.submissionOnBidding.submissionOnBiddingId + '")><i  style="color:#fc5a03" class="fa fa-trash-o fa-lg"></i></a>'

                }
            },



        ],




        "language": {
            "lengthMenu": "",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "",
        },
        "lengthMenu": [4, 20, 30, 40, 50]
    });

    dataTable6 = $('#myTable6').DataTable({
        order: [[0, 'desc']],
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
                },
                display: $.fn.dataTable.Responsive.display.childRowImmediate,
                type: 'none',
                target: ''
            }

        },

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },


        "dom": '<l<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/SubmissionOnBiddings/GetNotSoldShares"
        },
        "columns": [
            {
                "data": {
                    "id": "submissionOnBidding.submissionOnBiddingId"

                },
                "render": function (data) {
                    return '<h5>#' + data.submissionOnBidding.submissionOnBiddingId + '</h5>'
                }
            },
            {
                "data": {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "realEstateName": "share.businessEntityName",
                    "NumberOfAssets": "submissionOnBidding.amountOnBidding",
                    "confirmed": "submissionOnBidding.confirmed",
                    "submissionOnBiddingId": "submissionOnBidding.submissionOnBiddingId",
                },

                "render": function (data) {

                    if (data.submissionOnBidding.confirmed && data.submissionOnBidding.daysLeft < 0)
                        return ' <h6 class="task_title_0">' + data.share.businessEntityName + ' <a style=" color: black"  ><i class="fa fa-times fa-lg"></i></a></h6 ><p class="project_name_0 badge badge-danger" style="color:white" >' + data.submissionOnBidding.amountOnBidding + ' та актив</p >'

                    
                }

            },


            { "data": "submissionOnBidding.biddingExposureDateStr", "width": "15%" },
            { "data": "submissionOnBidding.tradingPlatformName", "width": "15%" },

            {
                "data":
                {
                    "daysLeft": "submissionOnBidding.daysLeft",
                    "holdDate": "submissionOnBidding.biddingHoldDateStr"
                },

                "render": function (data) {
                    if (data.submissionOnBidding.daysLeft > 0)
                        return '<h3 style="color:#6362e7">' + data.submissionOnBidding.daysLeft + '<span style="font-size:14px"> кун қолди</span></h3 ><p class="task_desc_0" style="color:#7dc006"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft == 0)
                        return '<h3 style="color:green">Аукцион куни</h3 ><p class="task_desc_0" style="color:blue"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                    else if (data.submissionOnBidding.daysLeft < 0)
                        return '<h4 style="color:#black">Аукцион тугади</h4 ><p class="task_desc_0" style="color:#fc5a03"> ' + data.submissionOnBidding.biddingHoldDateStr + '</p>'
                }
            },

            {
                "data": {
                    "activeValue": "submissionOnBidding.activeValue"
                },
                "render": function (data) {
                    return '<h5><mark>' + data.submissionOnBidding.activeValue + ' сўм</mark ></h5 > '
                }
            },

            {
                "data": {
                    "url": "submissionOnBidding.url",
                    "submissionOnBiddingId": "submissionOnBidding.submissionOnBiddingId",
                    "confirmed": "submissionOnBidding.confirmed"
                },
                "render": function (data) {

                    if (data.submissionOnBidding.confirmed)
                        return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                    else return '<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Аукцион э-манзили" style="cursor:pointer" class="me-2" href="' + data.submissionOnBidding.url + '"><i class="fa fa-link fa-lg" ></i></a>'

                        + '&nbsp&nbsp<span data-bs-toggle="modal" data-bs-target="#exampleModal3"><a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўзгартириш" style="cursor:pointer" onclic=Edit(' + data.submissionOnBidding.submissionOnBiddingId + ',' + data.share.shareId + ',' + data.target + ')><i style="color:#eb8c21; " class="fa fa-edit fa-lg"></i></a></span>'
                        + '&nbsp&nbsp<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" style="cursor:pointer" onclick=Delete("/SimpleUser/SubmissionOnBiddings/Delete/' + data.submissionOnBidding.submissionOnBiddingId + '")><i  style="color:#fc5a03" class="fa fa-trash-o fa-lg"></i></a>'

                }
            },



        ],




        "language": {
            "lengthMenu": "",
            "zeroRecords": "Ҳеч нима топилмади - узур",
            "info": "",
            "infoEmpty": "",
            "infoFiltered": "( Jami _MAX_ та маълумотдан филтер қилинди)",
            "oPaginate": {
                "sFirst": "Биринчи",
                "sLast": "Охирги",
                "sNext": "Кейинги",
                "sPrevious": "Аввалги"
            },
            "sSearch": "",
        },
        "lengthMenu": [4, 20, 30, 40, 50]
    });
}


function Edit(id, id1, target) {

    window.location.href = "/SimpleUser/SubmissionOnBiddings/Edit/?id=" + id + "&id1=" + id1 + "&target=" + target;

}

function Delete(id,target) {
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
            var data = {
                id: id,
                target:target
            }
            $.ajax({
                type: "DELETE",
                url: "/SimpleUser/SubmissionOnBiddings/Delete/",
                data: JSON.stringify(data),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {

                        Swal.fire({
                            title: "Ўчирилди!",
                            text: data.message,
                            icon: "success"
                        });
                        /* toastr.success(data.message);*/
                        dataTable1.ajax.reload();
                        dataTable2.ajax.reload();
                        dataTable3.ajax.reload();
                        dataTable4.ajax.reload();
                        dataTable5.ajax.reload();
                        dataTable6.ajax.reload();
                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                        
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
        title: "Аукцион маълумотларини тасдиқлайсизми?",
        text: "Маълумотларни тасдиқлагач, уларни ўзгартира олмайсиз!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',

    }).then(function (result) {
        if (result.isConfirmed) {
            var data = {
                id: id,
                target: target
            };
            $.ajax({
                type: "POST",
                url: "/SimpleUser/SubmissionOnBiddings/Send/",
                data: JSON.stringify(data),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: "Тасдиқланди!",
                            text: data.message,
                            icon: "success"

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



function SendResult(id, target) {


    const inputOptions = new Promise((resolve) => {
        setTimeout(() => {
            resolve({
                'True': 'Сотилди',
                'False': 'Сотилмади',

            })
        }, 1000)
    })

    Swal.fire({
        title: 'Аукцион натижасини белгиланг!',
        icon: "warning",
        input: 'radio',
        inputOptions: inputOptions,
        inputValidator: (value) => {
            if (!value) {
                return 'Натижани Белгиланг!'
            }
        },
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',

    }).then( function(result)  {
        if (result.isConfirmed) {
            var data = {
                id : id,
                result: result.value,
                target: target
            }
            $.ajax({
                type: "POST",
                url: "/SimpleUser/SubmissionOnBiddings/SendResult",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data),
                
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: "Тасдиқланди!",
                            text: data.message,
                            icon: "success"

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
                },
                failure: function (data) {
                    Swal.fire(
                        "Internal Error",
                        "Афсуски амалиётни бажариш имлони бўлмади!", // had a missing comma
                        "error"
                    )
                }
            });
        }
    })

}
    

