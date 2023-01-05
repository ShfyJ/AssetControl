var dataTable1;
var dataTable2;
var dataTable3;
var dataTable4;
var dataTable5;
var dataTable6;
var dataTable7;
var dataTable8;

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

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },

        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnSale",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = {target:0, type:1};
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:red; font-weight:bold">тасдиқланмаган</span>'
                }
            },

            {
                "data": { "status": "oneTimePaymentAsset.status" },
                "render": function (data) {
                    return '<h6><span class="badge badge-warning" style="color:black">Сотувда</span></h6>'
                }

            },

            { "data": "oneTimePaymentAsset.biddingDateStr", "width": "15%"},
            {
                "data": { "solutionFileId": "oneTimePaymentAsset.solutionFileId" },
                "render": function (data) {
                    return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentAsset.solutionFileId + '"><i class="fa fa-download">Ҳужжатни юклаб олиш</i></a>'
                                    
                }

            },
            

            {
                "data": {
                    "name":"name",
                    "oneTimePaymentAssetId": "oneTimePaymentAsset.oneTimePaymentAssetId",
                    "status": "status"
                },
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDate,
                        solutionFileLink: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDate
                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=Edit('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep1('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete(${data.oneTimePaymentAsset.oneTimePaymentAssetId},1)><i style="color:red;" class="fa fa-trash-o"></i></button>`
                    
                }
            },

            {
                "data": {

                    "oneTimePaymentAssetId": "oneTimePaymentAsset.oneTimePaymentAssetId",
                    "status": "status"
                },
                "render": function (data) {

                    return '<button class="loader-box" style="height:35px; width:130px; color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm(' + data.oneTimePaymentAsset.oneTimePaymentAssetId +','+1+') >Тасдиқлаш<p class="loader-9" style="height:30px;width:30px; background-color:red;"></p></button>'
                }
            },


        ],


        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [10, 20, 30, 40, 50]

    });
    dataTable2 = $('#myTable_').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnSale",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 1, type: 1 };
                return JSON.stringify(data);
            }
                
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:green; font-weight:bold">тасдиқланган</span>'
                }
            },
            {
                "data": { "status": "oneTimePaymentAsset.status" },
                "render": function (data) {
                    return '<h6><span class="badge badge-success" style="color:black">Сотувда</span></h6>'
                }

            },

            { "data": "oneTimePaymentAsset.biddingDateStr", "width": "15%" },
            {
                "data": { "solutionFileId": "oneTimePaymentAsset.solutionFileId" },
                "render": function (data) {
                    return '<div>' +
                        '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentAsset.solutionFileId + '"><i class="fa fa-download">Ҳужжатни юклаб олиш</i></a>' +
                        '</div >'
                }

            },
            

            {
                "data": {
                    
                },
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDate,
                        solutionFileLink: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDate
                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));

                    if (data.status)
                        return `<button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep1('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                                <span data-bs-toggle="modal" data-bs-target="#contractModal"><button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Шартнома киритиш" style="cursor:pointer" onclick=AssignPaymentId('${data.oneTimePaymentAsset.oneTimePaymentAssetId}') ><i class="fa fa-pencil" style="color:blue;"> Шартнома киритиш</i></button></span>
                               <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Сотувни бекор қилиш" style="cursor:pointer" onclick=Cancel(${data.oneTimePaymentAsset.oneTimePaymentAssetId}`+`,`+1+`)><i class="fa fa-times-circle-o" style="color:red;"></i></button>`
                            
                    else return `<button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep1('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                                 <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Шартнома киритиш" style="cursor:pointer; color:grey" disabled><i class="fa fa-ban" > Шартнома киритиш</i></button>`

                }
            },

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [10, 20, 30, 40, 50]

    });

    dataTable3 = $('#myTable_1').DataTable({
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
                        $('<table/>').append(data): 
                        false;
                }
            }

        },
        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnContract",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 0, type: 1 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:red; font-weight:bold">тасдиқланмаган</span>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<span class="badge badge-secondary" style="color:black">' + data.oneTimePaymentAsset.status + '</span>'
                },
            },

            { "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountOfAssetSold + ' <a ><span class="badge rounded-pill badge-dark">cўм</span></a></p>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountPayed + ' <span class="badge rounded-pill badge-dark">cўм</span> | <span>' + data.oneTimePaymentStep2.percentage + ' %</span></p>'
                }
            },
                        

            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        status: data.oneTimePaymentAsset.status,
                        step2Id: data.oneTimePaymentStep2.oneTimePaymentStep2Id,
                        assetBuyerName: data.oneTimePaymentStep2.assetBuyerName,
                        amountOfAssetSold: data.oneTimePaymentStep2.amountOfAssetSold,
                        amountPayed: data.oneTimePaymentStep2.amountPayed,
                        percentage: data.oneTimePaymentStep2.percentage,
                        agreementDate: data.oneTimePaymentStep2.agreementDateStr,
                        agreementNumber: data.oneTimePaymentStep2.aggreementNumber,
                        agreementFile: data.oneTimePaymentStep2.aggreementFileLink,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr

                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=EditContract('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep2('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete("/SimpleUser/OneTimePaymentAssets/Delete/?id=${data.oneTimePaymentStep2.oneTimePaymentStep2Id}&target=2")><i style="color:red;" class="fa fa-trash-o"></i></button>`
                }
            },

            {
                "data": {},
                "render": function (data) {

                    return '<button class="loader-box" style="height:35px; width:130px; color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm(' + data.oneTimePaymentStep2.oneTimePaymentStep2Id + ','+2+') >Тасдиқлаш<p class="loader-9" style="height:30px;width:30px; background-color:red;"></p></button>'
                }
            },


        ],


        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [10, 20, 30, 40, 50]
    });
    dataTable4 = $('#myTable_2').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnContract",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 1, type: 1 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:green; font-weight:bold">тасдиқланган</span>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<span class="badge badge-success" style="color:black">' + data.oneTimePaymentAsset.status + '</span>'
                },
            },

            { "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountOfAssetSold + ' <a ><span class="badge rounded-pill badge-dark">cўм</span></a></p>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountPayed + ' <span class="badge rounded-pill badge-dark">cўм</span> | <span>' + data.oneTimePaymentStep2.percentage + ' %</span></p>'
                }
            },


            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        status: data.oneTimePaymentAsset.status,
                        step2Id: data.oneTimePaymentStep2.oneTimePaymentStep2Id,
                        assetBuyerName: data.oneTimePaymentStep2.assetBuyerName,
                        amountOfAssetSold: data.oneTimePaymentStep2.amountOfAssetSold,
                        amountPayed: data.oneTimePaymentStep2.amountPayed,
                        percentage: data.oneTimePaymentStep2.percentage,
                        agreementDate: data.oneTimePaymentStep2.agreementDateStr,
                        agreementNumber: data.oneTimePaymentStep2.aggreementNumber,
                        agreementFile: data.oneTimePaymentStep2.aggreementFileLink,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr

                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `
                        <button id="inDetail3" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep2('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <span data-bs-toggle="modal" data-bs-target="#actModal"><button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Акт тузиш" style="cursor:pointer" onclick=AssignPaymentId('${data.oneTimePaymentAsset.oneTimePaymentAssetId}') ><i class="fa fa-pencil" style="color:blue;"> Акт тузиш</i></button></span>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Шартномани бекор қилиш" style="cursor:pointer" onclick=Cancel(${data.oneTimePaymentStep2.oneTimePaymentStep2Id}` + `,` + 2 + `)><i class="fa fa-times-circle-o" style="color:red;"></i></button>`

                    }
            },

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [10, 20, 30, 40, 50]
    });

    dataTable5 = $('#myTable1_').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnAct",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 0, type: 1 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:red; font-weight:bold">тасдиқланмаган</span>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<span class="badge badge-secondary" style="color:black">' + data.oneTimePaymentAsset.status + '</span>'
                },
            },

            { "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountOfAssetSold + ' <a ><span class="badge rounded-pill badge-dark">cўм</span></a></p>'
                }
            },

            {
                "data": {},
                "render": function (data) {
                    return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.actAndAssetFileId + '"><i class="fa fa-download">Акт ҳужжатини юклаб олиш</i></a><br/>' +
                        '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.invoiceFileId + '"><i class="fa fa-download">Инвоисни юклаб олиш</i></a>'

                }

            },

            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        status: data.oneTimePaymentAsset.status,
                        step3Id: data.oneTimePaymentStep3.oneTimePaymentStep3Id,
                        assetBuyerName: data.oneTimePaymentStep2.assetBuyerName,
                        amountOfAssetSold: data.oneTimePaymentStep2.amountOfAssetSold,
                        
                        agreementDate: data.oneTimePaymentStep2.agreementDateStr,
                        agreementNumber: data.oneTimePaymentStep2.aggreementNumber,
                        agreementFile: data.oneTimePaymentStep2.aggreementFileLink,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr,
                        actDate: data.oneTimePaymentStep3.actAndAssetDateStr,
                        actNumber: data.oneTimePaymentStep3.actAndAssetNumber,
                        actFile: data.oneTimePaymentStep3.actAndAssetFileLink,
                        invoiceFile: data.oneTimePaymentStep3.invoiceFileLink,

                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=EditAct('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep3('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete("/SimpleUser/OneTimePaymentAssets/Delete/?id=${data.oneTimePaymentStep3.oneTimePaymentStep3Id}&target=3")><i style="color:red;" class="fa fa-trash-o"></i></button>`
                }
            },

            {
                "data": {},
                "render": function (data) {

                    return '<button class="loader-box" style="height:35px; width:130px; color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm(' + data.oneTimePaymentStep3.oneTimePaymentStep3Id + ',' + 3 + ') >Тасдиқлаш<p class="loader-9" style="height:30px;width:30px; background-color:red;"></p></button>'
                }
            },


        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [20, 30, 40, 50]
    });
    dataTable6 = $('#myTable2_').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnAct",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 1, type: 1 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:green; font-weight:bold">тасдиқланган</span>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<span class="badge badge-success" style="color:black;">' + data.oneTimePaymentAsset.status + '</span>'
                },
            },

            { "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountOfAssetSold + ' <a ><span class="badge rounded-pill badge-dark">cўм</span></a></p>'
                }
            },

            {
                "data": {},
                "render": function (data) {
                    return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.actAndAssetFileId + '"><i class="fa fa-download">Акт ҳужжатини юклаб олиш</i></a><br/>' +
                        '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.invoiceFileId + '"><i class="fa fa-download">Инвоисни юклаб олиш</i></a>'

                }

            },

            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        status: data.oneTimePaymentAsset.status,
                        step3Id: data.oneTimePaymentStep3.oneTimePaymentStep3Id,
                        assetBuyerName: data.oneTimePaymentStep2.assetBuyerName,
                        amountOfAssetSold: data.oneTimePaymentStep2.amountOfAssetSold,

                        agreementDate: data.oneTimePaymentStep2.agreementDateStr,
                        agreementNumber: data.oneTimePaymentStep2.aggreementNumber,
                        agreementFile: data.oneTimePaymentStep2.aggreementFileLink,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr,
                        actDate: data.oneTimePaymentStep3.actAndAssetDateStr,
                        actNumber: data.oneTimePaymentStep3.actAndAssetNumber,
                        actFile: data.oneTimePaymentStep3.actAndAssetFileLink,
                        invoiceFile: data.oneTimePaymentStep3.invoiceFileLink,

                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `
                        <button id="inDetail3" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep3('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        `
                }
            },

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [20, 30, 40, 50]
    });

    dataTable7 = $('#_myTable1').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetNotSoldAssets",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (e) {
                e = 1;
                return JSON.stringify(e);
            }
        },
        "columns": [

            { "data": "name", "width": "15%" },
  

            {
                "data": {},
                "render": function (data) {
                    return '<h6><span class="badge badge-danger" style="color:black">' + data.oneTimePaymentAsset.status + '</span></h6>'
                }

            },

            { "data": "oneTimePaymentAsset.governingBodyName", "width": "15%" },
            { "data": "oneTimePaymentAsset.solutionNumber", "width": "15%" },
            { "data": "oneTimePaymentAsset.solutionDateStr", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentAsset.solutionFileId + '"><i class="fa fa-download"> Қарор ҳужжати</i></a><br/>'

                }

            },
            { "data": "oneTimePaymentAsset.biddingDateStr", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.amountOfAssetSold", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.aggreementDateStr", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.aggreementNumber", "width": "15%" },
            //{
            //    "data": {},
            //    "render": function (data) {
            //        return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep2.aggreementFileId + '"><i class="fa fa-download"> Шартнома ҳужжати</i></a><br/>'

            //    }

            //},
            //{ "data": "oneTimePaymentStep2.amountPayed", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.percentage ", "width": "15%" },
            //{ "data": "oneTimePaymentStep3.actAndAssetDateStr", "width": "15%" },
            //{ "data": "oneTimePaymentStep3.actAndAssetNumber", "width": "15%" },
            //{
            //    "data": {},
            //    "render": function (data) {
            //        return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.actAndAssetFileId + '"><i class="fa fa-download"> Акт ҳужжати</i></a><br/>'+
            //            '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.invoiceFileId + '"><i class="fa fa-download"> Инвоис ҳужжати</i></a><br/>'
            //    }

            //},
           

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [20, 30, 40, 50]
    });

    dataTable8 = $('#myTable2').DataTable({
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

        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnSale",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 0, type: 2 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:red; font-weight:bold">тасдиқланмаган</span>'
                }
            },

            {
                "data": { "status": "oneTimePaymentAsset.status" },
                "render": function (data) {
                    return '<h6><span class="badge badge-warning" style="color:black">Сотувда</span></h6>'
                }

            },

            { "data": "oneTimePaymentAsset.biddingDateStr", "width": "15%" },
            {
                "data": { "solutionFileId": "oneTimePaymentAsset.solutionFileId" },
                "render": function (data) {
                    return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentAsset.solutionFileId + '"><i class="fa fa-download">Ҳужжатни юклаб олиш</i></a>'

                }

            },


            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        step1Id: data.oneTimePaymentAsset.oneTimePaymentAssetId,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr

                    };
                    assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=Edit('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep1('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete(${data.oneTimePaymentAsset.oneTimePaymentAssetId},1)><i style="color:red;" class="fa fa-trash-o"></i></button>`
                }
            },

            {
                "data": {

                    "oneTimePaymentAssetId": "oneTimePaymentAsset.oneTimePaymentAssetId",
                    "status": "status"
                },
                "render": function (data) {

                    return '<button class="loader-box" style="height:35px; width:130px; color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm(' + data.oneTimePaymentAsset.oneTimePaymentAssetId + ',' + 1 + ') >Тасдиқлаш<p class="loader-9" style="height:30px;width:30px; background-color:red;"></p></button>'
                }
            },


        ],


        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [10, 20, 30, 40, 50]

    });
    dataTable9 = $('#my_Table2').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnSale",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 1, type: 2 };
                return JSON.stringify(data);
            }

        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:green; font-weight:bold">тасдиқланган</span>'
                }
            },
            {
                "data": { "status": "oneTimePaymentAsset.status" },
                "render": function (data) {
                    return '<h6><span class="badge badge-success" style="color:black">Сотувда</span></h6>'
                }

            },

            { "data": "oneTimePaymentAsset.biddingDate", "width": "15%" },
            {
                "data": { "solutionFileId": "oneTimePaymentAsset.solutionFileId" },
                "render": function (data) {
                    return '<div>' +
                        '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentAsset.solutionFileId + '"><i class="fa fa-download">Ҳужжатни юклаб олиш</i></a>' +
                        '</div >'
                }

            },


            {
                "data": {

                },
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDate,
                        solutionFileLink: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDate
                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));

                    if (data.status)
                        return `<button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep1('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                                <span data-bs-toggle="modal" data-bs-target="#contractModal"><button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Шартнома киритиш" style="cursor:pointer" onclick=AssignPaymentId('${data.oneTimePaymentAsset.oneTimePaymentAssetId}') ><i class="fa fa-pencil" style="color:blue;"> Шартнома киритиш</i></button></span>
                               <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Сотувни бекор қилиш" style="cursor:pointer" onclick=Cancel(${data.oneTimePaymentAsset.oneTimePaymentAssetId}` + `,` + 1 + `)><i class="fa fa-times-circle-o" style="color:red;"></i></button>`

                    else return `<button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep1('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                                 <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Шартнома киритиш" style="cursor:pointer; color:grey" disabled><i class="fa fa-ban" > Шартнома киритиш</i></button>`

                }
            },

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [10, 20, 30, 40, 50]

    });

    dataTable10 = $('#myTable3').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnContract",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 0, type: 2 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:red; font-weight:bold">тасдиқланмаган</span>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<span class="badge badge-secondary" style="color:black">' + data.oneTimePaymentAsset.status + '</span>'
                },
            },

            { "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountOfAssetSold + ' <a ><span class="badge rounded-pill badge-dark">cўм</span></a></p>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountPayed + ' <span class="badge rounded-pill badge-dark">cўм</span> | <span>' + data.oneTimePaymentStep2.percentage + ' %</span></p>'
                }
            },


            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        status: data.oneTimePaymentAsset.status,
                        step2Id: data.oneTimePaymentStep2.oneTimePaymentStep2Id,
                        assetBuyerName: data.oneTimePaymentStep2.assetBuyerName,
                        amountOfAssetSold: data.oneTimePaymentStep2.amountOfAssetSold,
                        amountPayed: data.oneTimePaymentStep2.amountPayed,
                        percentage: data.oneTimePaymentStep2.percentage,
                        agreementDate: data.oneTimePaymentStep2.agreementDateStr,
                        agreementNumber: data.oneTimePaymentStep2.aggreementNumber,
                        agreementFile: data.oneTimePaymentStep2.aggreementFileLink,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr

                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=EditContract('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep2('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete(${data.oneTimePaymentStep2.oneTimePaymentStep2Id},2)><i style="color:red;" class="fa fa-trash-o"></i></button>`
                }
            },

            {
                "data": {},
                "render": function (data) {

                    return '<button class="loader-box" style="height:35px; width:130px; color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm(' + data.oneTimePaymentStep2.oneTimePaymentStep2Id + ',' + 2 + ') >Тасдиқлаш<p class="loader-9" style="height:30px;width:30px; background-color:red;"></p></button>'
                }
            },


        ],


        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [10, 20, 30, 40, 50]
    });
    dataTable11 = $('#my_Table3').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnContract",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 1, type: 2 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:green; font-weight:bold">тасдиқланган</span>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<span class="badge badge-success" style="color:black">' + data.oneTimePaymentAsset.status + '</span>'
                },
            },

            { "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountOfAssetSold + ' <a ><span class="badge rounded-pill badge-dark">cўм</span></a></p>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountPayed + ' <span class="badge rounded-pill badge-dark">cўм</span> | <span>' + data.oneTimePaymentStep2.percentage + ' %</span></p>'
                }
            },


            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        status: data.oneTimePaymentAsset.status,
                        step2Id: data.oneTimePaymentStep2.oneTimePaymentStep2Id,
                        assetBuyerName: data.oneTimePaymentStep2.assetBuyerName,
                        amountOfAssetSold: data.oneTimePaymentStep2.amountOfAssetSold,
                        amountPayed: data.oneTimePaymentStep2.amountPayed,
                        percentage: data.oneTimePaymentStep2.percentage,
                        agreementDate: data.oneTimePaymentStep2.agreementDateStr,
                        agreementNumber: data.oneTimePaymentStep2.aggreementNumber,
                        agreementFile: data.oneTimePaymentStep2.aggreementFileLink,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr

                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `
                        <button id="inDetail3" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep2('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <span data-bs-toggle="modal" data-bs-target="#actModal"><button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Акт тузиш" style="cursor:pointer" onclick=AssignPaymentId('${data.oneTimePaymentAsset.oneTimePaymentAssetId}') ><i class="fa fa-pencil" style="color:blue;"> Акт тузиш</i></button></span>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Шартномани бекор қилиш" style="cursor:pointer" onclick=Cancel(${data.oneTimePaymentStep2.oneTimePaymentStep2Id}` + `,` + 2 + `)><i class="fa fa-times-circle-o" style="color:red;"></i></button>`

                }
            },

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [10, 20, 30, 40, 50]
    });

    dataTable12 = $('#myTable4').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnAct",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 0, type: 2 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:red; font-weight:bold">тасдиқланмаган</span>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<span class="badge badge-secondary" style="color:black">' + data.oneTimePaymentAsset.status + '</span>'
                },
            },

            { "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountOfAssetSold + ' <a ><span class="badge rounded-pill badge-dark">cўм</span></a></p>'
                }
            },

            {
                "data": {},
                "render": function (data) {
                    return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.actAndAssetFileId + '"><i class="fa fa-download">Акт ҳужжатини юклаб олиш</i></a><br/>' +
                        '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.invoiceFileId + '"><i class="fa fa-download">Инвоисни юклаб олиш</i></a>'

                }

            },

            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        status: data.oneTimePaymentAsset.status,
                        step3Id: data.oneTimePaymentStep3.oneTimePaymentStep3Id,
                        assetBuyerName: data.oneTimePaymentStep2.assetBuyerName,
                        amountOfAssetSold: data.oneTimePaymentStep2.amountOfAssetSold,

                        agreementDate: data.oneTimePaymentStep2.agreementDateStr,
                        agreementNumber: data.oneTimePaymentStep2.aggreementNumber,
                        agreementFile: data.oneTimePaymentStep2.aggreementFileLink,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr,
                        actDate: data.oneTimePaymentStep3.actAndAssetDateStr,
                        actNumber: data.oneTimePaymentStep3.actAndAssetNumber,
                        actFile: data.oneTimePaymentStep3.actAndAssetFileLink,
                        invoiceFile: data.oneTimePaymentStep3.invoiceFileLink,

                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=EditAct('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button  data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep3('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete(${data.oneTimePaymentStep3.oneTimePaymentStep3Id},3)><i style="color:red;" class="fa fa-trash-o"></i></button>`
                }
            },

            {
                "data": {},
                "render": function (data) {

                    return '<button class="loader-box" style="height:35px; width:130px; color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm(' + data.oneTimePaymentStep3.oneTimePaymentStep3Id + ',' + 3 + ') >Тасдиқлаш<p class="loader-9" style="height:30px;width:30px; background-color:red;"></p></button>'
                }
            },


        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [20, 30, 40, 50]
    });
    dataTable13 = $('#my_Table4').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetAssetsOnAct",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = { target: 1, type: 2 };
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {
                    "name": "name"
                },
                "render": function (data) {
                    return '<h6>' + data.name + '</h6><span style="color:green; font-weight:bold">тасдиқланган</span>'
                }
            },
            {
                "data": {},
                "render": function (data) {
                    return '<span class="badge badge-success" style="color:black;">' + data.oneTimePaymentAsset.status + '</span>'
                },
            },

            { "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<p>' + data.oneTimePaymentStep2.amountOfAssetSold + ' <a ><span class="badge rounded-pill badge-dark">cўм</span></a></p>'
                }
            },

            {
                "data": {},
                "render": function (data) {
                    return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.actAndAssetFileId + '"><i class="fa fa-download">Акт ҳужжатини юклаб олиш</i></a><br/>' +
                        '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.invoiceFileId + '"><i class="fa fa-download">Инвоисни юклаб олиш</i></a>'

                }

            },

            {
                "data": {},
                "render": function (data) {
                    var asset = {
                        name: data.name,
                        status: data.oneTimePaymentAsset.status,
                        step3Id: data.oneTimePaymentStep3.oneTimePaymentStep3Id,
                        assetBuyerName: data.oneTimePaymentStep2.assetBuyerName,
                        amountOfAssetSold: data.oneTimePaymentStep2.amountOfAssetSold,

                        agreementDate: data.oneTimePaymentStep2.agreementDateStr,
                        agreementNumber: data.oneTimePaymentStep2.aggreementNumber,
                        agreementFile: data.oneTimePaymentStep2.aggreementFileLink,
                        orgName: data.oneTimePaymentAsset.governingBodyName,
                        solutionNumber: data.oneTimePaymentAsset.solutionNumber,
                        solutionDate: data.oneTimePaymentAsset.solutionDateStr,
                        solutionFile: data.oneTimePaymentAsset.solutionFileLink,
                        biddingDate: data.oneTimePaymentAsset.biddingDateStr,
                        actDate: data.oneTimePaymentStep3.actAndAssetDateStr,
                        actNumber: data.oneTimePaymentStep3.actAndAssetNumber,
                        actFile: data.oneTimePaymentStep3.actAndAssetFileLink,
                        invoiceFile: data.oneTimePaymentStep3.invoiceFileLink,

                    };
                    var assetStr = encodeURIComponent(JSON.stringify(asset));
                    return `
                        <button id="inDetail3" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailStep3('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        `
                }
            },

        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [20, 30, 40, 50]
    });

    dataTable14 = $('#myTable5').DataTable({
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
        dom: 'lfrtip',
        scrollX: false,
        "dom": '<lf<rt>ip>',

        "ajax": {
            "url": "/SimpleUser/OneTimePaymentAssets/GetNotSoldAssets",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (e) {
                e = 2;
                return JSON.stringify(e);
            }
        },
        "columns": [

            { "data": "name", "width": "15%" },


            {
                "data": {},
                "render": function (data) {
                    return '<h6><span class="badge badge-danger" style="color:black">' + data.oneTimePaymentAsset.status + '</span></h6>'
                }

            },

            { "data": "oneTimePaymentAsset.governingBodyName", "width": "15%" },
            { "data": "oneTimePaymentAsset.solutionNumber", "width": "15%" },
            { "data": "oneTimePaymentAsset.solutionDateStr", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentAsset.solutionFileId + '"><i class="fa fa-download"> Қарор ҳужжати</i></a><br/>'

                }

            },
            { "data": "oneTimePaymentAsset.biddingDateStr", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.assetBuyerName", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.amountOfAssetSold", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.aggreementDateStr", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.aggreementNumber", "width": "15%" },
            //{
            //    "data": {},
            //    "render": function (data) {
            //        return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep2.aggreementFileId + '"><i class="fa fa-download"> Шартнома ҳужжати</i></a><br/>'

            //    }

            //},
            //{ "data": "oneTimePaymentStep2.amountPayed", "width": "15%" },
            //{ "data": "oneTimePaymentStep2.percentage", "width": "15%" },
            //{ "data": "oneTimePaymentStep3.actAndAssetDateStr", "width": "15%" },
            //{ "data": "oneTimePaymentStep3.actAndAssetNumber", "width": "15%" },
            //{
            //    "data": {},
            //    "render": function (data) {
            //        return '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.actAndAssetFileId + '"><i class="fa fa-download"> Акт ҳужжати</i></a><br/>' +
            //            '<a  href="/SimpleUser/OneTimePaymentAssets/DownloadFile/' + data.oneTimePaymentStep3.invoiceFileId + '"><i class="fa fa-download"> Инвоис ҳужжати</i></a><br/>'
            //    }

            //},


        ],

        "language": {
            "lengthMenu": "Кўрсатинг _MENU_ ",
            "zeroRecords": "Mаълумот топилмади",
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
        "lengthMenu": [20, 30, 40, 50]
    });

}

