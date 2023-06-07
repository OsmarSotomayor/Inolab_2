<%@ Page Language="C#" AutoEventWireup="true" Inherits="DetalleFSR" EnableEventValidation="false" Codebehind="DetalleFSR.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title></title>
    
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="CSS/EstiloVista.css" />
    <link rel="stylesheet" href="CSS/EncabezadoDetalleFSR.css" />
    <link rel="stylesheet" href="CSS/drop.css" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.12.1/themes/smoothness/jquery-ui.css" />

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.jquery.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.12.1/jquery-ui.min.js"></script>

    <script type="text/javascript">
        function go(clicked)
        {            
            if (clicked == "salir")
            {
                window.location.href = "./Sesion.aspx";
                return false;
            }   
            if (clicked == "atras") {
                window.location.href = "./FSR.aspx";
                return false;
            }
        }
    </script>
    
    <script>    
        $.datepicker.regional['es'] = {
            closeText: 'Cerrar',
            prevText: '< Ant',
            nextText: 'Sig >',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            weekHeader: 'Sm',
            dateFormat: 'yy-mm-dd',
            
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ''
        };

        $.datepicker.setDefaults($.datepicker.regional['es']);

        $(function () {
            $("#datepicker").datepicker();
        });

        $.datepicker.regional['es'] = {
            closeText: 'Cerrar',
            prevText: '< Ant',
            nextText: 'Sig >',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            weekHeader: 'Sm',
            dateFormat: 'yy-mm-dd',
            
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ''
        };

        $.datepicker.setDefaults($.datepicker.regional['es']);

        $(function () {
            $("#datepicker1").datepicker();
        });
    </script>

    <style type="text/css">
        .auto-style1 {
            width: 98%;
            max-width: 10000px;
            margin: auto;
            height: 69px;
        }
        .auto-style4 {
            text-decoration: underline;
           font-weight:bold;
        }
        .auto-style5 {
            width: 100%;
        }
        .auto-style6 {
            height: 48px;
        }
        .auto-style7 {
            margin-left: 0;
        }
        .auto-style8 {
            width: 222px;
        }
        </style>
         

</head>

<body style="overflow:auto;" onload="window.history.forward();">
    <form id="form1" runat="server" autocomplete="off">
        <header class="header2">
            <div id="headerone" class="auto-style1" runat="server">
            <div class="logo" style="height: 67px"><img src="Imagenes/LOGO_Blanco_Lineas.png" class="logo"/></div>
                <asp:Label ID="Label1" runat="server" Text="Usuario: " Font-Bold="True" ForeColor="White"  class="logo" ></asp:Label>
                <asp:Label ID="lbluser" runat="server" Text="usuario" Font-Bold="True" ForeColor="White" class="logo"></asp:Label>
                <nav>
                                       
                        <button type="reset" class="dropbtn" id="Btn_Atras" onclick="go('atras')">Atras</button>
                    

