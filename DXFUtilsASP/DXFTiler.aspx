<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DXFTiler.aspx.cs" Inherits="DXFUtilsASP.DXFTiler" %>

<%@ Register Src="~/User_Controls/DXF_Display_Control.ascx" TagPrefix="uc1" TagName="DXF_Display_Control" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>DXF Tiler</h1>
        <p class="lead">Routines for converting DXF files to sets of tiles</p>
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/dxf-icon-transparent.png" Width="150px" />
    </div>
    <hr /> 
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>    
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
                <asp:Label ID="LabelSelectedLayer" runat="server" Text=" "></asp:Label>
            </p>
            <h2>Grid Settings</h2>
                <p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label5" runat="server" Text="Number X" ></asp:Label>
                    <asp:TextBox ID="TextBoxNumberX" runat="server">2</asp:TextBox> <br />
                    </p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label9" runat="server" Text="Number Y" ></asp:Label>
                    <asp:TextBox ID="TextBoxNumberY" runat="server">2</asp:TextBox> <br />
                    </p>

                    <p>
                    <asp:Label class="text_box_label" ID="Label10" runat="server" Text="Pitch X(mm)" ></asp:Label>
                    <asp:TextBox ID="TextBoxPitchX" runat="server">20.0</asp:TextBox> <br />
                    </p>
                
                    <p>
                    <asp:Label class="text_box_label" ID="Label2" runat="server" Text="Pitch Y(mm)" ></asp:Label>
                    <asp:TextBox ID="TextBoxPitchY" runat="server">20.0</asp:TextBox> <br />
                    </p>

                    <p>
                    <asp:Label class="text_box_label" ID="Label3" runat="server" Text="Center X(mm)" ></asp:Label>
                    <asp:TextBox ID="TextBoxCenterX" runat="server">0.0</asp:TextBox> <br />
                    </p>

                    <p>
                    <asp:Label class="text_box_label" ID="Label4" runat="server" Text="Center Y(mm)" ></asp:Label>
                    <asp:TextBox ID="TextBoxCenterY" runat="server">0.0</asp:TextBox> <br />
                    </p>                    
                </p>
        </div>

        <uc1:DXF_Display_Control runat="server" ID="DXF_Display_Control" />

    </div>
    <hr />
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="ButtonUploadToServer" />
    </Triggers>
    </asp:UpdatePanel>
</asp:Content>
