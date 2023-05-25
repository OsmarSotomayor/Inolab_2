<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="experimento.aspx.cs" Inherits="INOLAB_OC.experimento" %>

<!DOCTYPE html>

<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>jQuery UI Tabs - Default functionality</title>
  
  <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
  <link rel="stylesheet" href="/resources/demos/style.css">
  <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
  <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
  <script>
  $( function() {
    $( "#tabs" ).tabs();
  } );
  </script>
</head>
<body>
 
<div id="tabs">
  <ul>
    <li><a href="#tabs-1">Pestaña  1</a></li>
    <li><a href="#tabs-2">Pestaña 2</a></li>
    <li><a href="#tabs-3">Pestaña 3</a></li>
    <li><a href="#tabs-4">Pestaña 4</a></li>
  </ul>
  <div id="tabs-1">
    <p>texto1</p>
  </div>
  <div id="tabs-2">
    <p>texto2</p>
  </div>
  <div id="tabs-3">
    <p>texto3</p>
    
  </div>
</div>
</body>
</html>