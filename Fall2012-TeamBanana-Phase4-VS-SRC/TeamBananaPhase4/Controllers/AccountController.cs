/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: Controllers/AccountController.cs
 * Created by: Jared Short
 * Created date: 11-25-12
 * Primary Programmer: Jared Short
 * File description: Controller for full authentication handling
 * 
 * Change Log:
 * Date programmer
 * 12-3-12 Joel		Added meeting authorizations
 * 
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.Net;
using WebMatrix.WebData;
using TeamBananaPhase4.Filters;
using TeamBananaPhase4.Models;

namespace TeamBananaPhase4.Controllers
{
    //[Authorize]
    //Comment out in favor of different initialize routine
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private jashdownEntities db = new jashdownEntities();
        public string CAS_Server = "https://cas.iu.edu/cas/";
        public string CAS_Method = "GET";
        public string CAS_AppCode = "IU";
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public void Login(string returnUrl)
        {
            Uri MyUrl = Request.Url;
            string returnURL = MyUrl.Scheme + "://" + MyUrl.Host + MyUrl.AbsolutePath + "/Login";

            // Prevent caching, so can't be viewed offline
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            string user = CheckCASLogin();
            if (user == "")
            {
                //redirect to login page
                RedirectToCASLogin(returnURL);
            }
            else
            {
                //user variable has the authenticated login id.
                //FormsAuthentication.RedirectFromLoginPage(user, false, "/SiteFramework/Start.aspx");

                /* NEED TO ADD ALLOWANCE FOR EXTERNAL LOGINS */
                //db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                //db.SaveChanges();

                FormsAuthentication.SetAuthCookie(user, false);
                //true will last forever and false looks at time in web.config (1 will expire when browser closes)
                Response.Redirect("/Divisions/Index", true);
            }
            ViewBag.ReturnUrl = returnUrl;
            //return View();
        }

        public void RedirectToCASLogin(string returnURL)
        {
            // Redirect to CAS for authentication
            string url = CAS_Server + "login?cassvc=" + CAS_AppCode + "&casurl=" + returnURL;
            //Response.Write(url);
            //Response.End();
            Response.Redirect(url, true);
        }

