<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DXFtoBMP.aspx.cs" Inherits="DXFUtilsASP.DXFtoBMP" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>DXF to Bitmap Converter</h1>
        <p class="lead">Routines for converting DXF files to bitmaps.</p>

    </div>
    <div class="row">
        <div class="col-md-4">
            <p>
                <asp:Button ID="ButtonLoadDXF" class="btn btn-primary" runat="server" Text="Load DXF" Height="43px" OnClick="ButtonLoadDXF_Click" Width="128px" />
                <asp:Label ID="LabelNoEntities" runat="server" Text="Number of Entities: 0"></asp:Label>
            </p>            
        </div>
    </div>
    <hr />
    <div>

        <h2>Data from loaded DXF:</h2>
        <p>
            <ul>
                <li>Number of LAYERS</li>
                <li>Number of LINES</li>
                <li>Number of ARCS</li>
                <li>Number of CIRCLES</li>
            </ul>
        </p>

       </div>

</asp:Content>