function Edit(data) {

    data = JSON.parse(decodeURIComponent(data));
    console.log(data);
    $('#_saleId').val(data.step1Id);

    $('#_orgNameEdit').val(data.orgName);

    $('#solutionNEdit').val(data.solutionNumber);

    $('#solutionDEdit').val(data.solutionDate);

    $('#fileSaleId').val(data.step1Id);

    var solFileLink = document.getElementById('solFileLink');
    solFileLink.href = data.solutionFile;

    $('#editSaleModal').modal('show');

}

function EditContract(data) {
    
    data = JSON.parse(decodeURIComponent(data));
    console.log(data);
    $('#_contractId').val(data.step2Id);
    
    $('#_buyerName').val(data.assetBuyerName);

    $('#_amountSold').val(data.amountOfAssetSold);
   
    $('#_agreementNumber').val(data.agreementNumber);
    $('#_amountPayed').val(data.amountPayed);
    $('#_percentage').val(data.percentage);
    $('#fileContractId').val(data.step2Id);

    $('#_agreementDate').val(data.agreementDate);
   
    var agrFileLink = document.getElementById('agrFileLink');
    agrFileLink.href = data.agreementFile;

    $('#editContractModal').modal('show');

}

function EditAct(data) {

    data = JSON.parse(decodeURIComponent(data));
    console.log(data);
    $('#_actId').val(data.step3Id);

    $('#_actDate').val(data.actDate);
    $('#_actNumber').val(data.actNumber);
   
    $('#fileActId').val(data.step3Id);
    $('#fileInvId').val(data.step3Id);

    var actFileLink = document.getElementById('actFileLink');
    actFileLink.href = data.actFile;

    var invoiceFileLink = document.getElementById('invoiceFileLink');
    invoiceFileLink.href = data.invoiceFile;

    $('#editActModal').modal('show');

}

