<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ejemplo1.aspx.cs" Inherits="INOLAB_OC.ejemplo1" %>

<!DOCTYPE html>

 <html>
<head>
    <meta name="viewport" content="width=device-width"/>
    <title>Upload file using jQuery AJAX in ASP.Net MVC
</title>
    <link rel="Stylesheet" type="text/css" href="../CSS/uploadify.css"/>
    <script type="text/javascript" src="../scripts/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery.uploadify.js"></script>
    <script type="text/javascript">
        $(function () {
            $("#FileUpload").fileUpload({
                'uploader': '../scripts/uploader.swf',
                'cancelImg': '../scripts/cancel.png',
                'buttonText': 'Browse Files',
                'script': '/employee/Index/',
                'folder': 'uploads',
                'fileDesc': 'Image Files',
                'fileExt': '*.jpg;*.jpeg;*.gif;*.png',
                'multi': true,
                'auto': true
            });
        });
    </script>
</head>
<body>
    <div>
        <input type="file" id="FileUpload" name="FileUpload" multiple="true"/>
    </div>
</body>
</html>