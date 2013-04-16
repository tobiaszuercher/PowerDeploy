<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SampleApp.Web.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <div>
        Config value: <%= ConfigurationManager.AppSettings["Setting"] %>    
    </div>
</body>
</html>
    