function DetailStep1(item) {

    item = JSON.parse(decodeURIComponent(item));
    var assetName = document.getElementById("assetName");
    var orgName = document.getElementById("orgName");
    var solutionNumber = document.getElementById("solutionNumber");
    var solutionDate = document.getElementById("solutionDate");
    var solutionFile = document.getElementById("solutionFile");
    var biddingDate = document.getElementById("biddingDate");

    assetName.innerHTML = item.name;
    orgName.innerHTML = item.orgName;
    solutionNumber.innerHTML = item.solutionNumber;
    solutionDate.innerHTML = item.solutionDate;
    solutionFile.href = item.solutionFile;
    solutionFile.innerHTML = "Қарор ҳужжати";
    biddingDate.innerHTML = item.biddingDate;

    $('#onSaleModal').modal('show');
    
}

function DetailStep2(data) {

    data = JSON.parse(decodeURIComponent(data));
    console.log(data);
    var assetBuyerName = document.getElementById("assetBuyerName");
    var amountOfAssetSold = document.getElementById("amountOfAssetSold");
    var agreementDate = document.getElementById("aggreementDate");
    var agreementNumber = document.getElementById("aggreementNumber");
    var agreementFile = document.getElementById("aggreementFile");
    var amountPayed = document.getElementById("amountPayed");
    var percentage = document.getElementById("percentage");

    var assetName = document.getElementById("assetName2");
    var orgName = document.getElementById("orgName2");
    var solutionNumber = document.getElementById("solutionNumber2");
    var solutionDate = document.getElementById("solutionDate2");
    var solutionFile = document.getElementById("solutionFile2");
    var biddingDate = document.getElementById("biddingDate2");

    assetBuyerName.innerHTML = data.assetBuyerName;
    amountOfAssetSold.innerHTML = data.amountOfAssetSold;
    agreementDate.innerHTML = data.agreementDate;
    agreementNumber.innerHTML = data.agreementNumber;
    agreementFile.href = data.agreementFile;
    agreementFile.innerHTML = "Шартнома ҳужжати";
    amountPayed.innerHTML = data.amountPayed;
    percentage.innerHTML = data.percentage;

    assetName.innerHTML = data.name;
    orgName.innerHTML = data.orgName;
    solutionNumber.innerHTML = data.solutionNumber;
    solutionDate.innerHTML = data.solutionDate;
    solutionFile.href = data.solutionFile;
    solutionFile.innerHTML = "Қарор ҳужжати";
    biddingDate.innerHTML = data.biddingDate;

    $('#onContractModal').modal('show');

}

