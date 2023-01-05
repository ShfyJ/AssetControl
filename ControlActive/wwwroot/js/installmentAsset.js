var dataTable1;

$(document).ready(function () {

    loadDataTable();

    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust().responsive.recalc();
    });

    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();

});



function loadDataTable() {
    dataTable1 = $('#table1').DataTable({
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
            "url": "/SimpleUser/InstallmentAssets/GetAssets",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = 1;
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {},
                "render": function (data) {
                    if (data.installmentAsset.confirmed && data.installmentAsset.status == 1)
                        return '<h6>' + data.name + '<span style="color:green" class="icofont icofont-tick-boxed"></span></h6><span style="font-weight:bold">' + data.installmentAsset.assetBuyerName + '</span><span style="font-size:12px;"> шартнома тузди</span>'
                    else if (!data.installmentAsset.confirmed && data.installmentAsset.status == 1)
                        return '<h6>' + data.name + '<span style="color:#ad8407" class="icofont icofont-exclamation-circle"></span></h6><span style="color:red; font-weight:bold">тасдиқланмаган</span>'
                    else if (data.installmentAsset.status == 2)
                        return '<h6>' + data.name + '<span style="color:green" class="icofont icofont-tick-boxed"></span></h6><span style="font-weight:bold">' + data.installmentAsset.assetBuyerName + '</span><span style="font-size:12px;"> сотиб олди</span>'
                    else if (data.installmentAsset.status == 0)
                        return '<h6>' + data.name + '<span style="color:green" class="icofont icofont-tick-boxed"></span></h6><span style="font-weight:bold">' + data.installmentAsset.assetBuyerName + '</span><span style="font-size:12px;"> га сотилмади</span>'

                }
            },

            {
                "data": {},
                "render": function (data) {
                    if (data.installmentAsset.status == 1)
                        return '<h6><span class="badge badge-warning" style="color:white">Кутилмоқда</span></h6>'
                    else if (data.installmentStep2 != null) {
                        if (!data.installmentStep2.confirmed && data.installmentAsset.status == 2)
                            return '<h6><span class="badge badge-warning" style="color:white">Муваффақиятли</span></h6><p style="color:red">акт тасдиқланмаган</p>'
                        else if (data.installmentStep2.confirmed && data.installmentAsset.status == 2)
                            return '<h6><span class="badge badge-success" style="color:white">Муваффақиятли</span></h6>'
                    }
                    else if (data.installmentAsset.status == 0)
                        return '<h6><span class="badge badge-danger" style="color:white">Сотилмади</span></h6>'
                }

            },

            { "data": "installmentAsset.amountOfAssetSold", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<a  href="/SimpleUser/InstallmentAssets/DownloadFile/' + data.installmentAsset.solutionFileId + '"><i class="fa fa-download">Ҳужжатни юклаб олиш</i></a>'

                }

            },


            {
                "data": {},
                "render": function (data) {
                    
                    var assetStr = encodeURIComponent(JSON.stringify(data));
                    if (!data.installmentAsset.confirmed && data.installmentAsset.status == 1) {
                        return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=Edit('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailSale('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete(${data.installmentAsset.installmentAssetId}` + `,` + 1 + `,` + 1 + `)><i style="color:red;" class="fa fa-trash-o"></i></button>

                        <button style="color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm(${data.installmentAsset.installmentAssetId}` + `,` + 1 + `)><i style="font-size:18px;" class="icofont icofont-safety"></i></button>`
                    }

                    else if (data.installmentAsset.confirmed && data.installmentAsset.status == 1) {
                        return `
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailSale('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Акт тузиш" style="cursor:pointer" onclick="AddAct(${data.installmentAsset.installmentAssetId},'${data.name}')" ><i style="color:blue" class="icofont icofont-paper"></i> <i style="color:blue" class="icofont icofont-plus"></i></button>
                         <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Сотувни бекор қилиш" style="cursor:pointer" onclick=CancelSale('${data.installmentAsset.installmentAssetId}') ><i style="color:red" class="icofont icofont-close-circled"></i></button>
                        `
                    }

                    else if (data.installmentStep2 != null) {
                        if (!data.installmentStep2.confirmed && data.installmentAsset.status == 2) {
                            return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=EditAct('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                            <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailAct('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                            <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete(${data.installmentStep2.installmentStep2Id}` + `,` + 1 + `,` + 2 + `)><i style="color:red;" class="fa fa-trash-o"></i></button>
                             <button style="color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm(${data.installmentStep2.installmentStep2Id}` + `,` + 2 + `)><i style="font-size:18px;" class="icofont icofont-safety"></i></button>`
                        }
                         else if (data.installmentStep2.confirmed && data.installmentAsset.status == 2)
                             return `
                             <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailAct('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                                `
                    }

                   
                    else if (data.installmentAsset.status == 0) {
                        return `
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailSale('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        `
                    }
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
    dataTable2 = $('#table2').DataTable({
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
            "url": "/SimpleUser/InstallmentAssets/GetAssets",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data = 2;
                return JSON.stringify(data);
            }
        },
        "columns": [
            {
                "data": {},
                "render": function (data) {
                    if (data.installmentAsset.confirmed && data.installmentAsset.status == 1)
                        return '<h6>' + data.name + '<span style="color:green" class="icofont icofont-tick-boxed"></span></h6><span style="font-weight:bold">' + data.installmentAsset.assetBuyerName + '</span><span style="font-size:12px;"> шартнома тузди</span>'
                    else if (!data.installmentAsset.confirmed && data.installmentAsset.status == 1)
                        return '<h6>' + data.name + '<span style="color:#ad8407" class="icofont icofont-exclamation-circle"></span></h6><span style="color:red; font-weight:bold">тасдиқланмаган</span>'
                    else if (data.installmentAsset.status == 2)
                        return '<h6>' + data.name + '<span style="color:green" class="icofont icofont-tick-boxed"></span></h6><span style="font-weight:bold">' + data.installmentAsset.assetBuyerName + '</span><span style="font-size:12px;"> сотиб олди</span>'
                    else if (data.installmentAsset.status == 0)
                        return '<h6>' + data.name + '<span style="color:green" class="icofont icofont-tick-boxed"></span></h6><span style="font-weight:bold">' + data.installmentAsset.assetBuyerName + '</span><span style="font-size:12px;"> га сотилмади</span>'

                }
            },

            {
                "data": {},
                "render": function (data) {
                    if (data.installmentAsset.status == 1)
                        return '<h6><span class="badge badge-warning" style="color:white">Кутилмоқда</span></h6>'
                    else if (data.installmentStep2 != null) {
                        if (!data.installmentStep2.confirmed && data.installmentAsset.status == 2)
                            return '<h6><span class="badge badge-warning" style="color:white">Муваффақиятли</span></h6><p style="color:red">акт тасдиқланмаган</p>'
                        else if (data.installmentStep2.confirmed && data.installmentAsset.status == 2)
                            return '<h6><span class="badge badge-success" style="color:white">Муваффақиятли</span></h6>'
                    }
                    else if (data.installmentAsset.status == 0)
                        return '<h6><span class="badge badge-danger" style="color:white">Сотилмади</span></h6>'
                }

            },

            { "data": "installmentAsset.amountOfAssetSold", "width": "15%" },
            {
                "data": {},
                "render": function (data) {
                    return '<a  href="/SimpleUser/InstallmentAssets/DownloadFile/' + data.installmentAsset.solutionFileId + '"><i class="fa fa-download">Ҳужжатни юклаб олиш</i></a>'

                }

            },


            {
                "data": {},
                "render": function (data) {
                    var assetStr = encodeURIComponent(JSON.stringify(data));
                    if (!data.installmentAsset.confirmed && data.installmentAsset.status == 1) {
                        return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=Edit('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button id="inDetail" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailSale('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete(${data.installmentAsset.installmentAssetId}` + `,` + 2 + `,` + 1 +`)><i style="color:red;" class="fa fa-trash-o"></i></button>

                        <button style="color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm('${data.installmentAsset.installmentAssetId}` + `,` + 1 + `)><i style="font-size:18px;" class="icofont icofont-safety"></i></button>`
                    }

                    else if (data.installmentAsset.confirmed && data.installmentAsset.status == 1) {
                        return `
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailSale('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Акт тузиш" style="cursor:pointer" onclick="AddAct(${data.installmentAsset.installmentAssetId},'${data.name}')" ><i style="color:blue" class="icofont icofont-paper"></i> <i style="color:blue" class="icofont icofont-plus"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Сотувни бекор қилиш" style="cursor:pointer" onclick=CancelSale('${data.installmentAsset.installmentAssetId}') ><i style="color:red" class="icofont icofont-close-circled"></i></button>
                        `
                    }

                    else if (!data.installmentStep2.confirmed && data.installmentAsset.status == 2) {
                        return `<button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Таҳрирлаш" style="cursor:pointer" onclick=Edit('${assetStr}')><i style="color:#eb8c21" class="fa fa-edit"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailAct('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Ўчириш" style="cursor:pointer" onclick=Delete(${data.installmentStep2.installmentStep2Id}` + `,` + 2 + `,` + 2 +`)><i style="color:red;" class="fa fa-trash-o"></i></button>
                         <button style="color:green;" data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Тасдиқлаш" style="cursor:pointer" onclick = Confirm('${data.installmentStep2.installmentStep2Id}` + `,` + 2 + `)><i style="font-size:18px;" class="icofont icofont-safety"></i></button>`

                    }

                    else if (data.installmentStep2.confirmed && data.installmentAsset.status == 2) {
                        return `
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailAct('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        `
                    }


                    else if (data.installmentAsset.status == 0) {
                        return `
                        <button data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="Батафсил кўриш" style="cursor:pointer" onclick=DetailSale('${assetStr}') ><i style="color:blue; " class="fa fa-bars"></i></button>
                        `
                    }
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
}

function Delete(id, target, step) {
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
                target: target,
                step:step
            }
            $.ajax({
                type: "POST",
                url: "/SimpleUser/InstallmentAssets/Delete/",
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
                url: "/SimpleUser/InstallmentAssets/Confirm/",
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
                      }

                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }


    });
}

function CancelSale(id) {
    Swal.fire({
        title: "Сотувни бекор қилмоқчимисиз?!",       
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Ҳа,тасдиқлайман',
        cancelButtonText: 'Йўқ',

    }).then(function (result) {
        if (result.isConfirmed) {

            $.ajax({
                type: "POST",
                url: "/SimpleUser/InstallmentAssets/CancelSale/",
                data: JSON.stringify(id),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: "Муваффақиятли!",
                            text: data.message,
                            icon: "success"
                        });
                        /* toastr.success(data.message);*/
                        dataTable1.ajax.reload();
                        dataTable2.ajax.reload();
                    }

                    else {
                        Swal.fire({
                            title: "Хатолик!",
                            text: data.message,
                            icon: "error"
                        });
                    }
                }
            });
        }


    });
}

function AddAct(id, assetName) {
    $("#_asset_name").val(assetName);
    $("#_installmentAssetId").val(id);
    $('#createActModal').modal('show');
}

var a1 = document.getElementById('actAndAssetDate');
var a2 = document.getElementById('actAndAssetNumber');
var a3 = document.getElementById('ActFileIn');

function EditAct(e) {
    var data = JSON.parse(decodeURIComponent(e));
    $("#_asset_name_").val(data.name);
    $("#_installmentStep2Id").val(data.installmentStep2.installmentStep2Id);
    $("#_actAndAssetDate").val(data.installmentStep2.actAndAssetDateStr);
    $("#_actAndAssetNumber").val(data.installmentStep2.actAndAssetNumber);
    $("#actFileLink").prop("href", data.installmentStep2.actAndAssetFileLink);
    $('#editActModal').modal('show');
}

var _a1 = document.getElementById('_actAndAssetDate');
var _a2 = document.getElementById('_actAndAssetNumber');
var _a3 = document.getElementById('_ActFileIn');

function DetailSale(e) {

    var data = JSON.parse(decodeURIComponent(e));
    var assetName = document.getElementById("assetName");
    var orgName = document.getElementById("organization");
    var solutionNumber = document.getElementById("sNumber");
    var solutionDate = document.getElementById("sDate");
    var solutionFile = document.getElementById("sFile");
    var biddingDate = document.getElementById("bDate");
    var buyerName = document.getElementById("buyerName");
    var amountSold = document.getElementById("amountSold");
    var aDate = document.getElementById("aDate");
    var aNumber = document.getElementById("aNumber");
    var agFile = document.getElementById("agFile");
    var iTime = document.getElementById("iTime");
    var initP = document.getElementById("initP");
    var pType = document.getElementById("pType");
    var aPayment = document.getElementById("aPayment");

    assetName.innerHTML = data.name;
    orgName.innerHTML = data.installmentAsset.governingBodyName;
    solutionNumber.innerHTML = data.installmentAsset.solutionNumber;
    solutionDate.innerHTML = data.installmentAsset.solutionDateStr;
    solutionFile.href = data.installmentAsset.solutionFileLink;
    solutionFile.innerHTML = "Қарор ҳужжати";
    biddingDate.innerHTML = data.installmentAsset.biddingDateStr;
    buyerName.innerHTML = data.installmentAsset.assetBuyerName;
    amountSold.innerHTML = data.installmentAsset.amountOfAssetSold;
    aDate.innerHTML = data.installmentAsset.aggreementDateStr;
    aNumber.innerHTML = data.installmentAsset.aggreementNumber;
    agFile.href = data.installmentAsset.aggreementFileLink;
    agFile.innerHTML = "Шартнома ҳужжати";
    iTime.innerHTML = data.installmentAsset.installmentTime;
    initP.innerHTML = data.installmentAsset.actualInitPayment;
    if (data.installmentAsset.paymentPeriodType == 12)
        pType.innerHTML = "ой";
    if (data.installmentAsset.paymentPeriodType == 4)
        pType.innerHTML = "квартал";
    if (data.installmentAsset.paymentPeriodType == 4)
        pType.innerHTML = "йил";
    aPayment.innerHTML = data.installmentAsset.actualPayment;

    $('#saleDModal').modal('show');
}

function DetailAct(e) {

    var data = JSON.parse(decodeURIComponent(e));
    var assetName = document.getElementById("assetName_");
    var orgName = document.getElementById("organization_");
    var solutionNumber = document.getElementById("sNumber_");
    var solutionDate = document.getElementById("sDate_");
    var solutionFile = document.getElementById("sFile_");
    var biddingDate = document.getElementById("bDate_");
    var buyerName = document.getElementById("buyerName_");
    var amountSold = document.getElementById("amountSold_");
    var aDate = document.getElementById("aDate_");
    var aNumber = document.getElementById("aNumber_");
    var agFile = document.getElementById("agFile_");
    var iTime = document.getElementById("iTime_");
    var initP = document.getElementById("initP_");
    var pType = document.getElementById("pType_");
    var aPayment = document.getElementById("aPayment_");
    var actDate_ = document.getElementById("actDate_");
    var actNumber_ = document.getElementById("actNumber_");
    var actFile_ = document.getElementById("actFile_");

    assetName.innerHTML = data.name;
    orgName.innerHTML = data.installmentAsset.governingBodyName;
    solutionNumber.innerHTML = data.installmentAsset.solutionNumber;
    solutionDate.innerHTML = data.installmentAsset.solutionDateStr;
    solutionFile.href = data.installmentAsset.solutionFileLink;
    solutionFile.innerHTML = "Қарор ҳужжати";
    biddingDate.innerHTML = data.installmentAsset.biddingDateStr;
    buyerName.innerHTML = data.installmentAsset.assetBuyerName;
    amountSold.innerHTML = data.installmentAsset.amountOfAssetSold;
    aDate.innerHTML = data.installmentAsset.aggreementDateStr;
    aNumber.innerHTML = data.installmentAsset.aggreementNumber;
    agFile.href = data.installmentAsset.aggreementFileLink;
    agFile.innerHTML = "Шартнома ҳужжати";
    iTime.innerHTML = data.installmentAsset.installmentTime;
    initP.innerHTML = data.installmentAsset.actualInitPayment;
    if (data.installmentAsset.paymentPeriodType == 12)
        pType.innerHTML = "ой";
    if (data.installmentAsset.paymentPeriodType == 4)
        pType.innerHTML = "квартал";
    if (data.installmentAsset.paymentPeriodType == 4)
        pType.innerHTML = "йил";
    aPayment.innerHTML = data.installmentAsset.actualPayment;

    actDate_.innerHTML = data.installmentStep2.actAndAssetDateStr;
    actNumber_.innerHTML = data.installmentStep2.actAndAssetNumber;
    actFile_.href = data.installmentStep2.actAndAssetFileLink;
    actFile_.innerHTML = "Акт ҳужжати";

    $('#actDModal').modal('show');
}

var v_ = document.getElementById("asset_name_");
var v2_ = document.getElementById("orgName_");
var v3_ = document.getElementById("SolutionFileIn_");
var v4_ = document.getElementById("solutionDate_");
var v5_ = document.getElementById("solutionNumber_");
var v6_ = document.getElementById("biddingDate_");
var v7_ = document.getElementById("assetBuyerName_");
var v8_ = document.getElementById("amountOfAssetSold_");
var v9_ = document.getElementById("aggreementDate_");
var v10_ = document.getElementById("aggreementNumber_");
var v11_ = document.getElementById("AgreementFileIn_");
var v12_ = document.getElementById("actualInitPayment_");
var v13_ = document.getElementById("actualPayment_");
var v14_ = document.getElementById("periodSelect_");

function Edit(e) {

    var data = JSON.parse(decodeURIComponent(e));

    $('#asset_name_').append($('<option>', {
        value:"",
        text: data.name
    }));

    $("asset_name_ select").text(data.name);

    $("#step1Id").val(data.installmentAsset.installmentAssetId);

    $('#orgName_').val(data.installmentAsset.governingBodyName);

    $('#solutionDate_').val(data.installmentAsset.solutionDateStr);

    $('#solutionNumber_').val(data.installmentAsset.solutionNumber);
    $('#biddingDate_').val(data.installmentAsset.biddingDateStr);
    $('#assetBuyerName_').val(data.installmentAsset.assetBuyerName);
    $('#amountOfAssetSold_').val(data.installmentAsset.amountOfAssetSold);

    $('#aggreementDate_').val(data.installmentAsset.aggreementDateStr);
    $('#aggreementNumber_').val(data.installmentAsset.aggreementNumber);
    $("#sLink").prop("href", data.installmentAsset.solutionFileLink);
    $("#agLink").prop("href", data.installmentAsset.aggreementFileLink);
    $('#actualInitPayment_').val(data.installmentAsset.actualInitPayment);
    $('#installmentTime_').val(data.installmentAsset.installmentTime);
    $('#periodSelect_').val(data.installmentAsset.paymentPeriodType).change();
    $('#actualPayment_').val(data.installmentAsset.actualPayment);

     $('#editModal').modal('show');
}

var v = document.getElementById("asset_name");
var v2 = document.getElementById("orgName");
var v3 = document.getElementById("SolutionFileIn");
var v4 = document.getElementById("solutionDate");
var v5 = document.getElementById("solutionNumber");
var v6 = document.getElementById("biddingDate");
var v7 = document.getElementById("assetBuyerName");
var v8 = document.getElementById("amountOfAssetSold");
var v9 = document.getElementById("aggreementDate");
var v10 = document.getElementById("aggreementNumber");
var v11 = document.getElementById("AgreementFileIn");
var v12 = document.getElementById("actualInitPayment");
var v13 = document.getElementById("actualPayment");
var v14 = document.getElementById("periodSelect");

$('#asset_name').on('click', function (e) {

    if (v.value == "") {

        v.style.borderColor = "red";
    }

    else {
        v.style.borderColor = "green";
    }

});

$('#orgName, #orgName_').on('keyup', function (e) {

    if (v2.value == "") {

        v2.style.borderColor = "red";
    }

    else {
        v2.style.borderColor = "green";
    }

});

$('#display-solutionFile').on('click', function () {

    if (v3.value == "") {

        $('#display-solutionFile').css('borderColor', 'red');
    }

});

$('#solutionNumber, #solutionNumber_').on('keyup', function (e) {

    if (v5.value == "") {

        v5.style.borderColor = "red";
    }

    else {
        v5.style.borderColor = "green";
    }

});

$("#solutionDate, #solutionDate_").datepicker({
    onSelect: function () {
        if (v4.value == "") {

            v4.style.borderColor = "red";
        }

        else {
            v4.style.borderColor = "green";
        }
    }
});

$("#biddingDate, #biddingDate_").datepicker({
    onSelect: function () {
        if (v6.value == "") {

            v6.style.borderColor = "red";
        }

        else {
            v6.style.borderColor = "green";
        }
    }
});

$("#aggreementDate, #aggreementDate_").datepicker({
    onSelect: function () {
        if (v9.value == "") {

            v9.style.borderColor = "red";
        }

        else {
            v9.style.borderColor = "green";
        }
    }
});

$('#assetBuyerName, #assetBuyerName_').on('keyup', function () {

    if (v7.value == "") {

        v7.style.borderColor = "red";
    }

    else {
        v7.style.borderColor = "green";
    }

});

$('#amountOfAssetSold, #amountOfAssetSold_').on('keyup', function (e) {

    if (v8.value == "") {

        v8.style.borderColor = "red";
    }

    else {
        v8.style.borderColor = "green";
    }

});

$('#aggreementNumber, #aggreementNumber_').on('keyup', function (e) {

    if (v10.value == "") {

        v10.style.borderColor = "red";
    }

    else {
        v10.style.borderColor = "green";
    }

});

$('#display-AgreementFile').on('click', function (e) {

    if (v11.value == "") {

        $('#display-AgreementFile').css('borderColor', 'red');
    }

});

$('#actualInitPayment, #actualInitPayment_').on('keyup', function (e) {

    if (v12.value == "") {

        v12.style.borderColor = "red";
    }

    else {
        v12.style.borderColor = "green";
    }

});

$('#actualPayment, #actualPayment_').on('keyup', function (e) {

    if (v13.value == "") {

        v13.style.borderColor = "red";
    }

    else {
        v13.style.borderColor = "green";
    }

});

$('#periodSelect, #periodSelect_ ').on('click', function (e) {

    if (v14.value == "") {

        v14.style.borderColor = "red";
    }

    else {
        v14.style.borderColor = "green";
    }

});

$('#create-btn').on('click', function (e) {
    e.preventDefault();
    //var form = $(this).parents('form');
    var form = $('#create-form')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    if (v.value == "") {

        v.style.borderColor = "red";
        return false;
    }

    else {
        v.style.borderColor = "green";
    }

    if (v2.value == "") {

        v2.style.borderColor = "red";
        return false;
    }

    else {
        v2.style.borderColor = "green";
    }

    if (!v3.value) {
        var noSFileMsg = document.getElementById("noSFileMsg");
        $("#display-solutionFile").css('borderColor','red');
        noSFileMsg.classList.add("fa");
        noSFileMsg.classList.add("fa-exclamation-circle");
        noSFileMsg.style.color = "#f27474";
        $('#noSFileMsg').show();

        return false;
    }

    if (v4.value == "" || !/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(v4.value)) {

        v4.style.borderColor = "red";
        return false;
    }
    else {
        v4.style.borderColor = "green";
    }

    if (v5.value == "") {

        v5.style.borderColor = "red";
        return false;
    }

    else {
        v5.style.borderColor = "green";
    }

    if (v6.value == "" || !/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(v6.value)) {

        v6.style.borderColor = "red";
        return false;
    }
    else {
        v6.style.borderColor = "green";
    }

    if (v7.value == "") {

        v7.style.borderColor = "red";
        return false;
    }

    else {
        v7.style.borderColor = "green";
    }

    if (v8.value == "" || v8.value <= 0 || isNaN(v8.value)) {

        v8.style.borderColor = "red";
        return false;
    }
    else {
        v8.style.borderColor = "green";
    }

    if (v9.value == "" || !/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(v9.value)) {

        v9.style.borderColor = "red";
        return false;
    }
    else {
        v9.style.borderColor = "green";
    }

    if (v10.value == "") {

        v10.style.borderColor = "red";
        return false;
    }

    else {
        v10.style.borderColor = "green";
    }

    if (!v11.value) {
        var noAFileMsg = document.getElementById("noAFileMsg");
        $("#display-AgreementFile").css('borderColor','red');
        noAFileMsg.classList.add("fa");
        noAFileMsg.classList.add("fa-exclamation-circle");
        noAFileMsg.style.color = "#f27474";
        $('#noAFileMsg').show();

        return false;
    }

    if (v12.value == "") {

        v12.style.borderColor = "red";
        return false;
    }
    else {
        v12.style.borderColor = "green";
    }

    if (v13.value == "") {

        v13.style.borderColor = "red";
        return false;
    }
    else {
        v13.style.borderColor = "green";
    }

    $('#createModal').modal('hide');

    $.ajax({
        type: "POST",
        url: "/SimpleUser/InstallmentAssets/Create/",
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
            }

            else {
                Swal.fire(data.message);
            }
        }
    });

});