<%--                    <div class="dropdown">
                        <button class="dropbtn"  type="reset" href="#" >Nuevo</button>
                            <div class="dropdown-content">
                                <a href="#">Cliente/Empresa</a>
                                <a href="#">Ingeniero</a>
                                <a href="#">Marca</a>
                                <a href="#">Modelo</a>
                            </div>
                    </div>--%>
                    
                        
                        
                                 
                        <button type="reset" class="dropbtn" id="Btn_Salir" onclick="go('salir')">Salir</button>
                    
                </nav>                              
            </div>
        </header>
        
        <section class="contenido2">
            <div class="drop" runat="server" id="contenone" >
                <div class="form-style-2-heading">
                    <asp:Label ID="titulo" runat="server">Detalle de FSR</asp:Label>
                </div>
                <asp:GridView ID="GridView1" runat="server"  AutoGenerateColumns="False" Width="100%" Font-Size="9pt" DataKeyNames="idFSRAccion" CellPadding="4"  ForeColor="#333333" GridLines="None" BorderStyle="Ridge" Font-Bold="False" CssClass="auto-style7" OnRowCommand="GridView1_OnRowComand" >
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:BoundField HeaderText="id" DataField="idFSRAccion" SortExpression="idFSRAccion" Visible="true">
                        <ItemStyle Width="1%" ForeColor="#808080"></ItemStyle></asp:BoundField>

                        <asp:BoundField DataField="FechaAccion"  HeaderText="Fecha de la Acción" SortExpression="FechaAccion" DataFormatString="{0:d}">
                        <ItemStyle  HorizontalAlign="Center" Width="12%"></ItemStyle></asp:BoundField>

                        <asp:BoundField DataField="HorasAccion" HeaderText="Horas de la Acción" SortExpression="HorasAccion" Visible="True">
                        <ItemStyle HorizontalAlign="Center"  Width="12%"/></asp:BoundField>
                
                        <asp:BoundField DataField="AccionR"  HeaderText="Acción Realizada"  SortExpression="AccionR" >
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" /></asp:BoundField>

                        <asp:ButtonField HeaderText="Eliminar"  SortExpression="Eliminar" ButtonType="Link" CommandName="Borrar" Text="Eliminar" >
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%"/></asp:ButtonField>
                    </Columns>

                    <EditRowStyle BackColor="#7C6F57" HorizontalAlign="Center" VerticalAlign="Middle" />
                    <FooterStyle BackColor="#464a49" ForeColor="White" Font-Bold="False" />
                    <HeaderStyle BackColor="#000000"  Font-Bold="True" ForeColor="White"  />
                    <PagerSettings Position="TopAndBottom" />
                    <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#E3EAEB" Height="20px"/>
                    <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F8FAFA" />
                    <SortedAscendingHeaderStyle BackColor="#246B61" />
                    <SortedDescendingCellStyle BackColor="#D4DFE1" />
                    <SortedDescendingHeaderStyle BackColor="#15524A" />
                    </asp:GridView>
                
                <div id="botonnuevo" class="btnnuevo" >
                    <asp:Button runat="server" Text="Nuevo" BorderStyle="None" style="float:right" ID="Button4" OnClick="Nuevo_Click" />
                    <asp:Button class="dropbtn" runat="server" Text="Ir a Servicios Asignados" BorderStyle="None" style="float:right"  ID="Btn_Ir_A_Servicios" OnClick="SA_Click"  />
                </div>
                <div id="botonsa" class="btnnuevo" >
                    
                </div>
                    <table style="width: 100%;">
                        <tr>
                            <td class="auto-style8">
                                <asp:Label ID="proxs" runat="server" >Próximo Servicio</asp:Label>
                                <asp:TextBox ID="datepicker1" runat="server" CssClass="auto-style7"></asp:TextBox> <br /> <br />
                                <asp:Button ID="btnProxServicio" runat="server" Text="Agregar" Width="92px" AutoPostBack="True" OnClick="btnProxServicio_Click" />
                            </td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                    </table>
                </div>
        </section>
        
        <!--seccion para ingresar una nueva accion realizada -->
        <section class="centrar2"  id="floatsection" runat="server" style="display: none;">
            <div class="drop2" style="background-color: RGBA(255,255,255,1); padding:30px;" id="sectionf">
                <div class="buton" id="closebtn">
                    <asp:ImageButton Visible="true" ID="closeimg" runat="server" ImageAlign="Right" ImageUrl="Imagenes/closeimg.png" Width="30px" Height="30px" OnClick="closeimg_Click" />
                </div>

                <table class="auto-style5">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="textnueva" runat="server" Text="Nueva acción realizada" CssClass="auto-style4"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style6" colspan="2">
                            <asp:Label ID="fechad" runat="server" Text="Fecha:"></asp:Label>
                            <asp:TextBox ID="datepicker" runat="server" autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                        </td>                           
                    </tr>
                    <tr>
                        <td class="auto-style6" colspan="2">
                            <asp:Label ID="horasD" runat="server" Text="Horas Dedicadas:"></asp:Label>
                            <asp:TextBox ID="txthorasD" runat="server" autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                ControlToValidate="txthorasD" runat="server"
                                ErrorMessage="Solo se permiten números"
                                ValidationExpression="\d+">
                            </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style6" colspan="2">
                            <asp:Label ID="acciones" runat="server" Text="Acciones Realizadas:"></asp:Label>
                            <asp:TextBox ID="txtacciones" runat="server" Columns="2" MaxLength="240" AutoCompleteType="Disabled"></asp:TextBox>
                        </td>    
                    </tr>
                    <tr>
                        <td class="auto-style6" colspan="2">
                            <asp:Button runat="server" Text="Agregar" BorderStyle="None" style="float:right;" ID="Addbutton" OnClick="Addbutton_Click" />
                        </td>    
                    </tr>
                </table>

                </div>
        </section>

        <section class="centrar2"  id="observaciones" runat="server" style="display: none;">
            <div class="drop2" style="background-color: RGBA(255,255,255,1); padding:30px;" id="sectionf1">
                <div class="buton" id="closebtndiv">
                    <asp:ImageButton Visible="true" ID="closebtn1" runat="server" ImageAlign="Right" ImageUrl="Imagenes/closeimg.png" Width="30px" Height="30px" OnClick="closebtn1_Click"  />
                </div>

                <table class="auto-style5">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label2" runat="server" Text="Observaciones" CssClass="auto-style4"></asp:Label>
                        </td>
                    </tr>                
                    <tr>
                        <td class="auto-style6" colspan="2">
                            <asp:TextBox ID="txtobservaciones" runat="server" TextMode="MultiLine" Rows="7" MaxLength="240" AutoCompleteType="Disabled"></asp:TextBox>
                        </td>    
                    </tr>            
                    <tr>
                        <td class="auto-style6" colspan="2">                            
                            <asp:CheckBox id="Chck" runat="server" Text=" Notificar al Asesor" style="float:left;" OnCheckedChanged="Chck_CheckedChanged" />
                            <asp:Button runat="server" Text="Guardar" BorderStyle="None" style="float:right;" ID="btnguardar" OnClick="btnguardar_Click"/>
                        </td>
                    </tr>
                </table>
            </div>
        </section>

        <section class="centrar2"  id="FallaEncontrada" runat="server" style="display: none;">
            <div class="drop2" style="background-color: RGBA(255,255,255,1); padding:30px;" id="sectionf4">
                <div class="buton" id="closebtndiv1">
                    <asp:ImageButton Visible="true" ID="ImageButton3" runat="server" ImageAlign="Right" ImageUrl="Imagenes/closeimg.png" Width="30px" Height="30px" OnClick="closebtn2_Click"  />
                </div>

                <table  class="auto-style5">
                    <tr>
                        <td colspan="2">
                                <asp:Label ID="Label3" runat="server" Text="Fallas Encontradas" CssClass="auto-style4"></asp:Label>
                        </td>
                    </tr>                   
                    <tr>
                        <td class="auto-style6" colspan="2">
                            <asp:TextBox ID="txtfallaencontrada" runat="server" TextMode="MultiLine" Rows="7" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </td>    
                    </tr>                
                    <tr>
                        <td class="auto-style6" colspan="2">
                            <asp:Button runat="server" Text="Guardar" BorderStyle="None" style="float:right;" ID="Button2" OnClick="btnguardarfalla_Click"/>
                        </td>    
                    </tr>
                </table>
            </div>
        </section>
        <!-- Se implementa el formulario para la solicitud de refacciones-->  
        <section class="centrar2"  id="refacciones" runat="server" style="display: none;">
            <div class="drop2" style="background-color: RGBA(255,255,255,1); padding:30px;" id="sectionf2">
                <div class="buton" id="closebtnref2">
                    <asp:ImageButton Visible="true" ID="ImageButton2" runat="server" ImageAlign="Right" ImageUrl="Imagenes/closeimg.png" Width="30px" Height="30px" OnClick="closeimg1_Click" />
                </div>
                <div>
                <!--Aquí va la lista de refacciones que ya se han agregado con anterioridad -->
                    <div class="form-style-2-heading">
                        <asp:Label ID="Label4" runat="server">Detalle de Refacciones</asp:Label>
                    </div>
                        
                    <asp:Table ID="Table1" runat="server" Width="100%" GridLines="Vertical" BorderColor="#252932"> 
                    <asp:TableRow BackColor="#252932" ForeColor="White" HorizontalAlign="Center">
                    <asp:TableHeaderCell>N° de refacción</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Cantidad</asp:TableHeaderCell>
                    </asp:TableRow>
                    </asp:Table> 
                            
                    <div id="botonnuevoRef" class="btnnuevoRef">
                        <asp:Button runat="server" Text="Nuevo" BorderStyle="None" style="float:right" ID="btnNuevoR" OnClick="btnNuevoR_Click"/>
                    </div>
                </div>
            </div>
        </section>

        <section class="centrar2"  id="SectionNewRef" runat="server" style="display: none;">
            <div class="drop2" style="background-color: RGBA(255,255,255,1); padding:30px;" id="sectionf3">
                <div class="buton" id="closebtnref1">
                    <asp:ImageButton Visible="true" ID="ImageButton1" runat="server" ImageAlign="Right" ImageUrl="Imagenes/closeimg.png" Width="30px" Height="30px" OnClick="closeimg2_Click" />
                </div>

                <div>
                    <table class="auto-style5">
                        <tr>
                            <td>
                                <asp:Label ID="Label5" runat="server" Text="Agregar refacción" CssClass="auto-style4"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style6">
                                <asp:Label ID="Label6" runat="server" Text="N° de parte"></asp:Label>
                                <asp:TextBox ID="textboxidrefaccion" runat="server" autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                            </td>
                        </tr>                
                        <tr>
                            <td class="auto-style6">
                                <asp:Label ID="LBL_CANTIDAD_REFACCION" runat="server" Text="Cantidad"></asp:Label>
                                <asp:TextBox ID="textboxnumrefaccion" runat="server" Columns="2"  autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3"
                                    ControlToValidate="textboxnumrefaccion" runat="server"
                                    ErrorMessage="Solo se permiten números"
                                    ValidationExpression="\d+">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style6">
                                <asp:Label ID="Label8" runat="server" Text="Descripción"></asp:Label>
                                <asp:TextBox ID="textboxdescrefaccion" runat="server" TextMode="MultiLine" Rows="5" MaxLength="240" autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                            </td>   
                        </tr>
                        <tr>
                            <td class="auto-style6">
                                <asp:Button runat="server" Text="Agregar" BorderStyle="None" style="float:right;" ID="Button1" OnClick="btnrefaccion_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </section>
        <section class="centrar2"  id="avisodel" runat="server" style="display: none;">
            <div class="drop2" style="background-color: RGBA(255,255,255,1); padding:30px;" id="sectionf12">
                <table class="auto-style5">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label9" runat="server" Text="Aviso de Eliminación de Acción Realizada" CssClass="auto-style4"></asp:Label>
                        </td>
                    </tr>                
                    <tr>
                        <td class="auto-style6" colspan="2" style="text-align:left">
                            <asp:Label ID="Label10" runat="server">
                                La acción "<asp:label ID="descacci" runat="server"> Descripcion de la accion</asp:label>" con fecha "<asp:label ID="fechacci" runat="server"> 
                                fecha de la accion</asp:label>" y "<asp:label ID="horaacci" runat="server"> hora de la accion</asp:label>" horas de acción será eliminada 
                                permanentemente del folio <asp:label ID="fol" runat="server"/> con servicio de <asp:label ID="serv" runat="server"/>. <br /><br />
                                Si la información anterior es correcta, dar click en "Borrar Acción". <asp:label ID="IDAccion" runat="server" Visible ="false"/>
                            </asp:Label></td></tr><tr>
                        <td class="auto-style6" colspan="2">
                            <asp:Button runat="server" Text="Cancelar" BorderStyle="None" style="float:right; margin-left:5px" ID="Button5" OnClick="borrarnobtn_Click"/>
                            <asp:Button runat="server" Text="Borrar Acción" BorderStyle="None" style="float:right; margin-left:5px" ID="Button3" OnClick="borrarsibtn_Click"/>
                        </td>
                    </tr>
                </table>
            </div>
        </section>
        <footer runat="server" id="footerid" class="footercl">
            <div runat="server" id="obsbutton" class="footerbtn" >
                <asp:Button runat="server" Text="Observaciones" BorderStyle="None" style="float:unset;" ID="observacionesbtn" OnClick="observacionesbtn_Click"  />
            </div>
            <div runat="server" id="AddRef" class="footerbtn" >
                <asp:Button runat="server" Text="Refacciones" BorderStyle="None" style="float:unset;" ID="addrefbtn" OnClick="addrefbtn_Click"  />
            </div>
            <div runat="server" id="sirvebutton" class="footerbtn">
                <asp:Label runat="server" ID="funciona" Text="¿Funciona al 100%? "/>
                <label class="switch"style="color:azure">
                    <asp:CheckBox ID="CHECKED_ESTA_FUNCIONANDO" runat="server" OnCheckedChanged="CHECKED_ESTA_FUNCIONANDO_CheckedChanged" AutoPostBack="true" />
                    <span class="slider round"></span>
                </label>
            </div>
            <div runat="server" id="AddFalla" class="footerbtn" >
                 <asp:Button runat="server" Text="Reportar Fallas" BorderStyle="None" style="float:unset;" ID="addfallabtn" OnClick="Btn_Fallas_Encontradas_Click"  />
            </div>
            <div runat="server" id="vpbuttonid" class="footerbtn" >
                 <asp:Button runat="server" Text="Vista Previa" BorderStyle="None" style="float:unset;" ID="vpbutton"  OnClick="Btn_Vista_Previa_Click" />
            </div>
        </footer>
    </form>
</body>
</html>
