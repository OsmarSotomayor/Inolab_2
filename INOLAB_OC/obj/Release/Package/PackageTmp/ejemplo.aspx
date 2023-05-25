<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ejemplo.aspx.cs" Inherits="INOLAB_OC.ejemplo" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div>
    
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" ProcessingMode="Remote" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Height="425px" ShowPageNavigationControls="False" ShowPrintButton="False" ShowToolBar="False" Width="1687px">
            <ServerReport ReportPath="/Fichas_Tecnicas/Prueba" ReportServerUrl="http://INOLABSERVER01/Reportes_Inolab" />
        </rsweb:ReportViewer>
    
    </div>
    </form>
</body>
</html>
