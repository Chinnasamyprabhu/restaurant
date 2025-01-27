<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Dashborad.aspx.cs" Inherits="restaurant.Admin.Dashborad" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
         <div class="pcoded-inner-content">
        <div class="main-body">
            <div class="page-wrapper">

                <div class="page-body">
                    <div class="row">

                        <div class="col-md-6 col-xl-3">
                            <div class="card widget-card-1">
                                <div class="card-block-small">
                                    <i class="icofont icofont-cheese bg-c-lite-green card1-icon"></i>
                                    <span class="text-c-lite-green f-w-700">Categories</span>
                                    <h4> <%Response.Write(Session["category"]); %></h4>
                                    <div>
                                        <span class="f-left m-t-15 text-muted">
                                            <a href="Category.aspx"><i class="text-c-lite-green f-16 icofont icofont-eye-alt">View Details</i></a>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-xl-3">
                            <div class="card widget-card-1">
                                <div class="card-block-small">
                                    <i class="icofont icofont-food-basket bg-c-yellow card1-icon"></i>
                                    <span class="text-c-yellow f-w-700">Products</span>
                                    <h4> <%Response.Write(Session["product"]); %></h4>
                                    <div>
                                        <span class="f-left m-t-15 text-muted">
                                            <a href="Product.aspx"><i class="text-c-yellow f-16 icofont icofont-eye-alt">View Details</i></a>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-xl-3">
                            <div class="card widget-card-1">
                                <div class="card-block-small">
                                    <i class="icofont icofont-5-star-hotel bg-c-green card1-icon"></i>
                                    <span class="text-c-green f-w-700">Total Orders</span>
                                    <h4> <%Response.Write(Session["order"]); %></h4>
                                    <div>
                                        <span class="f-left m-t-15 text-muted">
                                            <a href="OrderStatus.aspx"><i class="text-c-green f-16 icofont icofont-eye-alt">View Details</i></a>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-xl-3">
                            <div class="card widget-card-1">
                                <div class="card-block-small">
                                    <i class="icofont icofont-car-alt-4 bg-c-pink card1-icon"></i>
                                    <span class="text-c-pink f-w-700">Delivered Items</span>
                                    <h4> <%Response.Write(Session["delivered"]); %></h4>
                                    <div>
                                        <span class="f-left m-t-15 text-muted">
                                            <a href="OrderStatus.aspx"><i class="text-c-pink f-16 icofont icofont-eye-alt">View Details</i></a>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">

                        <div class="col-md-6 col-xl-3">
                            <div class="card widget-card-1">
                                <div class="card-block-small">
                                    <i class="icofont icofont-delivery-time bg-c-orenge card1-icon"></i>
                                    <span class="text-c-orenge f-w-700">Pending-Items</span>
                                    <h4> <%Response.Write(Session["pending"]); %></h4>
                                    <div>
                                        <span class="f-left m-t-15 text-muted">
                                            <a href="OrderStatus.aspx"><i class="text-c-orenge f-16 icofont icofont-eye-alt">View Details</i></a>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-xl-3">
                            <div class="card widget-card-1">
                                <div class="card-block-small">
                                    <i class="icofont icofont-user-suited bg-c-blue card1-icon"></i>
                                    <span class="text-c-blue f-w-700">Users</span>
                                    <h4> <%Response.Write(Session["user"]); %></h4>
                                    <div>
                                        <span class="f-left m-t-15 text-muted">
                                            <a href="Users.aspx"><i class="text-c-blue f-16 icofont icofont-eye-alt">View Details</i></a>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-xl-3">
                            <div class="card widget-card-1">
                                <div class="card-block-small">
                                    <i class="icofont icofont-money bg-danger card1-icon"></i>
                                    <span class=" text-danger f-w-700">Sold Amount</span>
                                    <h4> <%Response.Write(Session["soldAmount"]); %></h4>
                                    <div>
                                        <span class="f-left m-t-15 text-muted">
                                            <a href="Report.aspx"><i class="text-danger f-16 icofont icofont-eye-alt">View Details</i></a>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-xl-3">
                            <div class="card widget-card-1">
                               <div class="card-block-small">
                                    <i class="icofont icofont-pen bg-inverse card1-icon"></i>
                                    <span class="text-inverse f-w-700">FeedBacks</span>
                                    <h4> <%Response.Write(Session["conact"]); %></h4>
                                    <div>
                                        <span class="f-left m-t-15 text-muted">
                                            <a href="Contacts.aspx"><i class="text-inverse f-16 icofont icofont-eye-alt">View Details</i></a>
                                        </span>
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
