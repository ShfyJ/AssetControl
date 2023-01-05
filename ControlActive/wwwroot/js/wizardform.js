var currentTab = 0;
var flag = true;

$(document).ready(function () {
    var current_fs, next_fs, previous_fs; //fieldsets
    var opacity;

    $(".next").click(function () {

        current_fs = $(this).parent();
        next_fs = $(this).parent().next();

        if (!flag) {
            
            return false;
        }
           

        //Add Class Active
        $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

        //show the next fieldset
        next_fs.show();
        currentTab++;
        //hide the current fieldset with style
        current_fs.animate({ opacity: 0 }, {
            step: function (now) {
                // for making fielset appear animation
                opacity = 1 - now;

                current_fs.css({
                    'display': 'none',
                    'position': 'relative'
                });
                next_fs.css({ 'opacity': opacity });
            },
            duration: 600
        });
    });

    $(".previous").click(function () {

        current_fs = $(this).parent();
        previous_fs = $(this).parent().prev();

        //Remove class active
        $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

        //show the previous fieldset
        previous_fs.show();
        currentTab--;
        //hide the current fieldset with style
        current_fs.animate({ opacity: 0 }, {
            step: function (now) {
                // for making fielset appear animation
                opacity = 1 - now;

                current_fs.css({
                    'display': 'none',
                    'position': 'relative'
                });
                previous_fs.css({ 'opacity': opacity });
            },
            duration: 600
        });
    });

    


    $('.radio-group .radio').click(function () {
        $(this).parent().find('.radio').removeClass('selected');
        $(this).addClass('selected');
    });

    $(".submit").click(function () {
        return false;
    })

});

//function validateForm() {
//    var x, y, i, valid = true;
//    x = document.getElementsByClassName("form-card");
//    y = x[currentTab].getElementsByTagName("input");
//    for (i = 0; i < y.length; i++) {
//        if (y[i].value == "") {
//            y[i].className += " invalid";
//            valid = false;
//        }
//    }
//    //if (valid) {
//    //    document.getElementsByClassName("step")[currentTab].className += " finish";
//    //}
    
//    return valid;
//}

