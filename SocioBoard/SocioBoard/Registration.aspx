﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/socialsuite.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SocioBoard.Registration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <script type="text/javascript">
        $(document).ready(function () {
            //alert("asdad");
            $("#ContentPlaceHolder1_btnRegister").click(function () {
                var txtemail = $('#ContentPlaceHolder1_txtEmail').val();
                if (txtemail.indexOf(".web") != -1) {
                    alert("Please Enter valid email!");
                    return false;
                }
            });


            $("#check").click(function () {
                //var couponcode = $("#TextBox1").val();
                var couponcode = $('#<%=TextBox1.ClientID%>').val();
                if (couponcode == "") {
                    alert("Please enter coupon Code")
                    return false;
                }
                $.ajax({
                    url: '../Ajaxcoupon.aspx',
                    type: 'POST',
                    data: 'code=' + couponcode,
                    dataType: 'html',
                    success: function (msg) {
                        //$("#rss_users").html(msg);
                        //$("#check").html(msg);
                        alert(msg);
                    }
                });


            });

            $("#loginlink").click(function () {
                debugger;
                document.getElementById('signinemailMessages').innerHTML = "";
                document.getElementById('signinpasswordMessages').innerHTML = "";
                $('#logindiv').bPopup({
                    easing: 'easeOutBack',
                    positionStyle: 'fixed',
                    speed: 650,
                    transition: 'slideDown'
                });
            });


        });
</script>
<style type="text/css">

.ws_err_star_enter
{
    float:left;
    padding:6px;
}
.ws_err_star
{
     float:left;
     margin-top:-27px;
}
</style>
    <div class="ssp_registration_page">
        <div class="login_reg_bg">
            <div class="signin">
                Create Your Account</div>
            <div class="reg_temp_reg">
                 <div class="left_reg_area">

                  <div class="inputbg"><asp:DropDownList class="drop" ID="DropDownList1" runat="server">
                  <asp:ListItem Value="Select">Select Account Type</asp:ListItem>
                      <asp:ListItem Value="Free">Basic(Free)</asp:ListItem>
                      <asp:ListItem Value="Standard">Standard</asp:ListItem>
                      <asp:ListItem Value="Deluxe">Deluxe</asp:ListItem>
                      <asp:ListItem Value="Premium">Premium</asp:ListItem>
                      <asp:ListItem Value="SocioBasic">SocioBasic</asp:ListItem>
                      <asp:ListItem Value="SocioStandard">SocioStandard</asp:ListItem>
                      <asp:ListItem Value="SocioPremium">SocioPremium</asp:ListItem>
                      <asp:ListItem Value="SocioDeluxe">SocioDeluxe</asp:ListItem>

                   </asp:DropDownList></div>



            <div class="inputbg"><asp:TextBox ID="txtFirstName" runat="server" placeholder="First Name"></asp:TextBox></div>
            <div class="error"><asp:RequiredFieldValidator ID="RequiredFieldValidator5" 
                    runat="server" ErrorMessage="Please Enter First Name!"
                                ControlToValidate="txtFirstName"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                    ErrorMessage="Please Enter Only Alphabets" ControlToValidate="txtFirstName" 
                    ValidationExpression="[a-zA-Z]+[ a-zA-Z-_]*"></asp:RegularExpressionValidator>
                                </div>
            <div class="inputbg"><asp:TextBox ID="txtLastName" runat="server" placeholder="Last Name"></asp:TextBox></div>
            <div class="error"><asp:RequiredFieldValidator ID="RequiredFieldValidator6" 
                    runat="server" ErrorMessage="Please Enter Last Name!"
                                ControlToValidate="txtLastName"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                    ErrorMessage="Please Enter Only Alphabets" ControlToValidate="txtLastName" 
                    ValidationExpression="[a-zA-Z]+[ a-zA-Z-_]*"></asp:RegularExpressionValidator>
                                </div>
            <div class="inputbg"><asp:TextBox ID="txtEmail" runat="server" placeholder="Email"></asp:TextBox></div>
            <div class="error"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
                    runat="server" ErrorMessage="Please Enter Email!"
                                ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="Please Enter Valid Email!"
                                ControlToValidate="txtEmail" CssClass="ws_err_star_enter" 
                    ValidationExpression="^([\w-\.+ ]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"></asp:RegularExpressionValidator>
                 


</div>
            <div class="input_pswd_bg"><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" AUTOCOMPLETE="off" placeholder="Password"></asp:TextBox></div>
            <div class="error"> <asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
                    runat="server" ErrorMessage="Please Enter Password!"
                                ControlToValidate="txtPassword" ></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                    runat="server" ErrorMessage="Please Enter Valid Password!"
                                ControlToValidate="txtPassword" ValidationExpression="^\S+$"
                                CssClass="ws_err_star_enter" ></asp:RegularExpressionValidator></div>

            <div class="input_pswd_bg"> <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder="Confirm Password"></asp:TextBox></div>
            <div class="error">    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" 
                    runat="server" ErrorMessage="Please Enter Confirm Password!"
                                ControlToValidate="txtConfirmPassword" ></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Password & Confirm Password should be same"
                                ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword"
                                CssClass="ws_err_star_enter"></asp:CompareValidator></div>


                                <div class="input_pswd_bg" style="width:120%"> <asp:TextBox style="width:218px;" ID="TextBox1" runat="server" placeholder="Coupon Code(Optional)"></asp:TextBox><a id="check" style="margin-left: 29px;" href="#">Click to verify</a></div>


 <asp:Label ID="lblerror" runat="server" CssClass="ws_err_star" style="text-decoration:none; float:left; margin-top:13px;"></asp:Label>
               <div class="create_your_account">
                   <asp:ImageButton ID="btnRegister" runat="server" 
                       ImageUrl="../Contents/img/create_account.png" OnClick="btnRegister_Click" 
                        />
             </div>
            
         </div>
                <!--<div class="right_area_reg">
             <div class="social_bg">
                <div class="you_cantext">Sign In With Twitter <br />
                 Your 30 day trial lasts until midnight on  <br /><b> June 27, 2013</b></div>
                 <div class="create_your_account"><a href="#"><img src="../Contents/img/create_account.png" alt="" boeder="none" /></a></div>
                               
                <div class="register_regtext">By clicking 'Create Account' you agree to the <br /> <a>Terms of Service</a> and <a>Privacy policies</a>. </div>
                
             </div>
         </div>-->
            </div>
        </div>
    </div>
</asp:Content>
