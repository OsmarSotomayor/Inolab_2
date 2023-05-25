<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubirArchivos.aspx.cs" Inherits="INOLAB_OC.SubirArchivos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>

<%--    <script src="uploadify-master/jquery.uploadifive.js"></script>
    <script src="uploadify-master/jquery.uploadifive.min.js"></script>
    <link href="uploadify-master/uploadifive.css" rel="stylesheet" />--%>

<%--    <script src="uploadify-master/Sample/jquery.min.js"></script>
    <script src="uploadify-master/Sample/jquery.uploadifive.js"></script>   
    <link href="uploadify-master/Sample/uploadifive.css" rel="stylesheet" />--%>


</head>
<body>
	<h1>Carga de Archivos</h1>
	<form>
		
        <div>
            <asp:FileUpload ID="FileUpload1" runat="server" />
        </div>
	</form>

<%--	<script type="text/javascript">
		<?php $timestamp = time();?>
		$(function() {
			$('#file_upload').uploadifive({
				'auto'             : false,
				'checkScript'      : 'check-exists.php',
				'fileType'         : '.jpg,.jpeg,.gif,.png',
				'formData'         : {
									   'timestamp' : '<?php echo $timestamp;?>',
									   'token'     : '<?php echo md5('unique_salt' . $timestamp);?>'
				                     },
				'queueID'          : 'queue',
				'uploadScript'     : 'uploadifive.php',
				'onUploadComplete' : function(file, data) { console.log(data); }
			});
		});
	</script>--%>
    
</body>
</html>
