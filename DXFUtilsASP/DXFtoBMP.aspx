<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DXFtoBMP.aspx.cs" Inherits="DXFUtilsASP.DXFtoBMP" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/Logo.bmp" Width="300" />
        <h1>DXF to Bitmap Converter</h1>
        <p class="lead">Routines for converting DXF files to bitmaps (.bmp/.tiff)</p>

    </div>
    <hr />
    <div class="row">
        <div class="col-md-4">
            <div>
                <h2>
                    Upload DXF file to server
                </h2>
            </div>
            <p> 
                <asp:Label ID="LabelFilename" class="btn btn-default" runat="server" for="FileUploadDXF_TO_BMP" Width="534px">
                    <asp:FileUpload class=".btn-lg" ID="FileUploadDXF_TO_BMP" runat="server" Width="270px" />
                </asp:Label>                

            </p>
            <p>
                
                <asp:Button class="btn btn-primary" ID="ButtonUploadToServer" runat="server" Text="Upload to Server" OnClick="ButtonUploadToServer_Click" />
                <asp:Label ID="LabelNoEntities" runat="server" Text="Number of Entities: 0"></asp:Label>
            </p>
            <p>
                <asp:Label ID="LabelWarn" runat="server" Text="Warning:" Visible="False"></asp:Label>
            </p>            
        </div>
    </div>
    <hr />
    <div>

        <h2>Data from loaded DXF:</h2>
        <p>
            <asp:BulletedList ID="BulletedListDXFInfo" runat="server"></asp:BulletedList>
        </p>
        <p>
            Note:  Polylines, LWPolylines and Splines converted to Arcs and Lines.  Blocks not currently handled.
        </p>

       </div>

</asp:Content>
