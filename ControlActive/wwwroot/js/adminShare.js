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

var dataTable;


$(document).ready(function () {
    dataTable = $('#myTable').DataTable({
        
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

        dom: 'frtip',
       scrollX: false,
       "dom": '<lf<rt>ip>',
        columnDefs: [{
            className: 'dtr-control',
            orderable: false,
            targets: 0
        }],
        order: [1, 'asc'],

        autoWidth: false,
        columnDefs: [
            { width: '20%', targets: 0 }, //step 2, column 1 out of 4
            { width: '10%', targets: 1 }, //step 2, column 2 out of 4
            { width: '15%', targets: 2 }, //step 2, column 3 out of 4
            { width: '15%', targets: 3 }, //step 2, column 1 out of 4
            { width: '10%', targets: 4 }, //step 2, column 2 out of 4
            { width: '10%', targets: 5 }, //step 2, column 3 out of 4
            { width: '20%', targets: 6, className: 'dt-body-center' }, //step 2, column 3 out of 4
        ],


        "ajax": {
            "url": "/Admin/Shares/GetConfirmed"
        },
        "columns": [
            { "data": "businessEntityName" },
            { "data": "idRegNumber"},
            { "data": "parentOrganization"},
            { "data": "activities"},
            { "data": "activityShare"},
            
            { "data": "regionName"},

            {
                "data": {},
                "render": function (data) {
                    var fdata = {
                        regCertificateLink: data.regCertificateLink,
                        orgCharterLink: data.orgCharterLink,
                        balanceSheetLink: data.balanceSheetLink,
                        financialResultLink: data.financialResultLink,
                        auditConclusionLink: data.auditConclusionLink
                    };

                    var fdataStr = encodeURIComponent(JSON.stringify(fdata));

                    return `
                            <div>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="масул шахс" onclick=getUser('${data.shareId}')  class="userC" style="cursor:pointer; width:150px">
                                    <i  class="fa fa-user"></i> &nbsp &nbsp
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="батафсил маълумот" onclick=inDetail('${data.shareId}')  class="detailC" style="cursor:pointer; width:150px">
                                    <i  class="fa fa-reorder"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="акциядорлар" onclick=GetShareHolders("${data.shareId}") class="shareholderC" style="cursor:pointer; width:120px">
                                    <i  class="icofont icofont-chart-pie"></i>  &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ҳужжатлар" onclick=showFiles('${fdataStr}')  class="filesC" style="cursor:pointer; width:150px">
                                    <i  class="fa fa-folder-open"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="таҳрирлаш" onclick=Edit('${data.shareId}')  class="editC" style="cursor:pointer; width:150px">
                                    <i  class="fas fa-edit"></i> &nbsp &nbsp
                                </a>
                             
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" onclick=Delete("/Admin/RealEstates/Delete/${data.shareId}") class="deleteC" style="cursor:pointer; width:120px">
                                    <i  class="fas fa-trash-alt"></i>  &nbsp &nbsp
                                </a>
                                
                                
                            </div>
                        `
                }
            },
            


        ],

        "language": {
            
            "lengthMenu": "Кўрсатинг _MENU_ ёзув ҳар саҳифада",
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
            "sSearch": "Қидирув:",
        },
        "lengthMenu": [20, 30, 40, 50]
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

            dataTable = $('#myTable').DataTable({
                "destroy": true,

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

                columnDefs: [{
                    className: 'dtr-control',
                    orderable: false,
                    targets: 0
                }],
                order: [1, 'asc'],

                "ajax": {
                    "url": '/Admin/Shares/GetFilteredData/',
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
                    { width: '15%', targets: 2 }, //step 2, column 3 out of 4
                    { width: '15%', targets: 3 }, //step 2, column 1 out of 4
                    { width: '10%', targets: 4 }, //step 2, column 2 out of 4
                    { width: '10%', targets: 5 }, //step 2, column 3 out of 4
                    { width: '20%', targets: 6, className: 'dt-body-center' }, //step 2, column 3 out of 4
                ],
               
                "columns": [
                    { "data": "businessEntityName" },
                    { "data": "idRegNumber" },
                    { "data": "parentOrganization" },
                    { "data": "activities" },
                    { "data": "activityShare" },

                    { "data": "regionName" },

                    {
                        "data": {},
                        "render": function (data) {
                            var fdata = {
                                regCertificateLink: data.regCertificateLink,
                                orgCharterLink: data.orgCharterLink,
                                balanceSheetLink: data.balanceSheetLink,
                                financialResultLink: data.financialResultLink,
                                auditConclusionLink: data.auditConclusionLink
                            };

                            var fdataStr = encodeURIComponent(JSON.stringify(fdata));

                            return `
                            <div>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="масул шахс" onclick=getUser('${data.shareId}')  class="userC" style="cursor:pointer; width:150px">
                                    <i  class="fa fa-user"></i> &nbsp &nbsp
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="батафсил маълумот" onclick=inDetail('${data.shareId}')  class="detailC" style="cursor:pointer; width:150px">
                                    <i  class="fa fa-reorder"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="акциядорлар" onclick=GetShareHolders("${data.shareId}") class="shareholderC" style="cursor:pointer; width:120px">
                                    <i  class="icofont icofont-chart-pie"></i>  &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ҳужжатлар" onclick=showFiles('${fdataStr}')  class="filesC" style="cursor:pointer; width:150px">
                                    <i  class="fa fa-folder-open"></i> &nbsp &nbsp
                                </a>

                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="таҳрирлаш" onclick=Edit('${data.shareId}')  class="editC" style="cursor:pointer; width:150px">
                                    <i  class="fas fa-edit"></i> &nbsp &nbsp
                                </a>
                             
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" onclick=Delete("/Admin/RealEstates/Delete/${data.shareId}") class="deleteC" style="cursor:pointer; width:120px">
                                    <i  class="fas fa-trash-alt"></i>  &nbsp &nbsp
                                </a>
                                
                                
                            </div>
                        `
                        }
                    },

                    //{
                    //    "data": {},
                    //    "render": function (data) {
                    //        return '<p><span class="badge badge-dark">' + data.share.year1 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear1 + '</span></p>'
                    //    }

                    //},
                    //{
                    //    "data": {},
                    //    "render": function (data) {
                    //        return '<p><span class="badge badge-dark">' + data.share.year2 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear2 + '</span></p>'
                    //    }

                    //},
                    //{
                    //    "data": {},
                    //    "render": function (data) {
                    //        return '<p><span class="badge badge-dark">' + data.share.year3 + '-йил:</span><span class="badge badge-light text-dark"> ' + data.share.profitOrLossOfYear3 + '</span></p>'
                    //    }

                    //},

                   
                ],

                "language": {
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
                    "sSearch": "Қидирув:",
                },
                "lengthMenu": [20, 30, 40, 50]
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
    else {
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
        $("#tab-block2").css({ 'display': '' });

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

function DownloadFile(id) {

    window.location.href = "/Admin/Shares/DownloadFile/" + id;


}
function Edit(id) {

    window.location.href = "/Admin/Shares/Edit/" + id;

}

function getUser(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/Shares/GetUser/",
        data: JSON.stringify(id),
        contentType: "application/json",

        success: function (data) {
            if (data != null) {
                $("#userFName").text(data.data.fullname);
                $("#userOrg").text(data.data.organization.organizationName);
                $("#userPosition").text(data.data.postion);
                $("#telegram").attr("href", "https://t.me/" + data.data.phoneNumber);
                $("#phone").attr("href", "tel:" + data.data.phoneNumber);
                $("#mail").attr("href", "mailto:" + data.data.phoneNumber);
                if (data.data.realEstates != null)
                    $("#realEstateCounter").text(data.data.realEstates.length);
                if (data.data.shares != null)
                    $("#sharesCounter").text(data.data.shares.length);

                var sold = 0;
                $.each(data.data.realEstates, function (index, value) {
                    if (!value.status) {
                        sold = sold + 1;
                    }
                });

                $("#soldCounter").text(sold);
                $('#userModal').modal('show');
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
        url: "/Admin/Shares/GetShare/",
        data: JSON.stringify(data),
        contentType: "application/json",

        success: function (data) {
            if (data.success) {

                $("#_sName").text(data.data.businessEntityName);
                $("#_idReg").text(data.data.idRegNumber);
                $("#_parentOrg").text(data.data.parentOrganization);
                $("#_mainAct").text(data.data.activities);
                $("#_share").text(data.data.activityShare);
                $("#_foundationY").text(data.data.foundationYearStr);
                $("#_regDate").text(data.data.stateRegistrationDateStr);
                $("#_reg").text(data.data.regionOfObject.regionName);
                $("#_dist").text(data.data.districtOfObject.districtName);
                $("#_address").text(data.data.address);
                $("#_authCap").text(data.data.authorizedCapital);
                $("#_numOfShares").text(data.data.numberOfShares);
                $("#_valueOf").text(data.data.parValueOfShares);
                $("#_adminStaff").text(data.data.administrativeStaff);
                $("#_productPers").text(data.data.productionPersonal);
                $("#_allPers").text(data.data.allStaff);
                $("#_averageSal").text(data.data.averageMonthlySalary);
                $("#_maintCost").text(data.data.maintanenceCostForYear);
                $("#_productArea").text(data.data.productionArea);
                $("#_buildArea").text(data.data.buildingsArea);
                $("#_allArea").text(data.data.allArea);
                $("#_amountPay").text(data.data.amountPayable);
                $("#_amountRec").text(data.data.amountReceivable);
                $("#_profitOrLossY1").text(data.data.year1 + "-йил:" + data.data.profitOrLossOfYear1);
                $("#_profitOrLossY2").text(data.data.year2 + "-йил:" + data.data.profitOrLossOfYear2);
                $("#_profitOrLossY3").text(data.data.year3 + "-йил:" + data.data.profitOrLossOfYear3);
                $("#_comment").text(data.data.comments);

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
    $("#file1").attr("href", fdata.regCertificateLink);
    $("#file2").attr("href", fdata.orgCharterLink);
    $("#file3").attr("href", fdata.balanceSheetLink);
    $("#file4").attr("href", fdata.financialResultLink);
    $("#file5").attr("href", fdata.auditConclusionLink);
    $('#filesModal').modal('show');

}

function GetShareHolders(id) {
    
    $.ajax({
        type: "POST",
        url: "/Admin/Shares/GetShareHolders/",
        data: JSON.stringify(id),
        contentType: "application/json",

        success: function (data) {
            if (data.success) {
                $('#shList').empty();
                for (var i = 0; i < data.data.length; i++) {

                    var ul = document.createElement('ul');
                    var span = document.createElement('span');
                    span.classList.add('badge', 'rounded-pill','badge-info');
                    span.innerHTML = i + 1;
                    var li1 = document.createElement('li');
                    var a1 = document.createElement('a');
                    a1.innerHTML = "Акциядор/иштирокчи";                    
                    var p1 = document.createElement('p');
                    p1.innerHTML = data.data[i].shareholderName;
                    li1.appendChild(span);
                    li1.appendChild(a1);
                    li1.appendChild(p1);                  

                    var li2 = document.createElement('li');
                    var a2 = document.createElement('a');
                    var p2 = document.createElement('p');
                    a2.innerHTML = "Устав капиталининг миқдори";
                    p2.innerHTML = data.data[i].shareAmount;
                    li2.appendChild(a2);
                    li2.appendChild(p2);

                    var li3 = document.createElement('li');
                    var a3 = document.createElement('a');
                    var p3 = document.createElement('p');
                    a3.innerHTML = "Устав капиталининг улуши, %";
                    p3.innerHTML = data.data[i].sharePercentage;
                    li3.appendChild(a3);
                    li3.appendChild(p3);

                    ul.appendChild(li1);
                    ul.appendChild(li2);
                    ul.appendChild(li3);

                    var hr = document.createElement('hr');
                    $('#shList').append(ul);
                    $('#shList').append(hr);
                }

                $('#shareholderModal').modal('show');
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
                        dataTable.ajax.reload();
                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });

}

