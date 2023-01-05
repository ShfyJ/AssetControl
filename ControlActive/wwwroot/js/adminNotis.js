var dataTable1;
var dataTable2;
var rows;

$(document).ready(function () {

    loadDataTable();

    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust().responsive.recalc();
    });

    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
    $('[data-bs-toggle="tooltip"]').tooltip();

    
});


function loadDataTable() {
    var userId = document.getElementById("userId").innerText;
    var data = {
        isRead: false,
        userId: userId,
        isAll: true
    };

   dataTable1 = $('#myTable').DataTable({
        

        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
           rows = $('#myTable').DataTable().rows().count();
           checkRowCount(rows);
        },

        "dom": '<lf<rt>ip>',
        scrollX: false,

        "ajax": {
            "url": '/Admin/Notis/GetNotifications/',
            "type": "POST",

            "data": function () {
                return JSON.stringify(data);
            },
            "contentType": "application/json",

       },
       autoWidth: false,
       columnDefs: [
           { width: '5%', targets: 0, orderable: false }, //step 2, column 1 out of 4
           { width: '15%', targets: 1, orderable: false }, 
           { width: '70%', targets: 2, orderable: false }, //step 2, column 2 out of 4
           { width: '10%', targets: 3, className: "dt-body-right", orderable: false }, //step 2, column 3 out of 4
           
       ],

       "order": [],

       "createdRow": function (row, data, dataIndex) {
           if (!data.isRead) {
               $(row).addClass('unRead');
           }
           else {
               $(row).addClass('read');
           }
       },

      /* "aoColumns": [{ "sWidth": "60%" }, { "sWidth": "20%" }, { "sWidth": "20%" }],*/
       "columns": [
           {
               "render": function () {
                   return `<input onchange="ToggleCheckbox(this)" style="height:12px;width:12px;border-color: black;cursor:pointer" class="form-check-input checker"  name="notBalance_block2" type="checkbox" >`;
               }
           },
            { "data": "fromUserName"},
            { "data": "message"},

            {
                "data": {},
                "render": function (data) {
                    
                    //var fdataStr = encodeURIComponent(JSON.stringify(fdata));
                    if (data.isRead) {
                        return `
                            <div>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" onclick=Delete("/Admin/Notis/Delete/${data.notiId}") class="" style="cursor:pointer; width:120px">
                                    <i style="color:black" class="fas fa-trash-alt"></i>  &nbsp &nbsp
                                </a>
                                
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўқилган қилиш" class="readBtn" style="cursor:pointer; width:120px" hidden>
                                    <i style="color:black" class="fa fa-envelope-o"></i>  
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўқилмаган қилиш" class="unReadBtn" style="cursor:pointer; width:120px">
                                    <i style="color:black" class="fa fa-envelope"></i>  &nbsp
                                </a>
                            </div>
                            <p>${data.createdTimeStr}</p>
                        `
                    }

                    else {
                        return `
                            <div>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" onclick=Delete("/Admin/Notis/Delete/${data.notiId}") class="" style="cursor:pointer; width:120px">
                                    <i style="color:black" class="fas fa-trash-alt"></i>  &nbsp &nbsp
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўқилган қилиш" class="readBtn" style="cursor:pointer; width:120px">
                                    <i style="color:black" class="fa fa-envelope-o"></i>  
                                </a>

                            <a  data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўқилмаган қилиш" class="unReadBtn" style="cursor:pointer; width:120px" hidden>
                                    <i style="color:black" class="fa fa-envelope"></i>  
                                </a>&nbsp

                            </div>
                            <p>${data.createdTimeStr}</p>
                        `
                    }
                        

                }
            },

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

function checkRowCount(rows) {
    
    if (rows == 0) {
        $("#check").hide();
    }
    else {
        $("#check").show();
    }
}

$('#myTable').on('click', 'tr td:not(:last-child)', function (evt) {
    var $cell = $(evt.target).closest('td');
    if ($cell.index() > 0) {
        var data = dataTable1.row(this).data();
               
        if (!data.isRead) {
            $.ajax({
                type: "POST",
                url: "/Admin/Notis/MakeRead/",
                data: JSON.stringify(data.notiId),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {

                      getUnRead("/Admin/Notis/GetNotifications/");

                    }
                    else {
                        toastr.error(data.message);
                    }
                }

            });
        }

        window.location.href = "/Admin/Notis/Details/?id=" + data.notiId +"&type="+data.objectType+"&objectId="+data.objectId;
    }
    
});

$('#checkHeader').on('click', function (evt) {
    var isChecked = document.getElementById("check").checked;
    
    if (isChecked) {

        $('.checker').prop('checked', true);
        $("#headerDel").css({ 'display': 'inline' });
        $("#headerRead").css({ 'display': 'inline' });
        $("#headerUnRead").css({ 'display': 'inline' });
    }

    else {
        $('.checker').prop('checked', false);
        $("#headerDel").hide();
        $("#headerRead").hide();
        $("#headerUnRead").hide();
    }
        
});

function ToggleCheckbox(element) {
    var checkboxes = document.getElementsByClassName('checker');
    
    var isCheckedAny = false;
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {
            isCheckedAny = true;
            break;
        }
    }
    if (element.checked) {

        $("#headerDel").css({ 'display': 'inline' });
        $("#headerRead").css({ 'display': 'inline' });
        $("#headerUnRead").css({ 'display': 'inline' });
    }

    else if (!isCheckedAny) {
        $("#headerDel").hide();
        $("#headerRead").hide();
        $("#headerUnRead").hide();
    }
}

function Delete(url) {

    Swal.fire({
        title: "Ўчиришни тасдиқлайсизми?",
        text: "Ўчирилгач хабарни қайта тиклай олмайсиз!",
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
                        Swal.fire({
                            title: "Ўчирилди!",
                            icon: "success",
                        });
                        /* toastr.success(data.message);*/
                        dataTable1.ajax.reload(function () {
                            var rows = dataTable1.rows().count();
                            if (rows == 0) {
                                $("#check").hide();
                                $('.checker').prop('checked', false);
                                $("#headerDel").hide();
                                $("#headerRead").hide();
                                $("#headerUnRead").hide();
                            }
                            else {
                                $("#check").show();
                                $('.checker').prop('checked', false);
                                $("#headerDel").hide();
                                $("#headerRead").hide();
                                $("#headerUnRead").hide();
                            }
                        });

                        getUnRead("/Admin/Notis/GetNotifications/");
                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });

}

function DeleteAll() {

    var checkboxes = document.getElementsByClassName('checker');
    var ids = [];
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].id == "check") {
            continue;
        }
            
        if (checkboxes[i].checked) {
            ids.push(dataTable1.row(checkboxes[i].parentNode.parentNode).data().notiId);
        }
    }

    Swal.fire({
        title: "Белгиланганларни ўчиришни тасдиқлайсизми?",
        text: "Ўчирилгач қайта тиклай олмайсиз!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (willDelete) {
        if (willDelete.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/Notis/DeleteAll/",
                data: JSON.stringify(ids),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: "Ўчирилди!",
                            icon: "success",
                        });
                        /* toastr.success(data.message);*/
                        dataTable1.ajax.reload(function () {
                            var rows = dataTable1.rows().count();
                            if (rows == 0) {
                                $("#check").hide();
                                $('.checker').prop('checked', false);
                                $("#headerDel").hide();
                                $("#headerRead").hide();
                                $("#headerUnRead").hide();
                            }
                            else {
                                $('.checker').prop('checked', false);
                                $("#headerDel").hide();
                                $("#headerRead").hide();
                                $("#headerUnRead").hide();
                            }
                        });

                        getUnRead("/Admin/Notis/GetNotifications/");
                    }
                    else {
                        Swal.fire(data.message);
                    }
                }
            });
        }
    });

}