        public string CheckCASLogin()
        {
            string user = "";
            string ticket = "";
            string resp = "";
            string[] respArray;

            if (Session["netid"] != null)
            {
                //Added quick for because there is no username or id field
                user = Session["netid"].ToString() + "@iusb.edu";
            }
            if (user == "")
            {
                // Check to see if we got a CAS ticket
                if (Request.QueryString["casticket"] != null)
                {
                    ticket = Request.QueryString["casticket"].ToString();
                }
                else
                {
                    if (Session["casticket"] != null)
                    {
                        ticket = Session["casticket"].ToString();
                    }
                }

                if (ticket == "")
                {
                    return "";
                }
                else
                {
                    // Validate the ticket
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(CAS_Server + "validate?" + "cassvc=" + Server.UrlEncode(CAS_AppCode) + "&casticket=" + ticket);
                    httpWebRequest.Method = CAS_Method;

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                    resp = streamReader.ReadToEnd();
                    streamReader.Close();

                    respArray = resp.Split((char)13); //resp.Split((char)10);
                    if (respArray[0].ToString().Trim() == "yes")
                    {
                        // Valid ticket, so get the network ID and save it in session
                        Session["casticket"] = ticket;
                        Session["netid"] = respArray[1].ToString().Trim();
                        //Added quick for for no username field
                        return respArray[1].ToString().Trim() + "@iusb.edu";
                    }
                    else
                    {
                        // Sorry, invalid ticket, so they don't get in
                        return "";
                    }
                }
            }
            else
            {
                // return the username
                return user;
            }
        }




        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Divisions");
        }


        //Unauthroized to view a resource
        public ActionResult Unauthorized()
        {
            return View();
        }

    }


    /*Custom Authorization Methods*/

	//returns true if current user is a member of the requested committee
    public class CommitteeMember : AuthorizeAttribute
    {
        private jashdownEntities db = new jashdownEntities();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            //var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
            int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
            int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);

            /* If a committee exist for this user with the passed in parameters grant access */
            if (db.CommMember.Count(c => c.Comm_CommOwn_ID == committeeOwner 
                                    && c.Comm_ID == committeeId
                                    && c.Member_Email == httpContext.User.Identity.Name
                                    && c.StartDate <= DateTime.Today
                                    && c.EndDate >= DateTime.Today) > 0)
                return true;

            /* Otherwise No access */
            return false;
        }
    }

	//returns true if current user is a voting-member of the requested committee
    public class CommitteeMemberVoting : AuthorizeAttribute
    {
        private jashdownEntities db = new jashdownEntities();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            //var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
            int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
            int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);

            /* If a committee exist for this user with the passed in parameters grant access */
            if (db.CommMember.Count(c => c.Comm_CommOwn_ID == committeeOwner
                                    && c.Comm_ID == committeeId
                                    && c.Member_Email == httpContext.User.Identity.Name
                                    && c.StartDate <= DateTime.Today
                                    && c.EndDate >= DateTime.Today
                                    && c.Voting_Non_Voting == "V") > 0)
                return true;

            /* Otherwise No access */
            return false;
        }
    }

	//returns true if current user is a committee admin of the requested committee
    public class CommitteeAdmin : AuthorizeAttribute
    {
        private jashdownEntities db = new jashdownEntities();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            //var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
            int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
            int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);

            /* If a committee exist for this user with the passed in parameters grant access */
            if (db.CommMember.Count(c => c.Comm_CommOwn_ID == committeeOwner
                                    && c.Comm_ID == committeeId
                                    && c.Member_Email == httpContext.User.Identity.Name
                                    && c.StartDate <= DateTime.Today
                                    && c.EndDate >= DateTime.Today
                                    && (c.IsAdministrator == "Y" || c.IsConvener == "Y")) > 0)
                return true;

            /* Otherwise No access */
            return false;
        }
    }

	//returns true if current user is a convener of the requested committee
	public class CommitteeConvener: AuthorizeAttribute
	{
		private jashdownEntities db = new jashdownEntities();

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
				return false;

			//var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
			int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
			int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);

			/* If a committee exist for this user with the passed in parameters grant access */
			if (db.CommMember.Count(c => c.Comm_CommOwn_ID == committeeOwner
									&& c.Comm_ID == committeeId
									&& c.Member_Email == httpContext.User.Identity.Name
									&& c.StartDate <= DateTime.Today
									&& c.EndDate >= DateTime.Today
									&& c.IsConvener == "Y") > 0)
				return true;

			/* Otherwise No access */
			return false;
		}
	}

	//returns true if current user is a committee super admin of the requested division,
	//or if no division is requested, true if the users is a CSA of any division.
    public class CommitteeSuperAdmin : AuthorizeAttribute
    {
        private jashdownEntities db = new jashdownEntities();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;
         
            /* Check if we are validating just as an SCA for Anything, or specific CommOwn */
            if (httpContext.Request.RequestContext.RouteData.Values["primaryKey1"] == null)
            {
                /* If exist this user in the SCA table, they are an SCA for something */
                if (db.CommSuperAdmin.Count(c => c.SysUser_Email == httpContext.User.Identity.Name
                                        && c.StartDate <= DateTime.Today
										&& (c.EndDate ?? DateTime.MaxValue) >= DateTime.Today) > 0)
                    return true;
            }
            else
            {
                //var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
                int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
                //int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);

                /* If exist for this user with the passed committee owner parameters grant access */
                if (db.CommSuperAdmin.Count(c => c.CommOwn_ID == committeeOwner
                                        && c.SysUser_Email == httpContext.User.Identity.Name
                                        && c.StartDate <= DateTime.Today
                                        && (c.EndDate ?? DateTime.MaxValue) >= DateTime.Today) > 0)
                    return true;
            }

            /* Otherwise No access */
            return false;
        }
    }

	//return true if the current user is an IT Administrator.
    public class ITAdmin : AuthorizeAttribute
    {
        private jashdownEntities db = new jashdownEntities();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);

            if (currentUser.IsITAdministrator == "Y")
                return true;

            return false;
        }
    }

	//returns true if current user is a current member of the request meeting's committee
	//and that the meeting is created during the current users current term in office.
	//This is the same as saying that the meeting date and the current date
	//are in the same interval of time. The interval of time is the user's membership in the
	//meeting's committee.
	public class MeetingMember : AuthorizeAttribute
	{
		private jashdownEntities db = new jashdownEntities();

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
				return false;

			//var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
			int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
			int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);
			DateTime meetingDateTime;

			//check if datetime is invalid format returns true so the controller can handle.
			if (!DateTime.TryParse(httpContext.Request.RequestContext.RouteData.Values["primarykey3"].ToString(), out meetingDateTime))
			{
				return true;
			}

			/* Find membership for current user, passed in committee, that spans current date, and passed in date */
			if (db.CommMember.Any(c => c.Comm_CommOwn_ID == committeeOwner
									&& c.Comm_ID == committeeId
									&& c.Member_Email == httpContext.User.Identity.Name
									&& c.StartDate <= DateTime.Today
									&& c.StartDate <= meetingDateTime
									&& c.EndDate >= DateTime.Today
									&& c.EndDate >= meetingDateTime))
				return true;

			/* Otherwise No access */
			return false;
		}
	}

	//returns true if current user is a current committee admin of the request meeting's committee
	//and that the meeting is created during the current users current term in office.
	//This is the same as saying that the meeting date and the current date
	//are in the same interval of time. The interval of time is the user's membership in the
	//meeting's committee.
	public class MeetingAdmin : AuthorizeAttribute
	{
		private jashdownEntities db = new jashdownEntities();

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
				return false;

			//var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
			int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
			int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);
			DateTime meetingDateTime;

			//check if datetime is invalid format returns true so the controller can handle.
			if (!DateTime.TryParse(httpContext.Request.RequestContext.RouteData.Values["primarykey3"].ToString(), out meetingDateTime))
			{
				return true;
			}

			/* Find membership for current user, passed in committee, that spans current date, and passed in date */
			if (db.CommMember.Any(c => c.Comm_CommOwn_ID == committeeOwner
									&& c.Comm_ID == committeeId
									&& c.Member_Email == httpContext.User.Identity.Name
									&& c.StartDate <= DateTime.Today
									&& c.StartDate <= meetingDateTime
									&& c.EndDate >= DateTime.Today
									&& c.EndDate >= meetingDateTime
									&& (c.IsAdministrator == "Y" || c.IsConvener=="Y")))
				return true;

			/* Otherwise No access */
			return false;
		}
	}
	//returns true if current user is a current committee convener of the request meeting's committee
	//and that the meeting is created during the current users current term in office.
	//This is the same as saying that the meeting date and the current date
	//are in the same interval of time. The interval of time is the user's membership in the
	//meeting's committee.
	public class MeetingConvener : AuthorizeAttribute
	{
		private jashdownEntities db = new jashdownEntities();

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
				return false;

			//var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
			int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
			int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);
			DateTime meetingDateTime;

			//check if datetime is invalid format returns true so the controller can handle.
			if (!DateTime.TryParse(httpContext.Request.RequestContext.RouteData.Values["primarykey3"].ToString(), out meetingDateTime))
			{
				return true;
			}

			/* Find membership for current user, passed in committee, that spans current date, and passed in date */
			if (db.CommMember.Any(c => c.Comm_CommOwn_ID == committeeOwner
									&& c.Comm_ID == committeeId
									&& c.Member_Email == httpContext.User.Identity.Name
									&& c.StartDate <= DateTime.Today
									&& c.StartDate <= meetingDateTime
									&& c.EndDate >= DateTime.Today
									&& c.EndDate >= meetingDateTime
									&& c.IsConvener == "Y"))
				return true;

			/* Otherwise No access */
			return false;
		}
	}

	//returns true if current user is a current committee admin of the request meeting's committee
	//and that the meeting is created during the current users current term in office.
	//This is the same as saying that the meeting date and the current date
	//are in the same interval of time. The interval of time is the user's membership in the
	//meeting's committee.
	public class MeetingVotingMember : AuthorizeAttribute
	{
		private jashdownEntities db = new jashdownEntities();

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
				return false;

			//var currentUser = db.SysUser.Find(httpContext.User.Identity.Name);
			int committeeOwner = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey1"]);
			int committeeId = Convert.ToInt32(httpContext.Request.RequestContext.RouteData.Values["primaryKey2"]);
			DateTime meetingDateTime;

			//check if datetime is invalid format returns true so the controller can handle.
			if (!DateTime.TryParse(httpContext.Request.RequestContext.RouteData.Values["primarykey3"].ToString(), out meetingDateTime))
			{
				return true;
			}

			/* Find membership for current user, passed in committee, that spans current date, and passed in date */
			if (db.CommMember.Any(c => c.Comm_CommOwn_ID == committeeOwner
									&& c.Comm_ID == committeeId
									&& c.Member_Email == httpContext.User.Identity.Name
									&& c.StartDate <= DateTime.Today
									&& c.StartDate <= meetingDateTime
									&& c.EndDate >= DateTime.Today
									&& c.EndDate >= meetingDateTime
									&& c.Voting_Non_Voting == "V"))
				return true;

			/* Otherwise No access */
			return false;
		}
	}
}
