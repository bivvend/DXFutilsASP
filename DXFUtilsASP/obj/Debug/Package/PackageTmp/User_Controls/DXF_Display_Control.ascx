<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DXF_Display_Control.ascx.cs" Inherits="DXFUtilsASP.User_Controls.DXF_Display_Control" %>
<div class="col-md-4">
    <h2>Preview</h2>
    <p>
        <asp:Label ID="Label5" runat="server" Text="Preview Image"></asp:Label>
    </p>
    <p>
        <asp:Image ID="ImagePreview" runat="server" Height="600px" Width="600px" />
    </p>
</div>