function DetailStep3(data) {

    data = JSON.parse(decodeURIComponent(data));

    var actFile = document.getElementById("actFileId");
    var actDate = document.getElementById("actAndAssetDate");
    var actNumber = document.getElementById("actAndAssetNumber");
    var invFile = document.getElementById("invoiceFileId");

    var assetBuyerName = document.getElementById("assetBuyerName3");
    var amountOfAssetSold = document.getElementById("amountOfAssetSold3");
    var agreementDate = document.getElementById("aggreementDate3");
    var agreementNumber = document.getElementById("aggreementNumber3");
    var agreementFile = document.getElementById("aggreementFile3");


    var assetName = document.getElementById("assetName3");
    var orgName = document.getElementById("orgName3");
    var solutionNumber = document.getElementById("solutionNumber3");
    var solutionDate = document.getElementById("solutionDate3");
    var solutionFile = document.getElementById("solutionFile3");
    var biddingDate = document.getElementById("biddingDate3");

    actDate.innerHTML = data.actDate;
    actNumber.innerHTML = data.actNumber;
    actFile.href = data.actFile;
    actFile.innerHTML = "Акт ҳужжати";
    invFile.innerHTML = "Инвоис ҳужжати";
    invFile.href = data.invoiceFile;

    assetBuyerName.innerHTML = data.assetBuyerName;
    amountOfAssetSold.innerHTML = data.amountOfAssetSold;
    agreementDate.innerHTML = data.agreementDate;
    agreementNumber.innerHTML = data.agreementNumber;
    agreementFile.href = data.agreementFile;
    agreementFile.innerHTML = "Шартнома ҳужжати";

    assetName.innerHTML = data.name;
    orgName.innerHTML = data.orgName;
    solutionNumber.innerHTML = data.solutionNumber;
    solutionDate.innerHTML = data.solutionDate;
    solutionFile.href = data.solutionFile;
    solutionFile.innerHTML = "Қарор ҳужжати";
    biddingDate.innerHTML = data.biddingDate;

    $('#onActModal').modal('show');

}