$('#myTable').on('click','.readBtn', function (evt) {
    var data = dataTable1.row($(this).parents('tr')).data();
    var td = $(this).closest('td');
    var aUnRead = td.find('a.unReadBtn');
    var aRead = td.find('a.readBtn');
    
    var row = $(this).parents('tr');
    $.ajax({
        type: "POST",
        url: "/Admin/Notis/MakeRead/",
        data: JSON.stringify(data.notiId),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                
                row.removeClass('unRead');
                row.addClass('read');
                //aUnRead.css({ 'display': 'inline' });
                aUnRead.removeAttr('hidden');
                aRead.prop("hidden", !this.checked);
                //td.html('<a data-toggle="tooltip" data-placement="top" title data-original-title="ўқилган қилиш" class="unReadBtn" style="cursor:pointer; width:120px">' +
                //    '<i style = "color:black" class= "fa fa-envelope-o" ></i ></a >&nbsp');
                //dataTable1.ajax.reload();

                getUnRead("/Admin/Notis/GetNotifications/");
               
            }
            else {
                toastr.error(data.message);
            }
        }

    });
    
});

$('#myTable').on('click', '.readSelectedBtn', function (evt) {
    var isUnReadSelected = false;
    var checkboxes = document.getElementsByClassName('checker');
    var checkedboxes = [];
    var ids = [];
    var trs = [];

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].id == "check") {
            continue;
        }

        if (checkboxes[i].checked) {
            ids.push(dataTable1.row(checkboxes[i].parentNode.parentNode).data().notiId); 
            trs.push(checkboxes[i].parentNode.parentNode);
            checkedboxes.push(checkboxes[i]);
            if (checkboxes[i].parentNode.parentNode.classList.contains('unRead')) {
                isUnReadSelected = true;
            }
                
        }
    }

    if (isUnReadSelected) {
        $.ajax({
            type: "POST",
            url: "/Admin/Notis/MakeReadAll/",
            data: JSON.stringify(ids),
            contentType: "application/json",
            success: function (data) {
                if (data.success) {

                    for (var i = 0; i < trs.length; i++) {
                        trs[i].className = "read";
                        if (trs[i].children[3].children[0].children[2].hidden) {
                            trs[i].children[3].children[0].children[2].hidden = false;
                        }                       

                        if (!trs[i].children[3].children[0].children[1].hidden) {
                            trs[i].children[3].children[0].children[1].hidden = true;
                        }
                        
                       
                    }

                    getUnRead("/Admin/Notis/GetNotifications/");

                }
                else {
                    toastr.error(data.message);
                }
            }

        });
    }
    

});

