<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DXFtoBMP.aspx.cs" Inherits="DXFUtilsASP.DXFtoBMP" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/Logo.bmp" Width="300" />
        <h1>DXF to Bitmap Converter</h1>
        <p class="lead">Routines for converting DXF files to bitmaps (.bmp/.tiff)</p>

    </div>
    <hr />
    <div class="row">
        <div class="col-md-8">
            <div>
                <h2>
                    (1) Upload DXF file to server
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
        <div class="col-md-4">
            <h2>Data from Loaded DXF:</h2>
            <p>
                <asp:BulletedList ID="BulletedListDXFInfo" runat="server"></asp:BulletedList>
            </p>
            <p>
                Note:  Polylines, LWPolylines and Splines converted to Arcs and Lines.  Blocks not currently handled.
            </p>
        </div>
    </div>
    <hr />
    <div class="row">
        <div class="col-md-4">
            <h2>(2) Generate Preview</h2>
            <p>
                <asp:Label ID="Label1" runat="server" Text="Layer to render"></asp:Label>
                <br />
                <asp:ListBox ID="ListBoxLayers" runat="server" ForeColor="Black" Height="222px" Width="283px"></asp:ListBox>
            </p>      
            <p>
                <asp:Button class="btn btn-primary" ID="ButtonPreview" runat="server" Text="Preview" OnClick="ButtonPreview_Click" />
            </p>
        </div>
        <div class="col-md-4">
            <h2>Preview</h2>
            <p>
                <asp:Label ID="Label5" runat="server" Text="Preview Image"></asp:Label>
            </p>
            <p>
                <asp:Image ID="ImagePreview" runat="server" Height="400px" Width="400px" />
            </p>
        </div>

    </div>
    <hr />
    <div class="row">        
        <div class="col-md-4">
            <h2>Render Settings</h2>
            <p>
                <p>
                <asp:Label class="text_box_label" ID="Label2" runat="server" Text="DPI X" ></asp:Label>
                <asp:TextBox ID="TextBoxDPIX" runat="server">600</asp:TextBox> <br />
                </p>

                <p>
                <asp:Label class="text_box_label" ID="Label3" runat="server" Text="DPI Y" ></asp:Label>
                <asp:TextBox ID="TextBoxDPIY" runat="server">600</asp:TextBox> <br />
                </p>

                <p>
                <asp:Label class="text_box_label" ID="Label4" runat="server" Text="Border (mm)" ></asp:Label>
                <asp:TextBox ID="TextBoxBorder" runat="server">2</asp:TextBox> <br />
                </p>

                <p>
                    <asp:CheckBox ID="CheckBoxInvertX" runat="server"/>
                    <asp:Label class="checkbox label" ID="Label6" runat="server" Text=' Invert X' ></asp:Label>
                </p>

                <p>
                    <asp:CheckBox ID="CheckBoxInvertY" runat="server"/>
                    <asp:Label class="checkbox label" ID="Label7" runat="server" Text=' Invert Y Coordinates' ></asp:Label>
                </p>

                <p>
                    <asp:CheckBox ID="CheckBoxInvertColor" runat="server"/>
                    <asp:Label class="checkbox label" ID="Label8" runat="server" Text=' Invert Black/White' ></asp:Label>
                </p>
            </p>
        </div>
    </div>

</asp:Content>
