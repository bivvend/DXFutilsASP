<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WaitSpinner.ascx.cs" Inherits="DXFUtilsASP.User_Controls.WaitSpinner" %>
<div class="PleaseWait">
        <asp:Label ID="Label5" runat="server" Text="Please Wait..."></asp:Label>
        <asp:Image ID="ImageWaitGif" ImageUrl="~/Content/Images/PleaseWait.gif" Width="50px" runat="server" Height="50px"/>
</div>