$('#edit-btn').on('click', function (e) {
    e.preventDefault();
    //var form = $(this).parents('form');
    var form = $('#edit-form')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    v_.style.borderColor = "green";

    if (v2_.value == "") {

        v2_.style.borderColor = "red";
        return false;
    }

    else {
        v2_.style.borderColor = "green";
    }


    if (v4_.value == "" || !(/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(v4_.value) || /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(v4_.value) )) {

        v4_.style.borderColor = "red";
        return false;
    }
    else {
        v4_.style.borderColor = "green";
    }

    if (v5_.value == "") {

        v5_.style.borderColor = "red";
        return false;
    }

    else {
        v5_.style.borderColor = "green";
    }

    if (v6_.value == "" || !(/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(v6_.value) || /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(v6_.value))) {

        v6_.style.borderColor = "red";
        return false;
    }
    else {
        v6_.style.borderColor = "green";
    }

    if (v7_.value == "") {

        v7_.style.borderColor = "red";
        return false;
    }
    else {
        v7_.style.borderColor = "green";
    }

    if (v8_.value == "" || v8_.value <= 0 || isNaN(v8_.value)) {

        v8_.style.borderColor = "red";
        return false;
    }
    else {
        v8_.style.borderColor = "green";
    }

    if (v9_.value == "" || !(/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(v9_.value) || /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(v9_.value)) ) {

        v9_.style.borderColor = "red";
        return false;
    }
    else {
        v9_.style.borderColor = "green";
    }

    if (v10_.value == "") {

        v10_.style.borderColor = "red";
        return false;
    }
    else {
        v10_.style.borderColor = "green";
    }

    if (v12_.value == "") {

        v12_.style.borderColor = "red";
        return false;
    }
    else {
        v12_.style.borderColor = "green";
    }

    if (v13_.value == "") {

        v13_.style.borderColor = "red";
        return false;
    }
    else {
        v13_.style.borderColor = "green";
    }

    $('#editModal').modal('hide');

    $.ajax({
        type: "POST",
        url: "/SimpleUser/InstallmentAssets/Edit/",
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
            }

            else {
                Swal.fire(data.message);
            }
        }
    });

});

