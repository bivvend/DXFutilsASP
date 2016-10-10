<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DXFTiler.aspx.cs" Inherits="DXFUtilsASP.DXFTiler" %>

<%@ Register Src="~/User_Controls/DXF_Display_Control.ascx" TagPrefix="uc1" TagName="DXF_Display_Control" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>DXF Tiler</h1>
        <p class="lead">Routines for converting DXF files sets of tiles</p>
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/dxf-icon-transparent.png" Width="150px" />
    </div>
    <hr /> 
    
    <uc1:DXF_Display_Control runat="server" ID="DXF_Display_Control" /> 
    <asp:Button ID="Button1" runat="server" Text="Make Preview" OnClick="Button1_Click" />
</asp:Content>
