"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable the send button until connection is established.
//document.getElementById("askEdit").disabled = true;

connection.on("ReceiveMessage", function (notification) {

    var notifications = [notification];
    var isNew = true;
    setNoti(notifications, isNew);

    toastr.options = {

        "progressBar": true,

        "timeOut": "6000",
        "positionClass": "toast-bottom-right"

    }
    toastr.info("Сизга янги хабар келди!");

    if (typeof dataTable1 !== 'undefined')
        dataTable1.ajax.reload(function () {
            var rows = dataTable1.rows().count();
            if (rows == 0) {
                if (("#check") != null)
                    $("#check").hide();
            }
            else if (("#check") != null) {
                $("#check").show();
                $("#check").prop('checked', false);
            }
        });
    if (typeof dataTable2 !== 'undefined')
        dataTable2.ajax.reload();

    if (typeof dataTable1 !== 'undefined')
        dataTable2.ajax.reload();

});

connection.start().then(function () {
    //document.getElementById("askEdit").disabled = false;
}).catch(function (err) {
    return console.log(err.toString());
});

var baseUrl = "";
var role = "";
$(document).ready(function (e) {

    var url = "";    
    role = document.getElementById("role").innerText;

    if (role == "admin") {
        url = "/Admin/Notis/GetNotifications/";
        baseUrl = "/Admin/Notis/";
    }
    if (role == "simpleUser") {
        url = "/SimpleUser/Notis/GetNotifications/";
        baseUrl = "/SimpleUser/Notis/";
    }

    getUnRead(url);

});

function getUnRead(url) {
    var userId = document.getElementById("userId").innerText;
    var data = {
        isRead: false,
        userId: userId,
        isAll: false
    };
    var isNew = false;
        if (url != "") {
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (data) {
                if (data.success) {
                   
                        setNoti(data.data, isNew);
                }
                else {
                    toastr.options = {

                        "progressBar": true,

                        "timeOut": "6000",
                        "positionClass": "toast-bottom-right"

                    }
                    toastr.error(data.message);
                }
            }
        });
    }
}

