<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PopOTP.aspx.cs" Inherits="DeskPlusOTP.PopOTP" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
	<meta name="robots" content="noindex">
    <title>OTP</title>   
    <link rel="stylesheet" type="text/css" href="/DeskPlusEIP/Design/C/Css/DeskPlusEIPBase.css"/><link rel="stylesheet" type="text/css" href="/DeskPlusEIP/Design/C1/Css/DeskPlusEIPBase.css"/>
    <script type="text/javascript" src="/DeskPlusEIP/Html/Js/jQuery/jquery.js"></script>
    <script type="text/javascript" src="/DeskPlusEIP/Html/Js/jQuery/jquery.common.js"></script>
    <script type="text/javascript" >
        var companyID = "<%=companyID%>";
        var loginID = "<%=loginID%>";

    </script>
    <script type="text/javascript" src="PopOTP.js?v=1"></script>
</head>
<body>  
    <form id="form1" runat="server">
    <div class="layer_popup">
    <div class="inside">
        <div class="popup_header">
            <h1>구글 OTP 인증</h1>            
        </div>
        <div class="dotLine"></div>
        <div class="popup_layout_style3" style="min-height:300px;height:295px;">
            <table class="table_typeB" border="0" style="width:98%;" > 
		    <caption></caption>
		    <colgroup>
                <col width="120px" />
                <col />
		    </colgroup>
		    <tbody>
            <tr>
                <th>OTP 6자리</th>
                <td><asp:TextBox ID="tbRandom" TabIndex="1" class="form-control" runat="server" placeholder="6자리 입력"></asp:TextBox></td>
            </tr>
            <tr>
                <th>OTP 확인</th>
                <td><input type="button" value="OTP확인" id="btnOTP" class="btn btn-lg btn-primary btn-block"  /></td>
             </tr>
            </tbody>
            </table>
        </div>
    </div>    
    </div>
    </form>
</body>
</html>

