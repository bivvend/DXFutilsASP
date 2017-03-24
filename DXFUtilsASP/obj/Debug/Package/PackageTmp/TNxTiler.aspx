<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="TNxTiler.aspx.cs" Inherits="DXFUtilsASP.TNxTiler" %>

<%@ Register Src="~/User_Controls/WaitSpinner.ascx" TagPrefix="uc1" TagName="WaitSpinner" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="TNxRecipeDB" runat="server" ConnectionString="<%$ ConnectionStrings:RecipeDBConnection %>" SelectCommand="SELECT * FROM [TNx_Recipes]"></asp:SqlDataSource>
    <div class="jumbotron">
        <h1>Touchnetix Recipe Tiler</h1>
        <p class="lead">Routines for splitting Touchnetix DXF files into tiles </p>
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
                        Note: Polylines, LWPolylines and Splines converted to Arcs and Lines. Entities in Blocks not counted.
                    </p>
                    <p>
                        <asp:Label ID="Label21" runat="server" Text="Layer List"></asp:Label>
                        <br />
                        <asp:ListBox ID="ListBoxLayers" runat="server" ForeColor="Black" Height="222px" Width="283px"></asp:ListBox>
                    </p>
                    <p>
                    </p>
                    <p>
                        <asp:Button ID="ButtonAddLayer" class="btn btn-primary" runat="server" Text="Add Layer"  Width="280px" OnClick="ButtonAddLayer_Click" />                    
                    </p>
                    <p>
                    </p>
                </p>
            </div>
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
                        <asp:CheckBox ID="CheckBoxInvertX" runat="server"/>
                        <asp:Label class="checkbox label" ID="Label14" runat="server" Text=' Invert X' ></asp:Label>
                    </p>
                    <p>
                        <asp:CheckBox ID="CheckBoxInvertY" runat="server"/>
                        <asp:Label class="checkbox label" ID="Label15" runat="server" Text=' Invert Y Coordinates' ></asp:Label>
                    </p>
                    <p>
                        <asp:CheckBox ID="CheckBoxConvertToLines" runat="server"/>
                        <asp:Label class="checkbox label" ID="Label1" runat="server" Text=' Convert to LINES' ></asp:Label>
                    </p>
                    <p>
                    <asp:Label class="text_box_label" ID="Label20" runat="server" Text="MScan DXF dir. " ></asp:Label>
                    <asp:TextBox ID="TextBoxMScanDir" runat="server">C:\Programs\Recipes</asp:TextBox> <br />
                    </p>
                    <p>
                        <asp:Button ID="ButtonRender" runat="server" class="btn btn-primary" Text="Run Script" Width="245px" OnClick="ButtonRender_Click" />
                        <asp:Button ID="ButtonDownload" runat="server" class="btn btn-primary" Text="Download zip file" Visible="False" Width="245px" OnClick="ButtonDownload_Click" />
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                        <ProgressTemplate>
                            <uc1:WaitSpinner runat="server" id="WaitSpinner" />
                        </ProgressTemplate>
                        </asp:UpdateProgress>
                    </p>
                    <p>
                        <asp:Label ID="LabelRenderWarning" runat="server" Text=""></asp:Label>
                    </p>
            </div>

            <div class="col-md-4">
                <h2>Rendering scripts</h2>
                <p>
                    <p>
                        <asp:Label ID="Label10" runat="server" Text="Script list"></asp:Label>
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
