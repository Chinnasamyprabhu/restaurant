<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="restaurant.User.Registration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script>
       /*For disappearing alert message*/
       window.onload = function () {

           var seconds = 5;
           setTimeout(function () {
               document.getElementById("<%=lblMsg.ClientID%>").style.display = "none";

              }(seconds * 2000));
       };

 </script>

    <script>
        function ImagePreview(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#<%=imgUser.ClientID%>').prop('src', e.target.result)
                        .width(200)
                        .height(200);
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <section class="book_section layout_padding">
        <div class="container">
            <div class="heading_container">
                <div class="align-self-end">
                    <asp:Label ID="lblMsg" runat="server" CssClass="alert" Visible="false"></asp:Label>
                    <asp:Label ID="lblSuccessMsg" runat="server" Visible="false" CssClass="alert alert-success"></asp:Label>
                </div>
                <asp:Label ID="lblHeaderMsg" runat="server" Text="<h2>User Registration</h2>"></asp:Label>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <div class="form_container">

                        <div>
                            
                            <asp:RegularExpressionValidator ID="rev" runat="server" ErrorMessage="Name Must be in character" ControlToValidate="txtName"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^([\sA-Za-z]+)$"></asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="rfv" runat="server" ErrorMessage="Enter the Name" ControlToValidate="txtName"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" ToolTip="Full Name" placeholder="Enter Full Name"></asp:TextBox>
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="rfvUser" runat="server" ErrorMessage="UserName is required" ControlToValidate="txtUserName"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="false"></asp:RequiredFieldValidator>
                              <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="Enter UserName" ToolTip="Username"></asp:TextBox>
                            <asp:Label ID="lblUsernameError" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label>
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="RfvEmail" runat="server" ErrorMessage="Enter the Email I'd" ControlToValidate="txtEmail"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="false"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revmail" runat="server" ErrorMessage="Invalid Email I'd" ControlToValidate="txtEmail"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter Email I'd" ToolTip="Email" TextMode="Email"></asp:TextBox>
                        </div>
                        <div>
                            <asp:RegularExpressionValidator ID="revMobile" runat="server" ErrorMessage="Number Must be 10 digits followed by +91" ControlToValidate="txtMoblie"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^([0]|\+91)?[6-9]\d{9}$"></asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="rfvmobile" runat="server" ErrorMessage="Enter the Mobile Number" ControlToValidate="txtMoblie"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            <asp:TextBox ID="txtMoblie" runat="server" CssClass="form-control" ToolTip="Full Name" placeholder="Enter Mobile Number"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form_container">
                        
                        <div>
                            <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ErrorMessage="Enter the Address" ControlToValidate="txtaddress"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                             <asp:TextBox ID="txtaddress" runat="server" CssClass="form-control" Placeholder="Enter Address" ToolTip="Address" TextMode="MultiLine"
                                 ></asp:TextBox>
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="rfvPostCode" runat="server" ErrorMessage="UserName is required" ControlToValidate="txtPostCode"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="false"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revPostCode" runat="server" ErrorMessage="PostCode must be of 6 digits" ControlToValidate="txtPostCode"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^[0-9]{6}$"></asp:RegularExpressionValidator>
                            <asp:TextBox ID="txtPostCode" runat="server" CssClass="form-control" placeholder="Enter Post/Zip Code" ToolTip="PostCode" TextMode="Number"></asp:TextBox>
                        </div>
                        <div>
                            <asp:FileUpload ID="fuUserImage" runat="server" CssClass="form-control" ToolTip="User Image" 
                                onchange="ImagePreview(this);"/>
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="rfvPass" runat="server" ErrorMessage="Enter the Password" ControlToValidate="txtPassword"
                                ForeColor="Red" Display="Dynamic" SetFocusOnError="true" BorderStyle="None"></asp:RequiredFieldValidator>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" ToolTip="Password" placeholder="Enter Password" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="row pl-4">
                    <div class="btn-box">
                        <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-success rounded-pill pl-4 pr-4 text-white"
                             OnClick="btnRegister_Click"/>

                       <asp:Label ID="lblAlreadyUser" runat="server" CssClass="pl-3 text-black-100"
                           Text="Already registered? <a href='Login.aspx' class='badge badge-info'>Login here..</a>">

                       </asp:Label>
                    </div>
                </div>
                <div class="row p-5">
                    <div style="align-items:center">
                        <asp:Image ID="imgUser" runat="server" CssClass="img-thumbnail" />
                    </div>
                </div>
            </div>
        </div>
    </section>


</asp:Content>
