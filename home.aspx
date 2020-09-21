<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="SignalRInbox.home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../Scripts/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.signalR-2.4.1.min.js" type="text/javascript"></script>    
    <script type="text/javascript" src='<%= ResolveClientUrl("~/signalr/hubs") %>'></script>

    <title></title>
    <script type="text/javascript"> 
        $(function () {
            var hdnEmpNo = document.getElementById("<%= hdnEmpNo.ClientID %>");
            var myNotificationHub = $.connection.notificationsHub;
            $.connection.hub.qs = { 'empNo' : hdnEmpNo.value };

            $.connection.hub.start().done(function ()
            {
                console.log("Connected");
            });

            myNotificationHub.client.SendMessage = function (message) {
                console.log(message);
            }
        });
    </script>
</head>

<body>
    <form id="form1" runat="server">
        <h2>This is Home Page ..... </h2>
        <asp:HiddenField ID="hdnEmpNo" runat="server" />

        <table id="listofreq"></table>
        <asp:Button ID="Notify" runat="server" Text="Button" OnClick="Notify_Click" />
    </form>
</body>
</html>
