/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: CategoryMeta.cs
 * Created by: Jared Short
 * Created date: 11-30-12
 * Primary Programmer: 
 * File description: Contains complete generation logic for dynamic menu
 * 
 * Change Log:
 * Date programmer    change
 * 
 * 
*************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBananaPhase4.Models;
namespace TeamBananaPhase4.Models
{
    public static class HtmlHelperSiteMenu
    {

        public static MvcHtmlString SiteMenu(this HtmlHelper helper)
        {
            jashdownEntities db = new jashdownEntities();

            //If the user is not authenticated, then return basic menu
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return MvcHtmlString.Create(@"<ul class='nav'>
                      <li>
                        <a href='/Search'><i class='icon-search icon-white'></i> Search</a>
                      </li>
                    </ul>");
            }
            else //Otherwise build dynamic menu for user
            {
                //Get all committees that user is a current member of
                var currentUserMemberships = db.CommMember.Where(c => c.Member_Email == HttpContext.Current.User.Identity.Name
                                                                 && c.StartDate <= DateTime.Today
                                                                 && c.EndDate >= DateTime.Today);

                string committeesUl = "<ul class='sub-menu'>";
                foreach(var membership in currentUserMemberships)
                {
                    Comm currentCommittee = db.Comm.Find(membership.Comm_CommOwn_ID,membership.Comm_ID);
                    if(currentCommittee.IsArchived != "Y")
                        committeesUl += "<li><a href=\"/Committees/Details/" + currentCommittee.CommOwn_ID + "/" + currentCommittee.ID + "\">"  + currentCommittee.Name + "</a></li>";
                }
                committeesUl += "</ul>";



                /* Routine for creating Super Admin Menu */
                var currentSuperAdmin = db.CommSuperAdmin.Where(c => c.SysUser_Email == HttpContext.Current.User.Identity.Name
                                                                && c.StartDate <= DateTime.Today
                                                                && (c.EndDate == null || c.EndDate >= DateTime.Today));

                string superAdminUl = "";
                if(currentSuperAdmin.Count() > 0)
                {
                    superAdminUl = "<li class='dropper'><a href=''><i class='icon-cog icon-white'></i>Division Admin</a><ul class='sub-menu'>";
                    foreach(var superAdmin in currentSuperAdmin)
                    {
                        var unitFound = db.Unit.FirstOrDefault(c => c.CommOwn_ID == superAdmin.CommOwn_ID);
                        if(unitFound != null)
                        {
                            superAdminUl += "<li><a href=\"/Divisions/Index/" + unitFound.CommOwn_ID + "\">" + unitFound.Name + "</a></li>";
                        }

                        var schoolFound = db.School.FirstOrDefault(c => c.CommOwn_ID == superAdmin.CommOwn_ID);
                        if(schoolFound != null)
                        {
                            superAdminUl += "<li><a href=\"/Divisions/Index/" + schoolFound.CommOwn_ID +"\">"  + schoolFound.Name + "</a></li>";
                        }

                        var campusFound = db.Campus.FirstOrDefault(c => c.CommOwn_ID == superAdmin.CommOwn_ID);
                        if(campusFound != null)
                        {
                            superAdminUl += "<li><a href=\"/Divisions/Index/" + campusFound.CommOwn_ID + "\">" + campusFound.Name + "</a></li>";
                        }

                        var universityFound = db.University.FirstOrDefault(c => c.CommOwn_ID == superAdmin.CommOwn_ID);
                        if(universityFound != null)
                        {
                            superAdminUl += "<li><a href=\"/Divisions/Index/" + universityFound.CommOwn_ID + "\">" + universityFound.Name + "</a></li>";
                        }
                    
                    }
                    superAdminUl += "</ul></li>";

                }


                /* Routine for IT Admin */



                return MvcHtmlString.Create(@"<ul class='nav'>
                       
                      <li class='dropper'>
                        <a href=''><i class='icon-th icon-white'></i> Committees</a>
                        " + committeesUl + @"
                      </li> 
                        " + superAdminUl + @"
                      <!-- Currently Commented Out <li>
                        <a href=''><i class='icon-font icon-white'></i> Meetings</a>
                      </li>-->

                      <li>
                        <a href='/reports'><i class='icon-retweet icon-white'></i> Reports</a>
                      </li>
					  <li>
                        <a href='/search'><i class='icon-search icon-white'></i> Search</a>
                      </li>
                    </ul>");
            }
        }
    }
}
