<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DXFtoBMP.aspx.cs" Inherits="DXFUtilsASP.DXFtoBMP" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>DXF to Bitmap Converter</h1>
        <p class="lead">Routines for converting DXF files to bitmaps.</p>

    </div>

        <div class="row">
        <div class="col-md-4">
            <p>
                <asp:Button ID="Button1" runat="server" Text="Button" />
                <asp:Label ID="Label2" runat="server" Text="Number of Entities: 0"></asp:Label>
            </p>
            <asp:Label ID="Label1" runat="server" Text="Load DXF"></asp:Label>
        </div>
    </div>

</asp:Content>
