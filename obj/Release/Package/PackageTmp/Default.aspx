<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="program4._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>PROGRAM4</h1>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Load</h2>
            <p>
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
            </p>
        </div>
        <div class="col-md-4">
            <h2>Query</h2>
            <p>Last name:
                <asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            </p>
            <p>First name:
                <asp:TextBox ID="TextBox2" runat="server" OnTextChanged="TextBox2_TextChanged"></asp:TextBox>
            </p>
            <p>
                <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Button" />
            </p>
            <p>
                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            </p>
            <p>
                <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
            </p>
            <p>
                <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Delete</h2>
            <p>
                <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Button" />
            </p>
        </div>
    </div>

</asp:Content>
