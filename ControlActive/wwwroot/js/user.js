var dataTable;

$(document).ready(function () {
    loadDataTable();
    $('[data-bs-toggle="tooltip"]').tooltip();
});


function loadDataTable() {
    dataTable = $('#data-table-basic').DataTable({
        drawCallback: function (settings, json) {

            $('[data-bs-toggle="tooltip"]').tooltip('update');
            //$("#list-of-product tbody tr > td").tooltip('hide');
        },

        "ajax": {
            "url": "/Admin/Users/GetAll"
        },
        "columns": [
            { "data": "firstName", "width": "15%" },
            { "data": "lastName", "width": "15%" },
            {"data": "organization.organizationName", "width":"15%"},
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "role", "width": "15%" },


            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //user is currently locked
                        //<div class="text-center ">
                        //    <a onclick=Edit({data.id}') class="btn mb-2 mr-2 btn-transition btn btn-outline-warning" style="cursor:pointer; width:100px">
                        //            <i class="fas fa-pencil-alt"></i>  
                        //        </a>

                        //   </div >
                        return `

                           
                             <div class="text-center">
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="блокдан очиш" onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px">
                                    <i class="fas fa-lock-open"></i>  
                                </a>
                                
                             </div>
                      
                           `;
                    }
                    else if (data.role == "Admin") {
                        return `
                            
                            <div class="text-center">
                                <a type="button" class=" disabled btn btn-outline-dark" style="cursor:pointer; width:100px" >
                                    <i class="fa fa-check"></i> 
                                </a>
                                
                            </div>
                            
                           `;
                    }
                    else {
                        //user is currently unlocked
                        //<div class="text-center">
                        //    <a onclick=Edit({data.id}') class="btn mb-2 mr-2 btn-transition btn btn-outline-warning" style="cursor:pointer; width:100px">
                        //            <i class="fas fa-pencil-alt"></i>  
                        //        </a>

                        //    </div >
                        return `
                            
                            <div class="text-center">
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="блоклаш" onclick=LockUnlock('${data.id}') class="btn-transition btn btn-outline-success" style="cursor:pointer; width:100px">
                                    <i class="fas fa-lock"></i> 
                                </a>
                                
                            </div>
                            
                           `;
                    }
                }, "width": "25%"
            },


            {
                "data": {
                    id: "id"
                },
                "render": function (data) {

                    return `
                  
                              <div class="text-center">
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="паролни алмаштириш" onclick=ResetPassword('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px">
                                    <i class="fas fa-retweet"></i>  
                                </a>
                                <a data-bs-toggle="tooltip" data-bs-placement="top" title data-bs-original-title="ўчириш" onclick=Delete('${data.id}') style="cursor:pointer;">
                                    DELETE <i class="fa fa-trash-o"></i> 
                                </a>
                                
                             </div>
                           `;

                }, "width": "25%"
            }
        ],

        "language": {
            "lengthMenu": "Кўрсат _MENU_ ёзув ҳар саҳифада",
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
        "lengthMenu": [10, 20, 30, 40, 50]
    });
}

function LockUnlock(id) {

    $.ajax({
        type: "POST",
        url: '/Admin/Users/LockUnlock/',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    });


}
function ResetPassword(id) {

    $.ajax({
        type: "POST",
        url: '/Admin/Users/ResetPassword/',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    });


}

function Edit(id) {

    $.ajax({
        type: "POST",
        url: '/Admin/Users/Edit/',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    });


}

function Delete(id) {
    Swal.fire({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Тасдиқлайман!',
        cancelButtonText: 'Бекор қилиш',
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: '/Admin/Users/Delete/',
                data: JSON.stringify(id),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            icon: 'success',
                            title: data.message
                        });
                        dataTable.ajax.reload();
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: data.message
                        });
                    }
                }
            });
        }
    });
}

