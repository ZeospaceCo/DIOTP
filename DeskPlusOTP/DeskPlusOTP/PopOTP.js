PopOTP = {}

$(function () {
    $(document).ready(function () {

        $("#btnOTP").click(function (e) {
            e.preventDefault();
            PopOTP.ConfirmOTP();
        });
    });
});


/**************************************************************************************************************/
PopOTP.ConfirmOTP = function () {

    var random = $("#tbRandom").val();

    if (random != "") {
        var url = "WsOTP.asmx/ConfirmGetOTP";
        var data = "{companyID : '" + companyID + "',loginID : '" + loginID + "',random : '" + random + "'}";
        var success = eval("PopOTP.SuccessOTP");
        AjaxService(url, data, success);
    }
    else {
        alert("OTP 6자리를 입력하세요");
    }
}

PopOTP.SuccessOTP = function (data, status) {

    if (data.d == "1") {
        alert("입력값이 일치합니다 \n로그인 처리중입니다");
        opener.RetunOTP(data.d);
        self.close();
    }
    else if (data.d == "3") {
        alert("OTP를 등록해주세요.");
    }
    else {
        alert("입력값이 일치하지 않습니다");
    }
}