var assetBuyerName;
var amountOfAssetSold;
var agreementDate;
var agreementNumber;
var agreementFile;
var amountPayed;

var v = document.getElementById("amountSold");
var v2 = document.getElementById("amountPayed_");
var v3 = document.getElementById("agreementDate");
var v4 = document.getElementById("agreementNumber");
var v5 = document.getElementById("buyerName");

var _v = document.getElementById("_amountSold");
var _v2 = document.getElementById("_amountPayed");
var _v3 = document.getElementById("_agreementDate");
var _v4 = document.getElementById("_agreementNumber");
var _v5 = document.getElementById("_buyerName");


var a1 = document.getElementById("actDate");
var a2 = document.getElementById("actNumber");

var _a1 = document.getElementById("_actDate");
var _a2 = document.getElementById("_actNumber");


function AssignPaymentId(id) {
    document.getElementById("paymentId").value = id;
    document.getElementById("paymentId_act").value = id;
}

$('#_amountSold, #_amountPayed').keyup(function () {

    if (_v.value != "" && _v.value > 0 && !isNaN(_v.value) && _v2.value != "" && _v2.value > 0 && !isNaN(_v2.value)) {
        $('#_percentage').val(((parseFloat(_v2.value) / parseFloat(_v.value)) * 100).toFixed(3));
    }
});

