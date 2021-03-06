﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SocioBoard.Domain;
using SocioBoard.Model;
using GlobusGooglePlusLib.Authentication;
using GlobusInstagramLib.Authentication;
using GlobusLinkedinLib.Authentication;
using GlobusTwitterLib.Authentication;
using System.Configuration;
using System.Collections;
using log4net;
using SocioBoard.Helper;

namespace SocioBoard
{
    public partial class Publishing : System.Web.UI.Page
    {
        ILog logger = LogManager.GetLogger(typeof(Publishing));
        string Datetime = string.Empty;
        UserRepository objUserRepository = new UserRepository();
        protected void Page_Load(object sender, EventArgs e)
        {
            TeamRepository objTeamRepository = new TeamRepository();
            GroupRepository objGroupRepository = new GroupRepository();
            //SocioBoard.Domain.Team team = (SocioBoard.Domain.Team)Session["GroupName"];
           //  Session["groupcheck"]

            User user = (User)Session["LoggedUser"];

            try
            {
                #region for You can use only 30 days as Unpaid User

                //SocioBoard.Domain.User user = (User)Session["LoggedUser"];
                if (user.PaymentStatus.ToLower() == "unpaid")
                {
                    if (!SBUtils.IsUserWorkingDaysValid(user.ExpiryDate))
                    {
                        // ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('You can use only 30 days as Unpaid User !');", true);

                        Session["GreaterThan30Days"] = "GreaterThan30Days";

                        Response.Redirect("/Settings/Billing.aspx");
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.StackTrace);
            }




            if (user != null)
            {
                  if (user.ActivationStatus == "0" || user.ActivationStatus == null)
                {
                    actdiv.InnerHtml = "<marquee behavior=\"scroll\" direction=\"left\"><span >Your account is not yet activated.Check your E-mail to activate your account.</span><a id=\"resendmail\" uid=\"" + user.Id + "\" href=\"#\">Resend Mail</a></marquee>";
                    if (Request.QueryString["stat"] == "activate")
                    {
                        if (Request.QueryString["id"] != null)
                        {
                            //objUserActivation = objUserActivationRepository.GetUserActivationStatusbyid(Request.QueryString["id"].ToString());
                            if (user.Id.ToString() == Request.QueryString["id"].ToString())
                            {
                                user.Id = user.Id; //Guid.Parse(Request.QueryString["id"]);
                                //objUserActivation.UserId = Guid.Parse(Request.QueryString["id"]);// objUserActivation.UserId;
                                user.ActivationStatus = "1";
                                //UserActivationRepository.Update(objUserActivation);

                                int res = objUserRepository.UpdateActivationStatusByUserId(user);

                                actdiv.Attributes.CssStyle.Add("display", "none");
                                Console.WriteLine("before");
                                #region to check/update user Reference Relation
                                IsUserReferenceActivated(Request.QueryString["id"].ToString());
                                Console.WriteLine("after");
                                #endregion





                            }
                            else
                            {
                                Session["ActivationError"] = "Wrong Activation Link please contact Admin!";
                                //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Wrong Activation Link please contact Admin!');", true);
                                //Response.Redirect("ActivationLink.aspx");


                            }
                        }
                        else
                        {
                            Session["ActivationError"] = "Wrong Activation Link please contact Admin!";
                            //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Wrong Activation Link please contact Admin!');", true);
                            //Response.Redirect("ActivationLink.aspx");
                        }

                    }
                    else
                    {
                        // Response.Redirect("ActivationLink.aspx");
                    }


                }
              
                
                //if (Session["groupcheck"] == null)
                //{
                   
                //    Session["groupcheck"] = groupsselection.SelectedValue;
                //}
                //else
                //{
                //    groupsselection.SelectedValue = Session["groupcheck"].ToString();
                //}
              
               
                
                if (user.ActivationStatus == "1")
                {
                    actdiv.Attributes.CssStyle.Add("display", "none");
                }
            }


            if (!IsPostBack)
            {
                if (user == null)
                    Response.Redirect("/Default.aspx");

                else
                {


                    try
                    {
                        ArrayList totalAccuount = objTeamRepository.getAllAccountUser(user.EmailId,user.Id);
                        if (totalAccuount.Count != 0)
                        {

                            try
                            {
                                foreach (Guid item in totalAccuount)
                                {
                                    Guid GroupIde = (Guid)item;
                                    List<Groups> GetData = objGroupRepository.getAllGroupsDetail(GroupIde);
                                    if (GetData.Count != 0)
                                    {
                                        foreach (var items in GetData)
                                        {
                                            try
                                            {
                                                groupsselection.Items.Add(new ListItem((string)items.GroupName, items.Id.ToString()));
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                                logger.Error("Error : " + ex.Message);
                                                logger.Error("Error : " + ex.StackTrace);
                                            }
                                        }
                                    }
                                    if (Session["groupcheck"] == null)
                                    {

                                        Session["groupcheck"] = groupsselection.SelectedValue;
                                    }
                                    else
                                    {
                                        groupsselection.SelectedValue = Session["groupcheck"].ToString();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                logger.Error("Error : " + ex.Message);
                                logger.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        logger.Error("Error : " + ex.Message);
                        logger.Error("Error : " + ex.StackTrace);
                    }



                    NewMethod(user);

                }



            }
        }

        private void NewMethod(User user)
        {
            SocioBoard.Domain.Team team = (SocioBoard.Domain.Team)Session["GroupName"];

            TeamRepository objTeamRepository = new TeamRepository();
            GroupRepository objGroupRepository = new GroupRepository();
            RssFeedsRepository rssFeedsRepo = new RssFeedsRepository();
            List<RssFeeds> lstrssfeeds = rssFeedsRepo.getAllActiveRssFeeds(user.Id);
            TwitterAccountRepository twtAccountRepo = new TwitterAccountRepository();
            ArrayList arrTwtAcc = twtAccountRepo.getAllTwitterAccountsOfUser(user.Id);
          

            //===================================================================================================================================
           

            //====================================================================================================================================

            if (lstrssfeeds != null)
            {
                if (lstrssfeeds.Count != 0)
                {
                    //int rssCount = 0;
                    string rssData = string.Empty;
                    rssData += "<h2 class=\"league section-ttl rss_header\">Active RSS Feeds</h2>";
                    foreach (RssFeeds item in lstrssfeeds)
                    {
                        TwitterAccount twtAccount = twtAccountRepo.getUserInformation(item.ProfileScreenName, user.Id);
                        string picurl = string.Empty;


                        if (string.IsNullOrEmpty(twtAccount.ProfileUrl))
                        {
                            picurl = "../Contents/img/blank_img.png";

                        }
                        else
                        {
                            picurl = twtAccount.ProfileUrl;

                        }
                        rssData += " <section id=\"" + item.Id + "\" class=\"publishing\">" +
                                "<section class=\"twothird\">" +
                                    "<article class=\"quarter\">" +
                                        "<div href=\"#\" class=\"avatar_link view_profile\" title=\"\">" +
                                            "<img title=\"" + item.ProfileScreenName + "\" src=\"" + picurl + "\" data-src=\"\" class=\"avatar sm\">" +
                                            "<article class=\"rss_ava_icon\"><span title=\"Twitter\" class=\"icon twitter_16\"></span></article>" +
                                        "</div>" +
                                    "</article>" +
                                    "<article class=\"threefourth\">" +
                                        "<ul>" +
                                            "<li>" + item.FeedUrl + "</li>" +
                                            "<li>Prefix: </li>" +
                                            "<li class=\"freq\" title=\"New items from this feed will be posted at most once every hour\">Max Frequency: " + item.Duration + "</li>" +
                                        "</ul>" +
                                    "</article>" +
                                "</section>" +
                                "<section class=\"third\">" +
                                    "<ul class=\"rss_action_buttons\">" +
                                        "<li onclick=\"pauseFunction('" + item.Id + "');\" class=\"\"><a id=\"pause_" + item.Id + "\" href=\"#\" title=\"Pause\" class=\"small_pause icon pause\"></a></li>" +
                                        "<li onclick=\"deleteRssFunction('" + item.Id + "');\" class=\"show-on-hover\"><a id=\"delete_" + item.Id + "\" href=\"#\" title=\"Delete\" class=\"small_remove icon delete\"></a></li>" +
                                    "</ul>" +
                                "</section>" +
                             "</section>";
                    }


                    rss.InnerHtml = rssData;
                    rss.Style.Add("display", "block");
                    rdata.Style.Add("display", "none");

                }
            }
            try
            {
                if (Session["IncomingTasks"] != null)
                {
                    //incom_tasks.InnerHtml = Convert.ToString((int)Session["IncomingTasks"]);
                    //incomTasks.InnerHtml = Convert.ToString((int)Session["IncomingTasks"]);
                }
                else
                {
                    TaskRepository taskRepo = new TaskRepository();
                    ArrayList alst = taskRepo.getAllIncompleteTasksOfUser(user.Id,team.GroupId);
                    Session["IncomingTasks"] = alst.Count;
                }
            }
            catch (Exception es)
            {
                logger.Error(es.StackTrace);
                Console.WriteLine(es.StackTrace);
            }
            if (Session["CountMessages"] != null)
            {
                //incom_messages.InnerHtml = Convert.ToString((int)Session["CountMessages"]);
                //incomMessages.InnerHtml = Convert.ToString((int)Session["CountMessages"]);
            }
            else
            {
                //incom_messages.InnerHtml = "0";
                //incomMessages.InnerHtml = "0";
            }
            //usernm.InnerHtml = "Hello, <a href=\"../Settings/PersonalSettings.aspx\"> " + user.UserName + "</a>";
            usernm.InnerHtml = "Hello, " + user.UserName + "";
            //usernm.InnerHtml = "Hello, <a href=\"../Settings/PersonalSettings.aspx\"> " + user.UserName + "</a>";


            usernm.InnerHtml = "Hello, " + user.UserName + "";
            //usernm.InnerHtml = "Hello, <a href=\"../Settings/PersonalSettings.aspx\"> " + user.UserName + "</a>";
            if (!string.IsNullOrEmpty(user.ProfileUrl))
            {

                //userimg.InnerHtml = "<a href=\"../Settings/PersonalSettings.aspx\"><img id=\"loggeduserimg\" src=\"" + user.ProfileUrl + "\" alt=\"" + user.UserName + "\"/></a>";
                userimg.InnerHtml = "<img id=\"loggeduserimg\" src=\"" + user.ProfileUrl + "\" alt=\"" + user.UserName + "\"/>";
                if (user.TimeZone != null)
                {
                    Datetime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, user.TimeZone).ToLongDateString() + " " + TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, user.TimeZone).ToShortTimeString() + " (" + user.TimeZone + ")";
                    //userinf.InnerHtml = Datetime;
                }
                if (user.TimeZone == null)
                {
                    Datetime = DateTime.Now.ToString();
                    //userinf.InnerHtml = Datetime;
                }
            }
            else
            {
                //userimg.InnerHtml = "<a href=\"../Settings/PersonalSettings.aspx\"><img id=\"loggeduserimg\" src=\"../Contents/img/blank_img.png\" alt=\"" + user.UserName + "\"/></a>";
                userimg.InnerHtml = "<img id=\"loggeduserimg\" src=\"../Contents/img/blank_img.png\" alt=\"" + user.UserName + "\"/>";
                if (user.TimeZone != null)
                {
                    Datetime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, user.TimeZone).ToLongDateString() + " " + TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, user.TimeZone).ToShortTimeString() + " (" + user.TimeZone + ")";
                    //userinf.InnerHtml = Datetime;
                }
                if (user.TimeZone == null)
                {
                    Datetime = DateTime.Now.ToString();
                    //userinf.InnerHtml = Datetime;
                }
            }

            try
            {

                GroupRepository grouprepo = new GroupRepository();
                List<Groups> lstgroups = grouprepo.getAllGroups(user.Id);
                string totgroups = string.Empty;
                if (lstgroups.Count != 0)
                {
                    foreach (Groups item in lstgroups)
                    {
                        totgroups += "<li><a href=\"../Settings/InviteMember.aspx?q=" + item.Id + "\" id=\"group_" + item.Id + "\"><img src=\"../Contents/img/groups_.png\"  alt=\"\"  style=\" margin-right:5px;\"/>" + item.GroupName + "</a></li>";
                    }
                    inviteRedirect.InnerHtml = totgroups;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.StackTrace);
                Console.WriteLine(ex.StackTrace);
            }

        }


        public void AuthenticateFacebook(object sender, EventArgs e)
        {
            string fb = "http://www.facebook.com/dialog/oauth/?scope=publish_stream,read_stream,read_insights,manage_pages,user_checkins,user_photos,read_mailbox,manage_notifications,read_page_mailboxes,email,user_videos,offline_access&client_id=" + ConfigurationManager.AppSettings["ClientId"] + "&redirect_uri=" + ConfigurationManager.AppSettings["RedirectUrl"] + "&response_type=code";
            // fb_cont.HRef = fb_account.HRef;
            Response.Redirect(fb);

        }
        public void AuthenticateTwitter(object sender, EventArgs e)
        {
            oAuthTwitter OAuth = new oAuthTwitter();

            if (Request["oauth_token"] == null)
            {
                OAuth.AccessToken = string.Empty;
                OAuth.AccessTokenSecret = string.Empty;
                OAuth.CallBackUrl = ConfigurationManager.AppSettings["callbackurl"].ToString();
                Response.Redirect(OAuth.AuthorizationLinkGet());
            }

        }

        public void AuthenticateLinkedin(object sender, EventArgs e)
        {
            oAuthLinkedIn Linkedin_oauth = new oAuthLinkedIn();
            string authLink = Linkedin_oauth.AuthorizationLinkGet();
            Session["reuqestToken"] = Linkedin_oauth.Token;
            Session["reuqestTokenSecret"] = Linkedin_oauth.TokenSecret;


            Response.Redirect(authLink);
        }

        public void AuthenticateInstagram(object sender, EventArgs e)
        {
            GlobusInstagramLib.Authentication.ConfigurationIns config = new GlobusInstagramLib.Authentication.ConfigurationIns("https://instagram.com/oauth/authorize/", ConfigurationManager.AppSettings["InstagramClientKey"].ToString(), ConfigurationManager.AppSettings["InstagramClientSec"].ToString(), ConfigurationManager.AppSettings["InstagramCallBackURL"].ToString(), "https://api.instagram.com/oauth/access_token", "https://api.instagram.com/v1/", "");
            oAuthInstagram _api = oAuthInstagram.GetInstance(config);
            Response.Redirect(_api.AuthGetUrl("likes+comments+basic+relationships"));
        }

        public void AuthenticateGooglePlus(object sender, EventArgs e)
        {
            oAuthToken objToken = new oAuthToken();
            Response.Redirect(objToken.GetAutherizationLink("https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/plus.me+https://www.googleapis.com/auth/plus.login"));
        }

        public void fbPage_connect(object sender, EventArgs e)
        {
            try
            {
                Session["fbSocial"] = "p";
                string fbpageconnectClick = "http://www.facebook.com/dialog/oauth/?scope=publish_stream,read_stream,read_insights,manage_pages,user_checkins,user_photos,read_mailbox,manage_notifications,read_page_mailboxes,email,user_videos,offline_access&client_id=" + ConfigurationManager.AppSettings["ClientId"] + "&redirect_uri=" + ConfigurationManager.AppSettings["RedirectUrl"] + "&response_type=code";
                Response.Redirect(fbpageconnectClick);
            }
            catch (Exception Err)
            {
                logger.Error(Err.StackTrace);
                Response.Write(Err.Message.ToString());
            }
        }


        public bool IsUserReferenceActivated(string RefereeId)
        {
            //testing
            Console.WriteLine("Inside " + RefereeId);

            bool ret = false;
            try
            {
                User objUser = new User();
                Package objPackage = new Package();
                UserPackageRelation objUserPackageRelation = new UserPackageRelation();
                UserRepository objUserRepository = new UserRepository();
                UserPackageRelationRepository objUserPackageRelationRepository = new UserPackageRelationRepository();
                UserRefRelation objUserRefRelation = new UserRefRelation();
                UserRefRelationRepository objUserRefRelationRepository = new UserRefRelationRepository();
                PackageRepository objPackageRepository = new PackageRepository();
                objUserRefRelation.ReferenceUserId = (Guid.Parse(RefereeId));

                //testing
                List<UserRefRelation> check = objUserRefRelationRepository.GetUserRefRelationInfo();
                //testing

                List<UserRefRelation> lstUserRefRelation = objUserRefRelationRepository.GetUserRefRelationInfoByRefreeId(objUserRefRelation);
                if (lstUserRefRelation.Count > 0)
                {
                    if (lstUserRefRelation[0].Status == "0")
                    {
                        objUserRefRelation = lstUserRefRelation[0];
                        objUserRefRelation.Status = "1";
                        objUser = objUserRepository.getUsersById(lstUserRefRelation[0].ReferenceUserId);
                        objUser.ExpiryDate = objUser.ExpiryDate.AddDays(30);
                        objUser.AccountType = "Premium";
                        objPackage = objPackageRepository.getPackageDetails("Premium");

                        objUserPackageRelation.Id = Guid.NewGuid();
                        objUserPackageRelation.UserId = objUser.Id;
                        objUserPackageRelation.PackageId = objPackage.Id;
                        objUserPackageRelation.ModifiedDate = DateTime.Now;
                        objUserPackageRelation.PackageStatus = true;
                        objUserPackageRelationRepository.AddUserPackageRelation(objUserPackageRelation);
                        int objUserRepositoryresponse = objUserRepository.UpdateUserExpiryDateById(objUser);
                        int objUserRefRelationRepositoryresponse = objUserRefRelationRepository.UpdateStatusById(objUserRefRelation);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return ret;
        }

        protected void groupsselection_SelectedIndexChanged(object sender, EventArgs e)
        {
            SocioBoard.Domain.User user = (User)Session["LoggedUser"];
            string GroupNames = string.Empty;         
            TeamRepository objTeamRepository = new TeamRepository();
            Team lstDetails = objTeamRepository.getAllGroupsDetails(user.EmailId.ToString(), Guid.Parse(groupsselection.SelectedValue),user.Id);

            Session["GroupName"] = lstDetails;
            Session["groupcheck"] = groupsselection.SelectedValue;
            NewMethod(user);
        }

    }
}