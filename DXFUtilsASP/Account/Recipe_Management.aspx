<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Recipe_Management.aspx.cs" Inherits="DXFUtilsASP.Account.Recipe_Management" %>


<%@ Register Src="~/User_Controls/WaitSpinner.ascx" TagPrefix="uc1" TagName="WaitSpinner" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="RecipeDB" runat="server" ConnectionString="<%$ ConnectionStrings:RecipeDBConnection %>" SelectCommand="SELECT * FROM [Bitmap_Recipes]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="TilerDB" runat="server" ConnectionString="<%$ ConnectionStrings:RecipeDBConnection %>" SelectCommand="SELECT * FROM [Tiler_Recipes]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="TNxTilerDB" runat="server" ConnectionString="<%$ ConnectionStrings:RecipeDBConnection %>" SelectCommand="SELECT * FROM [TNx_Recipes]"></asp:SqlDataSource>
    <div class="jumbotron">
        <h1>Recipe Management</h1>
        <p class="lead">View/manage contents of current recipe database</p>
    </div>
    <hr />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-md-8">
                    <h2>Bitmap Recipes</h2>
                    <asp:GridView ID="GridViewBitmapRecipes" runat="server" AutoGenerateColumns="False" DataSourceID="RecipeDB" AllowSorting="True" AllowPaging="True" EmptyDataText="No data to display" >
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                            <asp:BoundField DataField="Filepath" HeaderText="Filepath" SortExpression="Filepath" />
                            <asp:BoundField DataField="Created_By" HeaderText="Created_By" SortExpression="Created_By" />
                            <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                            <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" />
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="col-md-8">
                    <h2>Tiler Recipes</h2>
                    <asp:GridView ID="GridViewTiler" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" DataSourceID="TilerDB" AllowSorting="True" AllowPaging="True" EmptyDataText="No data to display" >
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" SortExpression="ID" />
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                            <asp:BoundField DataField="Filepath" HeaderText="Filepath" SortExpression="Filepath" />
                            <asp:BoundField DataField="Created_By" HeaderText="Created_By" SortExpression="Created_By" />
                            <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                            <asp:BoundField DataField="Date_Created" HeaderText="Date_Created" SortExpression="Date_Created" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <h2>Touchnetix Recipes</h2>
                    <asp:GridView ID="GridViewTNx" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" DataSourceID="TNxTilerDB" AllowSorting="True" AllowPaging="True" EmptyDataText="No data to display" >
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" SortExpression="ID" />
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                            <asp:BoundField DataField="Download_Filepath" HeaderText="Download_Filepath" SortExpression="Download_Filepath" />
                            <asp:BoundField DataField="Recipe_Pack_Filepath" HeaderText="Recipe_Pack_Filepath" SortExpression="Recipe_Pack_Filepath" />
                            <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                            <asp:BoundField DataField="Created_By" HeaderText="Created_By" SortExpression="Created_By" />
                            <asp:BoundField DataField="Creation_Date" HeaderText="Creation_Date" SortExpression="Creation_Date" />
                            <asp:BoundField DataField="Customer_Name" HeaderText="Customer_Name" SortExpression="Customer_Name" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </ContentTemplate>
    <Triggers>
    </Triggers>
    </asp:UpdatePanel>

</asp:Content>

