<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DXFTiler.aspx.cs" Inherits="DXFUtilsASP.DXFTiler" %>

<%@ Register Src="~/User_Controls/DXF_Display_Control.ascx" TagPrefix="uc1" TagName="DXF_Display_Control" %>
<%@ Register Src="~/User_Controls/WaitSpinner.ascx" TagPrefix="uc1" TagName="WaitSpinner" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>DXF Tiler</h1>
        <p class="lead">Routines for converting DXF files to sets of tiles</p>
        <table>
            <tr>
                <td rowspan="2"><asp:Image ID="Image6" runat="server" ImageUrl="~/Content/Images/dxf-icon-transparent.png"  Width="150px" /></td>
                <td><asp:Image ID="Image2" runat="server" ImageUrl="~/Content/Images/dxf-icon-transparent.png" Width="60px" /></td>
                <td><asp:Image ID="Image3" runat="server" ImageUrl="~/Content/Images/dxf-icon-transparent.png" Width="60px" /></td>
            </tr>
            <tr>
                <td><asp:Image ID="Image4" runat="server" ImageUrl="~/Content/Images/dxf-icon-transparent.png" Width="60px" /></td>
                <td><asp:Image ID="Image5" runat="server" ImageUrl="~/Content/Images/dxf-icon-transparent.png" Width="60px" /></td>

            </tr>
        </table>
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
                <p>
                </p>
                <p>
                    Note: Polylines, LWPolylines and Splines converted to Arcs and Lines. Blocks not currently handled.
                </p>                
                <p>
                </p>
                <p>
                </p>
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
                    <p>
                    </p>
                    <p>
                    </p>
                    <p>
                    </p>
                    <p>
                    </p>
                </p>
        </div>

        <uc1:DXF_Display_Control runat="server" ID="DXF_Display_Control" />

    </div>
    <hr />
    <div class="row">        
            <div class="col-md-8">
                <h2>(3) Tiling Settings</h2>
                <p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label6" runat="server" Text="Recipe Name" ></asp:Label>
                    <asp:TextBox ID="TextBoxOutputFilename" runat="server">Recipe</asp:TextBox> <br />
                    </p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label17" runat="server" Text="DXF version" ></asp:Label>
                    <asp:DropDownList ID="DropDownListDXFVesrion" ForeColor="Black" runat="server"></asp:DropDownList>
                    </p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label7" runat="server" Text="Layers (,)" ></asp:Label>
                    <asp:TextBox ID="TextBoxSelectedLayer" runat="server">All</asp:TextBox> <br />
                    </p>

                    <p>
                    <asp:Label class="text_box_label" ID="Label11" runat="server" Text="Python script" ></asp:Label>
                    <asp:TextBox ID="TextBoxSelectedScript" runat="server"></asp:TextBox> <br />
                    </p>

                    <p>
                    <asp:Label class="text_box_label" ID="Label8" runat="server" Text="Extensions (mm)" ></asp:Label>
                    <asp:TextBox ID="TextBoxExtensionsMM" runat="server">0.0</asp:TextBox> <br />
                    </p>

                    <p>
                        <asp:CheckBox ID="CheckBoxKnots" runat="server"/>
                        <asp:Label class="checkbox label" ID="Label13" runat="server" Text=' Apply Knots?' ></asp:Label>
                    </p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label12" runat="server" Text="Knot Type" ></asp:Label>
                    <asp:TextBox ID="TextBoxKnotType" runat="server">None</asp:TextBox> <br />
                    </p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label19" runat="server" Text="Knot Size(mm)" ></asp:Label>
                    <asp:TextBox ID="TextBoxKnotSize" runat="server">0.1</asp:TextBox> <br />
                    </p>

                    <p>
                        <asp:CheckBox ID="CheckBoxInvertX" runat="server"/>
                        <asp:Label class="checkbox label" ID="Label14" runat="server" Text=' Invert X' ></asp:Label>
                    </p>

                    <p>
                        <asp:CheckBox ID="CheckBoxInvertY" runat="server"/>
                        <asp:Label class="checkbox label" ID="Label15" runat="server" Text=' Invert Y Coordinates' ></asp:Label>
                    </p>

                    <p>
                        <asp:CheckBox ID="CheckBoxConvertToChords" runat="server"/>
                        <asp:Label class="checkbox label" ID="Label16" runat="server" Text=' Convert curves' ></asp:Label>
                    </p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label20" runat="server" Text="MScan DXF dir. " ></asp:Label>
                    <asp:TextBox ID="TextBoxMScanDir" runat="server">C:\Programs\Recipe</asp:TextBox> <br />
                    </p>
                    <p>
                    </p>
                    <p>
                        <asp:Button ID="ButtonRender" runat="server" class="btn btn-primary" Text="Run Script" Width="245px" OnClick="ButtonRender_Click" />
                        <asp:Button ID="ButtonDownload" runat="server" class="btn btn-primary" Text="Download zip file" Visible="False" Width="245px" OnClick="ButtonDownload_Click" />
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                        <ProgressTemplate>
                            <uc1:WaitSpinner runat="server" ID="WaitSpinner" />
                        </ProgressTemplate>
                        </asp:UpdateProgress>
                        <p>
                        </p>
                        <p>
                            <asp:Label ID="LabelRenderWarning" runat="server" Text=""></asp:Label>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                    </p>
                </p>
            </div>

            <div class="col-md-4">
                <h2>Rendering scripts</h2>
                <p>
                    <p>
                        <asp:Label ID="Label18" runat="server" Text="Script list"></asp:Label>
                    </p>
                    <p>
                        <asp:ListBox ID="ListBoxScripts" runat="server" Width="283px" ForeColor="Black" Height="128px"></asp:ListBox>
                    <p>
                        <asp:Button ID="ButtonSelectScript" class="btn btn-primary" runat="server" Text="Select Script" OnClick="ButtonSelectScript_Click" Width="280px" />
                    </p>    
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                </p>
            </div>
        </div>
        <hr />
        <div class="row">
            <div class="col-md-8">
                <h2>Script Output</h2>
                <p>
                    <asp:TextBox ID="TextBoxScriptOutput" runat="server" TextMode="MultiLine" Height="331px" Width="611px"></asp:TextBox>
                </p>      
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="ButtonUploadToServer" />
        <asp:PostBackTrigger ControlID="ButtonDownload" />
    </Triggers>
    </asp:UpdatePanel>
</asp:Content>