function validate(val) {

    v1 = document.getElementsByClassName("step1");
    v2 = document.getElementsByClassName("step2");
    v3 = document.getElementsByClassName("step3");
    v4 = document.getElementsByClassName("step4");
    v5 = document.getElementsByClassName("step5");
    v6 = document.getElementsByClassName("step6");
    v7 = document.getElementsByClassName("step7");

    


    flag = true;

    if (val == 1) {
        flag = true;
        for (var i = 0; i < v1.length; i++) {
            if (v1[i].value == "") {
                v1[i].style.borderColor = "red";
                flag = false;
            }

            else if (v1[i].id == "share") {
                if (!isNaN(v1[i].value)) {
                    if (v1[i].value < 0 || v1[i].value > 100) {

                        v1[i].style.borderColor = "red";

                        flag = false;

                    }

                    else {
                        v1[i].style.borderColor = "green";

                    }
                }

                else {
                    flag = false;
                    v1[i].style.borderColor = "red";
                    $('#errorMsg').show();
                }



            }

            else {
                v1[i].style.borderColor = "green";
                if (flag == false)
                    continue;
                flag = true;
            }
        }
    }

    if (val == 2) {
        flag = true;
        for (var i = 0; i < v2.length; i++) {
            if (v2[i].value == "") {
                v2[i].style.borderColor = "red";

                flag = false;
            }
            else if (v2[i].id == "share") 
            {
                if (!isNaN(v2[i].value)) {
                    if (v2[i].value < 0 || v2[i].value > 100) {

                        v2[i].style.borderColor = "red";

                        flag = false;

                    }

                    else {
                        v2[i].style.borderColor = "green";

                    }
                }

                else {
                    flag = false;
                    v2[i].style.borderColor = "red";
                    $('#errorMsg').show();
                }

                
                
            }
            else {
                v2[i].style.borderColor = "green";
                if (flag == false)
                    continue;
                flag = true;
            }
        }
    }

    if (val == 3) {
        flag = true;
        for (var i = 0; i < v3.length; i++) {
            if (v3[i].value == "" && v3[i].id !="keep-order2") {
                v3[i].style.borderColor = "red";
                
                flag = false;
            }

            else if (v3[i].id == "farea") {

                if (!validPositive(v3[i].value)) {
                    $('#errorArea1').show();
                    v3[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorArea1').hide();
                    v3[i].style.borderColor = "green";
                }
            }
            else if (v3[i].id == "barea") {
                
                if (!validPositive(v3[i].value)) {
                    $('#errorArea2').show();
                    v3[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorArea2').hide();
                    v3[i].style.borderColor = "green";
                }
               
            }
            else if(v3[i].id == "keep-order2"){

                if ($('#keep-order2').val() === null) {
                    flag = false;
                    $("#multi1").css({ border: "1px solid red" });
                }
                else {
                    $("#multi1").css({ border: "1px solid green" });
                   
                }
            }
            

            else  {
                v3[i].style.borderColor = "green";
                if (flag == false)
                    continue;
                flag = true;
            }
           
           
        }
        
    }

    if (val == 4) {
        flag = true;
        for (var i = 0; i < v4.length; i++) {
            if (v4[i].value == "" && v4[i].id != "keep-order") {
                v4[i].style.borderColor = "red";

                flag = false;
            }

            else if (v4[i].id == "capital") {
                
                if (!validPositive(v4[i].value)) {
                    $('#errorCapital').show();
                    v4[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorCapital').hide();
                    v4[i].style.borderColor = "green";
                }

            }

            else if (v4[i].id == "amountfromcap") {
                
                if (!validPositive(v4[i].value)) {
                    $('#erroramountfromcap').show();
                    v4[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#erroramountfromcap').hide();
                    v4[i].style.borderColor = "green";
                }

            }

            else if (v4[i].id == "parvalue") {
               
                if (!validPositive(v4[i].value)) {
                    $('#errorparvalue').show();
                    v4[i].style.borderColor = "red";
                    flag =false;
                }
                else {
                    $('#errorparvalue').hide();
                    v4[i].style.borderColor = "green";
                }

            }
            else if (v4[i].id == "keep-order") {
                if ($('#keep-order').val() == "") {
                    flag = false;
                    $("#multi2").css({ border: "1px solid red" });
                }
                else {
                    $("#multi2").css({ border: "1px solid green" });
                    
                }
            }

            else {
                v4[i].style.borderColor = "green";
                if (flag == false)
                    continue;
                flag = true;
            }
            
        }
    }

    if (val == 5) {
        flag = true;
        for (var i = 0; i < v5.length; i++) {
            if (v5[i].value == "") {
                v5[i].style.borderColor = "red";

                flag = false;
            }

            else if (v5[i].id == "profit1") {
                
                if (!validNumber(v5[i].value)) {
                    $('#errorPr1').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorPr1').hide();
                    v5[i].style.borderColor = "green";
                }
            }

            else if (v5[i].id == "profit2") {
                
                if (!validNumber(v5[i].value)) {
                    $('#errorPr2').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorPr2').hide();
                    v5[i].style.borderColor = "green";
                }
            }

            else if (v5[i].id == "profit3") {
                
                if (!validNumber(v5[i].value)) {
                    $('#errorPr3').show();
                    v5[i].style.borderColor = "red";
                    flag =false;
                }
                else {
                    $('#errorPr3').hide();
                    v5[i].style.borderColor = "green";
                }
            }

            else if (v5[i].id == "tax") {
               
                if (!validPositive(v5[i].value)) {
                    $('#errortax').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errortax').hide();
                    v5[i].style.borderColor = "green";
                }
            }

            else if (v5[i].id == "wage") {
           
                if (!validPositive(v5[i].value)) {
                    $('#errorwage').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorwage').hide();
                    v5[i].style.borderColor = "green";
                }
                
            }

            else if (v5[i].id == "otherexp") {
               
                if (!validPositive(v5[i].value)) {
                    $('#errorother').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorother').hide();
                    v5[i].style.borderColor = "green";
                }
                
            }

            else if (v5[i].id == "salary") {
                
                if (!validPositive(v5[i].value)) {
                    $('#errorsalary').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorsalary').hide();
                    v5[i].style.borderColor = "green";
                }
            }

            else if (v5[i].id == "costForYear") {

                if (!validPositive(v5[i].value)) {
                    $('#errormCost').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errormCost').hide();
                    v5[i].style.borderColor = "green";
                }
            }

            else if (v5[i].id == "prodarea") {
               
                if (!validPositive(v5[i].value)) {
                    $('#errorprodarea').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorprodarea').hide();
                    v5[i].style.borderColor = "green";
                }
            }

            else if (v5[i].id == "buildarea") {
              
                if (!validPositive(v5[i].value)) {
                    $('#errorbuildarea').show();
                    v5[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorbuildarea').hide();
                    v5[i].style.borderColor = "green";
                }
            }


            else {
                v5[i].style.borderColor = "green";
                if (flag == false)
                    continue;
                flag = true;
            }
        }
    }

    if (val == 6) {
        flag = true;
        for (var i = 0; i < v6.length; i++) {
            if (v6[i].value == "") {
                v6[i].style.borderColor = "red";

                flag = false;
            }

            else if (v6[i].id == "cost") {
                
                if (!validPositive(v6[i].value)) {
                    $('#errorcost').show();
                    v6[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorcost').hide();
                    v6[i].style.borderColor = "green";
                }
                    
            }

            else if (v6[i].id == "wear") {
               
                if (!validPositive(v6[i].value)) {
                    $('#errorwear').show();
                    v6[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorwear').hide();
                    v6[i].style.borderColor = "green";
                }
            
            }

            else if (v6[i].id == "resvalue") {
                
                if (!validPositive(v6[i].value)) {
                    $('#errorvalue').show();
                    v6[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorvalue').hide();
                    v6[i].style.borderColor = "green";
                }
                   
            }

            else if (v6[i].id == "profit_1") {
               
                if (!validNumber(v6[i].value)) {
                    $('#errorPr_1').show();
                    v6[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorPr_1').hide();
                    v6[i].style.borderColor = "green";
                }
            }

            else if (v6[i].id == "profit_2") {
                
                if (!validNumber(v6[i].value)) {
                    $('#errorPr_2').show();
                    v6[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorPr_2').hide();
                    v6[i].style.borderColor = "green";
                }
            }

            else if (v6[i].id == "profit_3") {
               
                if (!validNumber(v6[i].value)) {
                    $('#errorPr_3').show();
                    v6[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorPr_3').hide();
                    v6[i].style.borderColor = "green";
                }
            }

            else if (v6[i].id == "payable") {
                
                if (!validPositive(v6[i].value)) {
                    $('#errorpayable').show();
                    v6[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorpayable').hide();
                    v6[i].style.borderColor = "green";
                }

            }

            else if (v6[i].id == "receivable") {
                
                if (!validPositive(v6[i].value)) {
                    $('#errorreceive').show();
                    v6[i].style.borderColor = "red";
                    flag = false;
                }
                else {
                    $('#errorreceive').hide();
                    v6[i].style.borderColor = "green";
                }

            }

            else {
                v6[i].style.borderColor = "green";
                if (flag == false)
                    continue;
                flag = true;
            }
        }
    }

    if (val == 7) {
        flag = true;
        for (var i = 0; i < v7.length; i++) {
            if (v7[i].value == "") {
                v7[i].style.borderColor = "red";

                flag = false;
            }
            else {
                v7[i].style.borderColor = "green";
                if (flag == false)
                    continue;
                flag = true;
            }
        }
    }

    return flag;
}

var value;

function validNumber(number) {
    
    if (!isNaN(number)) {

        value = true;

    }
    else
        value = false;

    return value;
}

function validPositive(number) {

    if (!isNaN(number) && number >= 0) {

        value = true;

    }
    else
        value = false;

    return value;
}