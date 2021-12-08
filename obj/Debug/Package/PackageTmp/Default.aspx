<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="program4._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h2 style="font-size: x-large; color: #FF6699"><strong>WELCOME TO POPPY LA&#39;S PROGRAM 4</strong></h2>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2 style="color: #CCFFFF; background-color: #FF6699"><strong>Load the input data</strong></h2>
            <p>This button will load the data from URL: </p>
            <p>https://s3-us-west-2.amazonaws.com/css490/input.txt</p>
            <p>
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Load data" Height="40px" Width="100px" />
            </p>
        </div>
        <div class="col-md-4">
            <h2 style="color: #CCFFFF; background-color: #FF6699"><strong>Query</strong></h2>
            <p>Input the name you would like to query. All the persons that have the matched name will be presented with their information.</p>
            <p>Access this URL to check the DataList: </p>
            <p>https://poppyla-program4.s3.us-west-2.amazonaws.com/pg4data.txt</p>
            <p>First name:
                <asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            </p>
            <p>Last name:
                <asp:TextBox ID="TextBox2" runat="server" OnTextChanged="TextBox2_TextChanged"></asp:TextBox>
            </p>
            <p>
                <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Query" Height="40px" Width="100px" />
            </p>
            <p>
                &nbsp;</p>
            <p>
                <asp:Label ID="Label1" runat="server" Text="" style="color: #FF6699; font-size: large; background-color: #FFFFFF"></asp:Label>
            </p>
            <p>
                <asp:Label ID="Label2" runat="server" Text="" style="color: #FF6699; font-size: large; background-color: #FFFFFF"></asp:Label>
            </p>
            <p>
                <asp:Label ID="Label3" runat="server" Text="" style="color: #FF6699; font-size: large; background-color: #FFFFFF"></asp:Label>
            </p>
        </div>
        <div class="col-md-4">
            <h2 style="color: #CCFFFF; background-color: #FF6699"><strong>Delete</strong></h2>
            <p>Data can only be clear if the data table and data list exist</p>
            <p>
                <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Clear data" Height="40px" Width="100px" />
            </p>
        </div>
    </div>

</asp:Content>