$('#createAct-btn').on('click', function (e) {

    e.preventDefault();
    //var form = $(this).parents('form');
    var form = $('#create-act-form')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    if (a2.value == "") {

        a2.style.borderColor = "red";
        return false;
    }

    else {
        a2.style.borderColor = "green";
    }

    if (a1.value == "" || !(/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(a1.value) || /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(a1.value))) {

        a1.style.borderColor = "red";
        return false;
    }
    else {
        a1.style.borderColor = "green";
    }

    if (!a3.value) {
        var noSFileMsg = document.getElementById("noActFileMsg");
        $("#display-actFile").css('borderColor', 'red');
        noSFileMsg.classList.add("fa");
        noSFileMsg.classList.add("fa-exclamation-circle");
        noSFileMsg.style.color = "#f27474";
        $('#noActFileMsg').show();

        return false;
    }
    else {
        $("#display-actFile").css('borderColor', 'green');
    }

    $('#createActModal').modal('hide');

    $.ajax({
        type: "POST",
        url: "/SimpleUser/InstallmentAssets/CreateAct/",
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
            }

            else {
                Swal.fire({
                    title: "Хатолик",
                    text: data.message,
                    icon: "error"
                });
            }
        }
    });

});

$('#editAct-btn').on('click', function (e) {

    e.preventDefault();
    //var form = $(this).parents('form');
    var form = $('#edit-act-form')[0];
    //var url = form.attr('id');
    var formData = new FormData(form);

    for (var pair of formData.entries()) {
        console.log(pair[0] + ', ' + pair[1]);
    }

    if (_a2.value == "") {

        _a2.style.borderColor = "red";
        return false;
    }

    else {
        _a2.style.borderColor = "green";
    }

    if (_a1.value == "" || !(/^\d{1,2}\.\d{1,2}\.\d{4}$/.test(_a1.value) || /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(_a1.value))) {

        _a1.style.borderColor = "red";
        return false;
    }
    else {
        _a1.style.borderColor = "green";
    }

    
    $('#editActModal').modal('hide');

    $.ajax({
        type: "POST",
        url: "/SimpleUser/InstallmentAssets/EditAct/",
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
            }

            else {
                Swal.fire({
                    title: "Хатолик",
                    text: data.message,
                    icon: "error"
                });
                dataTable1.ajax.reload();
                dataTable2.ajax.reload();
            }
        }
    });

});

function getActFile(fileName) {

    var name = fileName.replace(/^.*[\\\/]/, '');
    $("#display-actFile").val(name);
    $("#display-actFile").css('borderColor','green');
    $('#noActFileMsg').hide();
}

function getActFile_(fileName) {

    var name = fileName.replace(/^.*[\\\/]/, '');
    $("#display-actFile_").val(name);
    $("#display-actFile_").css('borderColor', 'green');
}



