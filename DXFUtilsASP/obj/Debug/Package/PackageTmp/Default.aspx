<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DXFUtilsASP._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>DXF Utilities Main Page</h1>
        <p class="lead">The DXF Utilities webpage is the front end for a collection of recipe generation routines for converting DXF files.</p>
        <p>&nbsp;</p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                Please login first before starting conversion.
            </p>
            <p>                
                &nbsp;<a href="/Account/Login" class="btn btn-primary btn-lg">Login or Register&raquo;</a></p>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Project Structure</h2>
            <p>
                DXF Utils is written using ASP.NET</p>
            <p>
                <a class="btn btn-default" href="http://www.asp.net">Learn more &raquo;</a>
        </div>
    </div>

</asp:Content>