$('#myTable').on('click', '.unReadBtn', function () {
    var data = dataTable1.row($(this).parents('tr')).data();
    var btn = $(this);
    var td = $(this).closest('td');
    var aUnRead = td.find('a.unReadBtn');
    var aRead = td.find('a.readBtn');
    var row = $(this).parents('tr');
    $.ajax({
        type: "POST",
        url: "/Admin/Notis/MakeUnRead/",
        data: JSON.stringify(data.notiId),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                dataTable1.row(btn.parents('tr')).data().isRead = false;
                row.removeClass('read');
                row.addClass('unRead');
                aUnRead.prop("hidden", !this.checked);
                //aRead.css({ 'display': 'inline' });
                aRead.removeAttr('hidden');
                //td.html('<a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўқилган қилиш" class="readBtn" style="cursor:pointer; width:120px">' +
                //    '<i style = "color:black" class= "fa fa-envelope" ></i ></a >&nbsp');
                //dataTable1.ajax.reload();

                getUnRead("/Admin/Notis/GetNotifications/");
               
            }
            else {
                toastr.error(data.message);
            }
        }

    });

});

$('#myTable').on('click', '.unReadSelectedBtn', function (evt) {
    var isReadSelected = false;
    var trs = [];
    var checkboxes = document.getElementsByClassName('checker');
    var checkedboxes = [];
    var ids = [];
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].id == "check") {
            continue;
        }

        if (checkboxes[i].checked) {
            ids.push(dataTable1.row(checkboxes[i].parentNode.parentNode).data().notiId);
            checkedboxes.push(checkboxes[i]);
            trs.push(checkboxes[i].parentNode.parentNode);
            
            if (checkboxes[i].parentNode.parentNode.classList.contains('read')) {
                isReadSelected = true;
            }
        }
    }

    if (isReadSelected) {
        $.ajax({
            type: "POST",
            url: "/Admin/Notis/MakeUnReadAll/",
            data: JSON.stringify(ids),
            contentType: "application/json",
            success: function (data) {
                if (data.success) {
                    for (var i = 0; i < trs.length; i++) {
                        
                        trs[i].className = "unRead";
                        dataTable1.row(trs[i]).data().isRead = false;
                        if (!trs[i].children[3].children[0].children[2].hidden) {
                            trs[i].children[3].children[0].children[2].hidden = true;
                        }
                        
                        if (trs[i].children[3].children[0].children[1].hidden) {
                            trs[i].children[3].children[0].children[1].hidden = false;
                        }
                        
                    }

                    getUnRead("/Admin/Notis/GetNotifications/");

                }
                else {
                    toastr.error(data.message);
                }
            }

        });
    }


});