function setNoti(notifications, isNew) {
   
    var notis = document.getElementById("notis");
    if (!isNew) {
        notis.innerHTML ="";
    }

    else {
        var noMsg = document.getElementById("noMsg");
        if (noMsg != null) {
            noMsg.remove();
        }
    }

    $("#msgs1").attr("href", baseUrl);
    
    //We can assign user-supplied strings to an element's textContent because it
    //is not interpreted as markup. If you're assigning in any other way, you
    //should be aware of possible script injection concerns.
    var li;  var div1; var div2; var img; var div3;
    var h5; var a1; var p1; var span; var time;
    var div4; var a2; var i; var checkBtn;
    var a2; var count; var notiDropdown;
    var hiddenP1; var hiddenP2; var hiddenP3;
    

    if (notifications.length == 0) {
        li = document.createElement("li");
        li.id = "noMsg";
        div1 = document.createElement("div");
        div1.classList.add("media");
        div2 = document.createElement("div");
        div2.classList.add("media-body");
        p1 = document.createElement("p");
        p1.textContent = "Янги хабарлар йўқ...";
        div2.appendChild(p1);
        div1.appendChild(div2);
        li.appendChild(div1);
        notis.appendChild(li);
    }

    for (const noti of notifications) {
        li = document.createElement("li");
        div1 = document.createElement("div");
        div1.classList.add("media");

        div2 = document.createElement("div");
        div2.classList.add("notification-img", "bg-light-primary");
        img = document.createElement('img');
        img.src = "/assets/images/avtar/man.png";
        div2.appendChild(img);
        div1.appendChild(div2);

        div3 = document.createElement("div");
        div3.classList.add("media-body");
        h5 = document.createElement("h5");
        a1 = document.createElement("a");
        a1.classList.add("f-14", "m-0");
        if (role == "admin")
             a1.href = "https://"+window.location.host + "/Admin/Users/";
        a1.textContent = `${ noti.fromUserName }`;
        h5.appendChild(a1);
        var p1 = document.createElement("p");       
        //p1.textContent = `${noti.message}`;
        var a2 = document.createElement('a');
        //a2.href = baseUrl + "Details/" + `${noti.notiId}`;
        a2.textContent = `${noti.message}`;
        a2.style.fontSize = "13px";
        a2.classList.add("toDetails");

        hiddenP1 = document.createElement('p');
        hiddenP1.classList.add("notiId");
        hiddenP1.innerText = `${noti.notiId}`;
        hiddenP1.style.display = 'none';

        hiddenP2 = document.createElement('p');
        hiddenP2.classList.add("objectType");
        hiddenP2.innerText = `${noti.objectType}`;
        hiddenP2.style.display = 'none';

        hiddenP3 = document.createElement('p');
        hiddenP3.classList.add("objectId");
        hiddenP3.innerText = `${noti.objectId}`;
        hiddenP3.style.display = 'none';

        p1.appendChild(a2);
        p1.addEventListener('mouseover', function () {
            this.firstChild.style.color = "#6362e7";
        });
        p1.addEventListener('mouseout', function () {
            this.firstChild.style.color = "black";
        }); 
        
        var span = document.createElement("span");
        var time = new Date(`${noti.createdDate}`);
        var date = time.toLocaleDateString();
        var today = new Date();
        today = today.toLocaleDateString();
        if (date == today)
            span.textContent = time.toLocaleTimeString('it-IT').replace(/(.*)\D\d+/, '$1');
        else
            span.textContent = date;

        div3.appendChild(h5);
        div3.appendChild(p1);
        div3.appendChild(hiddenP1);
        div3.appendChild(hiddenP2);
        div3.appendChild(hiddenP3);
        div3.appendChild(span);

        div1.appendChild(div3);

        var div4 = document.createElement("div");
        div4.classList.add("notification-right");

        var a3 = document.createElement("a");
        a3.href = "#";
        var i = document.createElement("i");
        div4.appendChild(a3);
        div4.appendChild(i);

        div1.appendChild(div4);

        li.appendChild(div1);

        
        if (isNew)
            notis.insertBefore(li, notis.firstChild);
        else {
            notis.appendChild(li);   
        }
       
    }

    if (notifications.length != 0) {
            var checkBtn = document.getElementById("checkBtn");
            if (checkBtn != null) {
                document.getElementById("notis").removeChild(checkBtn);
            }
            var liBtn = document.createElement("li");
            liBtn.id = "checkBtn";
            liBtn.classList.add("p-0");
            var a3 = document.createElement("a");
            a3.classList.add("btn", "btn-primary");
            a3.textContent = "Барчасини кўриш";
            a3.href = baseUrl;
            liBtn.appendChild(a3);
            notis.appendChild(liBtn);
        }
    

    count = $('#notis').children().length;
    let height = li.offsetHeight;
    var notiDropdown = document.getElementById("notiBody");
    if (count < 5 && count>1) {
        notiDropdown.style.height = (count) * height + "px";
    }
    else if (count == 1) {
        notiDropdown.style.height = "130px";
    }
    else {
        notiDropdown.style.height = "416px";
    }

    document.getElementById("counterMsg").innerText = count>=2?count-1:"";

}

$(document).on('click','.toDetails', function (evt) {
    var div = this.closest('div');
    var notiId = parseInt(div.querySelector('.notiId').innerHTML);
    var objectType = parseInt(div.querySelector('.objectType').innerHTML);
    var objectId = parseInt(div.querySelector('.objectId').innerHTML);
            $.ajax({
                type: "POST",
                url: baseUrl + "MakeRead/",
                data: JSON.stringify(notiId),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {

                        getUnRead(baseUrl + "GetNotifications/");

                    }
                    else {
                        toastr.error(data.message);
                    }
                }

            });
        

        window.location.href = baseUrl + "Details/?id=" + notiId + "&type=" + objectType + "&objectId=" + objectId;
    

});