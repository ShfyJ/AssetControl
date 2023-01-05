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
            "url": "/SimpleUser/RealEstates/GetUnSent"
      },
      deferRender: true,
        "columns": [
            { "data": "realEstateName", "width": "15%" },
            { "data": "cadastreNumber", "width": "15%" },
            { "data": "cadasterRegDateStr", "width": "15%" },
            { "data": "commisioningDateStr", "width": "15%" },
            { "data": "activity", "width": "15%" },
            { "data": "regionOfObject.regionName", "width": "15%" },
            { "data": "districtOfObject.districtName", "width": "15%" },
            { "data": "address", "width": "15%" },
            { "data": "assetHolderName", "width": "15%" },
            { "data": "buildingArea", "width": "15%" },
            { "data": "fullArea", "width": "15%" },
            { "data": "infrastructureNames", "width": "15%" },
            { "data": "numberOfEmployee", "width": "15%" },

            { "data": "maintenanceCostForYear", "width": "15%" },
            { "data": "initialCostOfObject", "width": "15%" },
            { "data": "wear", "width": "15%" }, 
            { "data": "residualValueOfObject", "width": "15%" },
            { "data": "proposal.proposalName", "width": "15%" },
            { "data": "comment", "width": "15%" },
           
            {
                "data": {},

                "render": function (data) {
                    return `
                          
                          <a href="/SimpleUser/RealEstates/DownloadFile/${data.cadastreFileId}" style="cursor: pointer;">Кадастр файли <i class="fa fa-download"></i></a>         
                           
                       `
                }
            },

            {
                "data": {},

                "render": function (data) {
                    return `
                            <div class="row">
                               <div>
                                <img style="width:30%; height:70%; display: block;" src="${data.photoOfObjectLink1}" alt="расм" />
                                <a style="cursor: pointer;" href="/SimpleUser/RealEstates/DownloadFile/${data.photoOfObject1Id}" >1-расм  <i class="fa fa-download"></i></a>
                                </div>
                                <div>
                                 <img style="width:30%; height:70%; display: block;" src="${data.photoOfObjectLink2}" alt="расм" />
                                 <a style="cursor: pointer;" href="/SimpleUser/RealEstates/DownloadFile/${data.photoOfObject2Id}" >2-расм <i class="fa fa-download"></i></a>
                                </div>
                                <div>
                                 <img style="width:30%; height:70%; display: block;" src="${data.photoOfObjectLink3}" alt="расм" />
                                 <a style="cursor: pointer;" href="/SimpleUser/RealEstates/DownloadFile/${data.photoOfObject3Id}" >3-расм <i class="fa fa-download"></i></a>
                                </div>

                            </div >
                       `
                }
            },

            {
                "data": {
                    realEstateId: "realEstateId"
                },
                "render": function (data) {
                    return `
                            <div >
                                <a onclick=Edit('${data.realEstateId}') id="edit" class="btn btn-warning-gradien" style="cursor:pointer; width:150px"><i class="fas fa-edit"></i> Таҳрирлаш
                                </a>

                                <a onclick=Delete('/SimpleUser/RealEstates/Delete/${data.realEstateId}') id="delete" class="btn btn-danger-gradien" style="cursor:pointer; width:120px"><i class="fas fa-trash"></i> Ўчириш
                                </a>

                                <a onclick=Send("${data.realEstateId}") id="send" class="btn btn-success-gradien" style="cursor:pointer; width:150px"><i class="fas fa-paper-plane"></i> Тасдиқлаш
                                </a>

                            </div>
                        `

                }

            },

        ],
        "aoColumnDefs": [
          { "sWidth": "20%", "aTargets": [4] }
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
            "url": "/SimpleUser/RealEstates/GetSent"
      },
      deferRender: true,
        "columns": [
            { "data": "realEstateName", "width": "15%" },
            { "data": "cadastreNumber", "width": "15%" },
            { "data": "cadasterRegDateStr", "width": "15%" },
            { "data": "commisioningDateStr", "width": "15%" },
            { "data": "activity", "width": "15%" },
            { "data": "regionOfObject.regionName", "width": "15%" },
            { "data": "districtOfObject.districtName", "width": "15%" },
            { "data": "address", "width": "15%" },
            { "data": "assetHolderName", "width": "15%" },
            { "data": "buildingArea", "width": "15%" },
            { "data": "fullArea", "width": "15%" },
            { "data": "infrastructureNames", "width": "15%" },
            { "data": "numberOfEmployee", "width": "15%" },
            { "data": "maintenanceCostForYear", "width": "15%" },
            { "data": "initialCostOfObject", "width": "15%" },
            { "data": "wear", "width": "15%" },
            { "data": "residualValueOfObject", "width": "15%" },
            { "data": "proposal.proposalName", "width": "15%" },
            { "data": "comment", "width": "15%" },

            {
                "data": {},

                "render": function (data) {
                    return `
                          
                          <a href="/SimpleUser/RealEstates/DownloadFile/${data.cadastreFileId}" style="cursor: pointer;">Кадастр файли <i class="fa fa-download"></i></a>         
                           
                       `
                }
            },

            {
                "data": {},

                "render": function (data) {
                    return `
                            <div class="row">
                               <div>
                                <img style="width:30%; height:70%; display: block;" src="${data.photoOfObjectLink1}" alt="расм" />
                                <a style="cursor: pointer;" href="/SimpleUser/RealEstates/DownloadFile/${data.photoOfObject1Id}" >1-расм  <i class="fa fa-download"></i></a>
                                </div>
                                <div>
                                 <img style="width:30%; height:70%; display: block;" src="${data.photoOfObjectLink2}" alt="расм" />
                                 <a style="cursor: pointer;" href="/SimpleUser/RealEstates/DownloadFile/${data.photoOfObject2Id}" >2-расм <i class="fa fa-download"></i></a>
                                </div>
                                <div>
                                 <img style="width:30%; height:70%; display: block;" src="${data.photoOfObjectLink3}" alt="расм" />
                                 <a style="cursor: pointer;" href="/SimpleUser/RealEstates/DownloadFile/${data.photoOfObject3Id}" >3-расм <i class="fa fa-download"></i></a>
                                </div>

                            </div >
                       `
                }
            },

            {
                "data": {},
                "render": function (data) {
                    return `
                            <div >
                                <a onclick=AskToEdit('${data.realEstateId}') id="askEdit" class="btn btn-primary-gradien" style="cursor:pointer; width:300px"><i class="fa fa-exchange"></i> Таҳрирлаш учун рухсат олиш
                                </a>                              
                            </div>
                        `

                }

            },



      
      ],
      "autoWidth": false,
      "columnDefs": [
          { "width": "5%", "target": [0] }
      ],
      fixedColumns: true,
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

    window.location.href = "/SimpleUser/RealEstates/DownloadFile/" + id;


}
function Edit(id) {

    window.location.href = "/SimpleUser/RealEstates/Edit/" + id;

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
                url: "/SimpleUser/RealEstates/GetUsersIds/",
                success: function (data) {
                    if (!data.success) {
                        Swal.fire("Хатолик", data.message, {
                            icon: "error",
                        });
                       
                    }
                    else {

                       notification = {
                            MessageType : 1, //ma'lumotni o'zgartirishga dostup
                            ToUserId: data.data[1],
                            FromUserId: data.data[0],
                            ObjectId: parseInt(id),
                            ObjectType: 1
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
                                    toastr.success("Сервер билан алоқа ўрнатилди!","Сўровни қайтадан жўнатинг");
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
        title: "Ўчиришни тасдиқлайсизми?",
        text: "Ўчирилгач маълумотларни қайта тиклай олмайсиз! Бу объектга боғланган барча маълумотлар базадан ўчирилади!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then(function (willDelete)  {
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
            $.ajax({
                type: "POST",
                url: '/SimpleUser/RealEstates/Send/',
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

