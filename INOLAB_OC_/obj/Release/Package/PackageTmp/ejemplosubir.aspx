<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ejemplosubir.aspx.cs" Inherits="INOLAB_OC.ejemplosubir" %>

<!DOCTYPE html>

<%--<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>--%>



<%--<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link rel="Stylesheet" type="text/css" href="CSS/uploadify.css" />
     <script type="text/javascript" src="scripts/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="scripts/jquery.uploadify.js"></script>
</head>
<body>
<form id="form1" runat="server">
    <div style = "padding:40px">
        <asp:FileUpload ID="FileUpload1" runat="server" />
    </div>
</form>
</body>
</html>
<script type = "text/javascript">
$(window).load(
    function() {
    $("#<%=FileUpload1.ClientID %>").fileUpload({
        'uploader': 'scripts/uploader.swf',
        'cancelImg': 'images/cancel.png',
        'buttonText': 'Browse Files',
        'script': 'Upload.ashx',
        'folder': 'uploads',
        'fileDesc': 'Image Files',
        'fileExt': '*.jpg;*.jpeg;*.gif;*.png',
        'multi': true,
        'auto': true
    });
   }
);
</script> --%>





<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

 <link rel="Stylesheet" href="CSS/uploadify.css" />

<%--<link rel="Stylesheet" type="text/css" href="CSS/uploadify.css" />--%>
<%-- <link rel="stylesheet" href="CSS/EstiloVista.css" />--%>

 <script type="text/javascript" src="scripts/jquery-1.3.2.min.js"></script>
 <script type="text/javascript" src="scripts/jquery.uploadify.js"></script>
<script type = "text/javascript">
$(window).load(
    function() {
        $("#FileUpload1").fileUpload({
        'uploader': 'scripts/uploader.swf',
        'cancelImg': 'images/cancel.png',
        'buttonText': 'Browse Files',
        'script': 'Upload.ashx',
        'folder': 'uploads',
        'fileDesc': 'Image Files',
        'fileExt': '*.jpg;*.jpeg;*.gif;*.png',
        'multi': true,
        'auto': true
    });
   }
);
</script> 

</head>
<body>
        <form id="form1" runat="server">
            <a href="javascript:$('#<%=FileUpload1.ClientID%>').fileUploadStart()">Start Upload</a>&nbsp; 
           |&nbsp;<a href="javascript:$('#<%=FileUpload1.ClientID%>').fileUploadClearQueue()">Clear</a> 
            <div style = "padding:40px">
                <asp:FileUpload ID="FileUpload1" runat="server" />
            </div>
        </form>
</body>


</html>