$('#sale-edit').on('click', function (e) {
    e.preventDefault();
    //var form = $(this).parents('form');
    var form = $('#demo-formSale')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    var s1 = document.getElementById("_orgNameEdit");
    var s2 = document.getElementById("solutionNEdit");
    var s3 = document.getElementById("solutionDEdit");

    if (s3.value == "" || (!/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(s3.value) && !/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(s3.value))) {

        s3.style.borderColor = "red";
        return false;
    }
    else {
        s3.style.borderColor = "green";
    }

    if (s1.value == "") {

        s1.style.borderColor = "red";
        return false;
    }
    else {
        s1.style.borderColor = "green";
    }

    if (s2.value == "") {

        s2.style.borderColor = "red";
        return false;
    }
    else {
        s2.style.borderColor = "green";
    }


    $('#editSaleModal').modal('hide');
    Swal.fire({
        title: "Киритилган маълумотларини тасдиқлайсизми?",
        text: "",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (result) {
        if (result.isConfirmed) {

            $.ajax({
                type: "POST",
                url: "/SimpleUser/OneTimePaymentAssets/Edit/",
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
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
                        dataTable7.ajax.reload();
                        dataTable8.ajax.reload();
                        dataTable9.ajax.reload();
                        dataTable10.ajax.reload();
                        dataTable11.ajax.reload();
                        dataTable12.ajax.reload();
                        dataTable13.ajax.reload();
                        dataTable14.ajax.reload();
                    }

                    else {
                        Swal.fire(data.message);
                    }
                }
            });

        }
    })
});

$('#contract-edit').on('click', function (e) {
    e.preventDefault();

    var form = $('#demo-form3')[0];

    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }
    if (_v5.value == "") {

        _v5.style.borderColor = "red";
        return false;
    }
    else {
        _v5.style.borderColor = "green";
    }

    if (_v.value == "" || _v.value <= 0 || isNaN(_v.value)) {

        _v.style.borderColor = "red";
        return false;
    }
    else {
        _v.style.borderColor = "green";
    }

    if (_v3.value == "" || (!/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(_v3.value) && !/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(_v3.value))) {

        _v3.style.borderColor = "red";
        return false;
    }
    else {
        _v3.style.borderColor = "green";
    }

    if (_v4.value == "") {

        _v4.style.borderColor = "red";
        return false;
    }
    else {
        _v4.style.borderColor = "green";
    }


    if (_v2.value == "" || _v2.value <= 0 || isNaN(_v2.value)) {

        _v2.style.borderColor = "red";
        return false;
    }

    else {
        _v2.style.borderColor = "green";
    }

    $('#editContractModal').modal('hide');

    Swal.fire({
        title: "Киритилган маълумотларини тасдиқлайсизми?",
        text: "",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (result) {
        if (result.isConfirmed) {

            $.ajax({
                type: "POST",
                url: "/SimpleUser/OneTimePaymentAssets/EditContract/",
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: "Таҳрирланди!",
                            text: data.message,
                            icon: "success"
                        });
                        
                        dataTable1.ajax.reload();
                        dataTable2.ajax.reload();
                        dataTable3.ajax.reload();
                        dataTable4.ajax.reload();
                        dataTable5.ajax.reload();
                        dataTable6.ajax.reload();
                        dataTable7.ajax.reload();
                        dataTable8.ajax.reload();
                        dataTable9.ajax.reload();
                        dataTable10.ajax.reload();
                        dataTable11.ajax.reload();
                        dataTable12.ajax.reload();
                        dataTable13.ajax.reload();
                        dataTable14.ajax.reload();
                    }

                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });


    

});

$('#contract-submit').on('click', function (e) {
    e.preventDefault();
    //var form = $(this).parents('form');
    var form = $('#demo-form2')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    if (v5.value == "") {

        v5.style.borderColor = "red";
        return false;
    }
    else {
        v5.style.borderColor = "green";
    }

    if (v.value == "" || v.value <= 0 || isNaN(v.value)) {

        v.style.borderColor = "red";
        return false;
    }
    else {
        v.style.borderColor = "green";
    }

    if (v3.value == "" || !/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(v3.value))
    {

        v3.style.borderColor = "red";
        return false;
    }
    else {
        v3.style.borderColor = "green";
    }

    if (v4.value == "") {

        v4.style.borderColor = "red";
        return false;
    }
    else {
        v4.style.borderColor = "green";
    }

    if (v2.value == "" || v2.value <= 0 || isNaN(v2.value)) {

        v2.style.borderColor = "red";

        return false;
    }


    else {
        v2.style.borderColor = "green";
    }

    var file = document.getElementById("AgreementFileIn");

    if (!file.value) {
        var nofileMsg = document.getElementById("noFileMsg");

        nofileMsg.classList.add("fa");
        nofileMsg.classList.add("fa-exclamation-circle");
        nofileMsg.style.color = "#f27474";
        $('#noFileMsg').show();

        return false;
    }

    $('#contractModal').modal('hide');
        Swal.fire({
            title: "Шартнома формаси яратиш учун тайёр",
            text: "",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Тасдиқлайман!',
            cancelButtonText: 'Бекор қилиш',
        }).then(function (result) {
            if (result.isConfirmed) {

                $.ajax({
                    type: "POST",
                    url: "/SimpleUser/OneTimePaymentAssets/CreateContract/",
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false,
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
                            dataTable7.ajax.reload();
                            dataTable8.ajax.reload();
                            dataTable9.ajax.reload();
                            dataTable10.ajax.reload();
                            dataTable11.ajax.reload();
                            dataTable12.ajax.reload();
                            dataTable13.ajax.reload();
                            dataTable14.ajax.reload();
                        }

                        else {
                            Swal.fire(data.message);
                        }
                    }
                });

            }
        })
    });

