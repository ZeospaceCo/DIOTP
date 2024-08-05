OTP = {}

$(function () {
    $(document).ready(function () {

        $("#btnOTP").click(function (e) {
            e.preventDefault();
            OTP.UpdOTP();
        });
    });
});


/**************************************************************************************************************/
OTP.UpdOTP = function () {

    var random = $("#tbRandom").val();

    if (loginID == "") {
        alert("아이디를 입력해주세요");
        return;
    }

    if (random != "") {
        var url = "WsOTP.asmx/UpdOTP";
        var data = "{companyID : '" + companyID + "',loginID : '" + loginID + "',random : '" + random + "',base32Secret : '" + base32Secret + "'}";
        var success = eval("OTP.SuccessOTP");
        AjaxService(url, data, success);
    }
    else {
        alert("OTP 6자리를 입력하세요");
    }    
}

OTP.SuccessOTP = function (data, status) {

    if (data.d == "1") {
        alert("등록이 완료되었습니다");
        self.close();
    }
    else {
        alert("입력값이 일치하지 않습니다");
    }
}