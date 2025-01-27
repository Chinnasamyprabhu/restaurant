<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="restaurant.Admin.Product" %>
<%@ Import Namespace="restaurant" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        /*For disappearing aler message*/
        window.onload = function () {

            var seconds = 5;
            setTimeout(function ()
            {
                document.getElementById("<%=lblMsg.ClientID%>").style.display = "none";

            }(seconds * 1000));
        };

    </script>
    <script>
        function ImagePreview(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#<%=imgProduct.ClientID%>').prop('src', e.target.result)
                        .width(200)
                        .height(200);
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="pcoded-inner-content pt-0">
        <div class="align-align-self-end">
            <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>

        </div>
        <div class="main-body">
            <div class="page-wrapper">

                <div class="page-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="card">
                                <div class="card-header">
                                </div>
                                <div class="card-block">
                                    <div class="row">
                                        <div class="col-sm-6 col-md-4 col-lg-4">
                                            <h4 class="sub-title">Product Name</h4>

                                            <div class="form-group">
                                                <label>Product Name</label>
                                                <div>
                                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control"
                                                        placeholder="Enter Product Name" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                                        ErrorMessage="Enter The Name" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                                         ControlToValidate="txtName">
                                                    </asp:RequiredFieldValidator>
                                                    <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                                </div>
                                            </div>

                                             <div class="form-group">
                                                <label>Product Description</label>
                                                <div>
                                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"
                                                        placeholder="Enter Product Description" TextMode="MultiLine"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                                        ErrorMessage="Enter The Description" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                                         ControlToValidate="txtDescription">
                                                    </asp:RequiredFieldValidator>
                                                   
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label>Product Price(₹)</label>
                                                <div>
                                                    <asp:TextBox ID="txtprice" runat="server" CssClass="form-control"
                                                        placeholder="Enter Product Price" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                                        ErrorMessage="Enter The Name" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                                         ControlToValidate="txtprice">
                                                    </asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                        ErrorMessage="Price should be in decimal" ForeColor="Red" Display="Dynamic" SetFocusOnError="True"
                                                         ControlToValidate="txtprice" ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                                                </div>
                                            </div>

                                             <div class="form-group">
                                                <label>Product Quantity</label>
                                                <div>
                                                    <asp:TextBox ID="txtquanity" runat="server" CssClass="form-control"
                                                        placeholder="Enter Product Quantity" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                                        ErrorMessage="Enter The Quantity" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                                         ControlToValidate="txtquanity">
                                                    </asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                                        ErrorMessage="Quantity should not be in negative" ForeColor="Red" Display="Dynamic" SetFocusOnError="True"
                                                         ControlToValidate="txtquanity" ValidationExpression="^(?:[1-9]\d*|0)?(?:\.\d+)?$"></asp:RegularExpressionValidator>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label>Product Image</label>
                                                <div>
                                                    <asp:FileUpload ID="fuProductImage" runat="server" CssClass="form-control"
                                                        onchange="ImagePreview(this);" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label>Product Category</label>
                                                <div>
                                                    <asp:DropDownList ID="ddlCategories" CssClass="form-control" runat="server"
                                                         DataSourceID="SqlDataSource1" DataTextField="Name" DataValueField="CategoryId" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">Select Category</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                                        ErrorMessage="Category is required" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                                         ControlToValidate="ddlCategories" InitialValue="0">
                                                    </asp:RequiredFieldValidator>
                                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:cs %>" 
                                                         SelectCommand="SELECT [CategoryId], [Name] FROM [Categories]"></asp:SqlDataSource>
                                                </div>
                                            </div>
                                            <div class="form-check pl-4">
                                                <asp:CheckBox ID="cbIsActive" runat="server" Text="&nbsp; IsActive" CssClass="form-check-input" />

                                            </div>
                                            <div class="pb-5">
                                                <asp:Button ID="btnAddorUpdate" runat="server" Text="Add" CssClass="btn btn-primary"
                                                     OnClick="btnAddorUpdate_Click"/>
                                                &nbsp;
                                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-primary" CausesValidation="false" 
                                                 OnClick="btnClear_Click"/>
                                            </div>
                                            <div>
                                                <asp:Image ID="imgProduct" runat="server" CssClass="img-thumbnail" />
                                            </div>
                                        </div>

                                        <div class="col-sm-6 col-md-8 col-lg-8 mobile-inputs">
                                            <h4 class="sub-title">Product Lists</h4>
                                            <div class="card-block table-border-style">
                                                <div class="table-responsive">
                                                    <asp:Repeater ID="rProduct" runat="server" OnItemCommand="rProduct_ItemCommand" 
                                                         OnItemDataBound="rProduct_ItemDataBound">
                                                        <HeaderTemplate>
                                                            <table class="table data-table-export table-hover nowrap">
                                                                <thead>
                                                                    <tr>
                                                                        <th class="table-plus">Name</th>
                                                                        <th>Image</th>
                                                                        <th>Price(₹)</th>
                                                                        <th>Qty</th>
                                                                        <th>Category</th>
                                                                        <th>IsActive</th>
                                                                        <th>Description</th>
                                                                        <th>CreatedDate</th>
                                                                        <th class="datatable-nosort">Action</th>
                                                                    </tr>

                                                                </thead>
                                                                <tbody>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td class="table-plus"><%# Eval("Name") %> </td>

                                                                <td>
                                                                    <img alt="" width="40" src="<%# Utils.GetImageUrl( Eval("ImageUrl")) %>" />
                                                                </td>

                                                                <td><%# Eval("Price") %> </td>

                                                                 <td>
                                                                    <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>'></asp:Label>
                                                                </td>

                                                                <td><%# Eval("CategoryName") %> </td>

                                                                <td>
                                                                    <asp:Label ID="lblIsActive" runat="server" Text='<%# Eval("IsActive") %>'></asp:Label>
                                                                </td>

                                                                 <td><%# Eval("Description") %> </td>

                                                                <td><%# Eval("CreatedDate") %> </td>

                                                                <td>
                                                                    <asp:LinkButton ID="lnkEdit" Text="Edit " runat="server" CssClass="badge badge-primary" CausesValidation="false"
                                                                        CommandArgument='<%# Eval("ProductId") %>' CommandName="edit"><i class="ti-pencil"></i> 
                                                                    </asp:LinkButton>
                                                                    <asp:LinkButton ID="lnkDelete" Text="Delete" runat="server" CommandName="delete" CausesValidation="false"
                                                                        CssClass="badge bg-danger" CommandArgument='<%# Eval("ProductId") %>'
                                                                        OnClientClick="return confirm('Do you want to delete this Product?');">
    <i class="ti-trash"></i>
</asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            </tbody>
                                                          </table>
                                                        </FooterTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>
                                        </div>


                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
        </div>
</asp:Content>