$('#act-submit').on('click', function (e) {
    e.preventDefault();
    //var form = $(this).parents('form');
    var form = $('#demo-form4')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    if (a1.value == "" || !/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(a1.value)) {

        a1.style.borderColor = "red";
        return false;
    }
    else {
        a1.style.borderColor = "green";
    }

    if (a2.value == "") {

        a2.style.borderColor = "red";
        return false;
    }
    else {
        a2.style.borderColor = "green";
    }


    var file = document.getElementById("actFileIn");

    if (!file.value) {
        var nofileMsg = document.getElementById("noFileMsg_act");

        nofileMsg.classList.add("fa");
        nofileMsg.classList.add("fa-exclamation-circle");
        nofileMsg.style.color = "#f27474";
        $('#noFileMsg_act').show();

        return false;
    }

    var file2 = document.getElementById("invoiceFileIn");

    if (!file2.value) {
        var nofileMsg2 = document.getElementById("noFileMsg_inv");

        nofileMsg2.classList.add("fa");
        nofileMsg2.classList.add("fa-exclamation-circle");
        nofileMsg2.style.color = "#f27474";
        $('#noFileMsg_inv').show();

        return false;
    }

    $('#actModal').modal('hide');
    Swal.fire({
        title: "Акт маълумотлари тайёр",
        text: "",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (result) {
        if (result.isConfirmed) {

            $.ajax({
                type: "POST",
                url: "/SimpleUser/OneTimePaymentAssets/CreateAct/",
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
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
                        dataTable7.ajax.reload();
                        dataTable8.ajax.reload();
                        dataTable9.ajax.reload();
                        dataTable10.ajax.reload();
                        dataTable11.ajax.reload();
                        dataTable12.ajax.reload();
                        dataTable13.ajax.reload();
                        dataTable14.ajax.reload();
                    }

                    else {
                        Swal.fire(data.message);
                    }
                }
            });

        }
    })
});

$('#act-edit').on('click', function (e) {
    e.preventDefault();
    //var form = $(this).parents('form');
    var form = $('#demo-form5')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    if (_a1.value == "" || (!/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(_a1.value) && !/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(_a1.value))) {

        _a1.style.borderColor = "red";
        return false;
    }
    else {
        _a1.style.borderColor = "green";
    }

    if (_a2.value == "") {

        _a2.style.borderColor = "red";
        return false;
    }
    else {
        _a2.style.borderColor = "green";
    }


    $('#editActModal').modal('hide');
    Swal.fire({
        title: "Киритилган маълумотларини тасдиқлайсизми?",
        text: "",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (result) {
        if (result.isConfirmed) {

            $.ajax({
                type: "POST",
                url: "/SimpleUser/OneTimePaymentAssets/EditAct/",
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
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
                        dataTable7.ajax.reload();
                        dataTable8.ajax.reload();
                        dataTable9.ajax.reload();
                        dataTable10.ajax.reload();
                        dataTable11.ajax.reload();
                        dataTable12.ajax.reload();
                        dataTable13.ajax.reload();
                        dataTable14.ajax.reload();
                    }

                    else {
                        Swal.fire(data.message);
                    }
                }
            });

        }
    })
});

$('#editActFile_').on('click', function (e) {
    e.preventDefault();
    var file = document.getElementById("ActFileInEdit");

    if (!file.value) {

        var nofileMsgEdit = document.getElementById("noFileActMsgEdit");

        nofileMsgEdit.classList.add("fa");
        nofileMsgEdit.classList.add("fa-exclamation-circle");
        nofileMsgEdit.style.color = "#f27474";
        $('#noFileActMsgEdit').show();

        return false;
    }

    var form = $('#actFileForm1')[0];
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    $('#editActFile').modal('hide');

    $.ajax({
            type: "POST",
            url: "/SimpleUser/OneTimePaymentAssets/ReplaceFile/",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success) {
                    console.log(data.success);
                    Swal.fire({
                        title: data.message,
                        icon: "success"
                    });
                    /* toastr.success(data.message);*/
                    dataTable1.ajax.reload();
                    dataTable2.ajax.reload();
                    dataTable3.ajax.reload();
                    dataTable4.ajax.reload();
                    dataTable5.ajax.reload();
                    dataTable6.ajax.reload();
                    dataTable7.ajax.reload();
                    dataTable8.ajax.reload();
                    dataTable9.ajax.reload();
                    dataTable10.ajax.reload();
                    dataTable11.ajax.reload();
                    dataTable12.ajax.reload();
                    dataTable13.ajax.reload();
                    dataTable14.ajax.reload();
                }

                else {
                    Swal.fire({
                        title: data.message,
                        icon: "error"
                    });
                }
            }
        });
        
});

$('#editSaleFile').click(function (e) {

    e.preventDefault();
    var file = document.getElementById("SaleFileInEdit");

    if (!file.value) {

        var nofileMsgEdit = document.getElementById("noSaleFileMsgEdit");

        nofileMsgEdit.classList.add("fa");
        nofileMsgEdit.classList.add("fa-exclamation-circle");
        nofileMsgEdit.style.color = "#f27474";
        $('#noSaleFileMsgEdit').show();

        return false;
    }

    var form = $('#editSaleFileForm')[0];
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    $('#editSaleFileModal').modal('hide');

    $.ajax({
        type: "POST",
        url: "/SimpleUser/OneTimePaymentAssets/ReplaceFile/",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.success) {
                Swal.fire({
                    title: data.message,
                    icon: "success"
                });
                /* toastr.success(data.message);*/
                dataTable1.ajax.reload();
                dataTable2.ajax.reload();
                dataTable3.ajax.reload();
                dataTable4.ajax.reload();
                dataTable5.ajax.reload();
                dataTable6.ajax.reload();
                dataTable7.ajax.reload();
                dataTable8.ajax.reload();
                dataTable9.ajax.reload();
                dataTable10.ajax.reload();
                dataTable11.ajax.reload();
                dataTable12.ajax.reload();
                dataTable13.ajax.reload();
                dataTable14.ajax.reload();
            }

            else {
                Swal.fire({
                    title: data.message,
                    icon: "error"
                });
            }
        }
    });

});

$('#editInvFile_').click(function (e) {
    e.preventDefault();
    var file = document.getElementById("InvoiceFileInEdit");

    if (!file.value) {

        var nofileMsgEdit = document.getElementById("noFileMsgInvEdit");

        nofileMsgEdit.classList.add("fa");
        nofileMsgEdit.classList.add("fa-exclamation-circle");
        nofileMsgEdit.style.color = "#f27474";
        $('#noFileMsgInvEdit').show();

        return false;
    }

    var form = $('#actFileForm2')[0];
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    $('#editActFile').modal('hide');

    $.ajax({
        type: "POST",
        url: "/SimpleUser/OneTimePaymentAssets/ReplaceFile/",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.success) {
                Swal.fire({
                    title: data.message,
                    icon: "success"
                });
                /* toastr.success(data.message);*/
                dataTable1.ajax.reload();
                dataTable2.ajax.reload();
                dataTable3.ajax.reload();
                dataTable4.ajax.reload();
                dataTable5.ajax.reload();
                dataTable6.ajax.reload();
                dataTable7.ajax.reload();
                dataTable8.ajax.reload();
                dataTable9.ajax.reload();
                dataTable10.ajax.reload();
                dataTable11.ajax.reload();
                dataTable12.ajax.reload();
                dataTable13.ajax.reload();
                dataTable14.ajax.reload();
            }

            else {
                Swal.fire({
                    title: data.message,
                    icon: "error"
                });
            }
        }
    });

});

$('#editAgFile').click(function (e) {

    e.preventDefault();
    var file = document.getElementById("AgreementFileInEdit");

    if (!file.value) {

        var nofileMsgEdit = document.getElementById("noFileMsgEdit");

        nofileMsgEdit.classList.add("fa");
        nofileMsgEdit.classList.add("fa-exclamation-circle");
        nofileMsgEdit.style.color = "#f27474";
        $('#noFileMsgEdit').show();

        return false;
    }

    var form = $('#editContractFileForm')[0];
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    $('#editContractFile').modal('hide');

    $.ajax({
        type: "POST",
        url: "/SimpleUser/OneTimePaymentAssets/ReplaceFile/",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.success) {
                Swal.fire({
                    title: data.message,
                    icon: "success"
                });
                /* toastr.success(data.message);*/
                dataTable1.ajax.reload();
                dataTable2.ajax.reload();
                dataTable3.ajax.reload();
                dataTable4.ajax.reload();
                dataTable5.ajax.reload();
                dataTable6.ajax.reload();
                dataTable7.ajax.reload();
                dataTable8.ajax.reload();
                dataTable9.ajax.reload();
                dataTable10.ajax.reload();
                dataTable11.ajax.reload();
                dataTable12.ajax.reload();
                dataTable13.ajax.reload();
                dataTable14.ajax.reload();
            }

            else {
                Swal.fire({
                    title: data.message,
                    icon: "error"
                });
            }
        }
    });

});

$('#editFileSol').click(function(){

    $('#editSaleModal').modal('hide');
    $('#editSaleFileModal').modal('hide');
});

function getFile(fileName) {

    $("#display-AgreementFile").html(fileName.replace(/^.*[\\\/]/, ''));
    $('#noFileMsg').hide();
}

function getActFile(fileName) {

    $("#display-actFile").html(fileName.replace(/^.*[\\\/]/, ''));
    $('#noFileMsg_act').hide();
}

function getInvoiceFile(fileName) {

    $("#display-invoiceFile").html(fileName.replace(/^.*[\\\/]/, ''));
    $('#noFileMsg_inv').hide();
}

function getFileForEdit(fileName) {

    $("#display-AgFile").html(fileName.replace(/^.*[\\\/]/, ''));
    $('#noFileMsgEdit').hide();
}

function getFileForSaleEdit(fileName) {

    $("#display-SaleFile").html(fileName.replace(/^.*[\\\/]/, ''));
    $('#noSaleFileMsgEdit').hide();

}

function getFileForActEdit(fileName) {

    $("#display-AcFile").html(fileName.replace(/^.*[\\\/]/, ''));
    $('#noFileActMsgEdit').hide();
}

function getFileForInvEdit(fileName) {

    $("#display-InvFile").html(fileName.replace(/^.*[\\\/]/, ''));
    $('#noFileMsgInvEdit').hide();
}

function Cancel(id, target) {

    var title = "";
    if (target == 1) {
        title = "Сотишни бекор қилмоқчимисиз?"
    }
    if (target == 2) {
        title = "Шартномани бекор қилмоқчимисиз?"
    }

    Swal.fire({
        title: title,
        text: "Амалиёт бажарилгач, савдо маълумотларини «Сотилмади» бўлимидан кўриш мумкин!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Йўқ',

    }).then(function (result) {
        if (result.isConfirmed) {

            var data = {

                id: id,
                target:target

            };

            $.ajax({
                type: "POST",
                url: "/SimpleUser/OneTimePaymentAssets/CancelSale/",
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
                        dataTable7.ajax.reload();
                        dataTable8.ajax.reload();
                        dataTable9.ajax.reload();
                        dataTable10.ajax.reload();
                        dataTable11.ajax.reload();
                        dataTable12.ajax.reload();
                        dataTable13.ajax.reload();
                        dataTable14.ajax.reload();
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

    }).then(function (result) {
        if (result.isConfirmed) {
            var data = {
                id: id,
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

function Delete(id, target) {
    Swal.fire({
        title: "Ўчиришни хоҳлайсизми?",
        text: "Ўчирилгач маълумотларни қайта тиклай олмайсиз!",
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
                url: "/SimpleUser/OneTimePaymentAssets/Delete/",
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
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
                        dataTable7.ajax.reload();
                        dataTable8.ajax.reload();
                        dataTable9.ajax.reload();
                        dataTable10.ajax.reload();
                        dataTable11.ajax.reload();
                        dataTable12.ajax.reload();
                        dataTable13.ajax.reload();
                        dataTable14.ajax.reload();

                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });

}

function Confirm(id, target) {
    Swal.fire({
        title: "Киритилган маълумотларини тасдиқлайсизми?",
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
                url: "/SimpleUser/OneTimePaymentAssets/Confirm/",
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
                        dataTable7.ajax.reload();
                        dataTable8.ajax.reload();
                        dataTable9.ajax.reload();
                        dataTable10.ajax.reload();
                        dataTable11.ajax.reload();
                        dataTable12.ajax.reload();
                        dataTable13.ajax.reload();
                        dataTable14.ajax.reload();
                    }

                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }


    });
}