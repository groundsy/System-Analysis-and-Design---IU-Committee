/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: DiscussionController.cs
 * Created by: Eric Grounds
 * Created date: 11-30-12
 * Primary Programmer: Eric Grounds
 * File description: Controller for discussions
 * 
 * Change Log:
 * Date programmer    change
 * 
 * 
*************************************************/﻿

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBananaPhase4.Models;

namespace TeamBananaPhase4.Controllers
{
    public class DiscussionController : Controller
    {
        private jashdownEntities db = new jashdownEntities();

        // POST: /Discussion/AddVote

        [HttpPost]
        [CommitteeMemberVoting]
        public ActionResult AddVote(string vote, int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "", string primaryKey4 = "")
        {
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            DiscItem discItem = db.DiscItem.Find(primaryKey1, primaryKey2, meetingPKDateTime, primaryKey4);
            if (discItem == null)
            {
                return HttpNotFound();
            }

            // Check to see if this discussion item is votable
            if (discItem.IsVoted == "N")
            {
                return RedirectToAction("Details", "DiscItem", new
                {
                    primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                    primaryKey2 = discItem.Meeting_Comm_ID,
                    primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                    primaryKey4 = discItem.Title
                });
            }

            if (ModelState.IsValid)
            {

                Discussion discussion = new Discussion();
                discussion.DiscItem_Meeting_Comm_CommOwn_ID = primaryKey1;
                discussion.DiscItem_Meeting_Comm_ID = primaryKey2;
                discussion.DiscItem_Meeting_DateTime = meetingPKDateTime;
                discussion.DiscItem_Title = primaryKey4;
                discussion.SysUser_Email = User.Identity.Name;

                // Check if the user already voted on this discussion item
                if (db.Discussion.Any(di => di.DiscItem_Meeting_Comm_CommOwn_ID == discussion.DiscItem_Meeting_Comm_CommOwn_ID &&
                                        di.DiscItem_Meeting_Comm_ID == discussion.DiscItem_Meeting_Comm_ID &&
                                        di.DiscItem_Meeting_DateTime == discussion.DiscItem_Meeting_DateTime &&
                                        di.DiscItem_Title == discussion.DiscItem_Title &&
                                        di.SysUser_Email == discussion.SysUser_Email &&
                                        di.HasVoted == "Y"))
                {

                    TempData["voted"] = "Y";
                    return RedirectToAction("Details", "DiscItem", new
                    {
                        primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                        primaryKey2 = discItem.Meeting_Comm_ID,
                        primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                        primaryKey4 = discItem.Title
                    });
                }

                // If voting for this discussion item is anonymous, add vote to AnonVoting table.
                if (discItem.IsAnonVoting == "Y")
                {
                    AnonVoting anonVote = new AnonVoting();
                    anonVote.DiscItem_Title = discItem.Title;
                    anonVote.DiscItem_Meeting_Comm_CommOwn_ID = discItem.Meeting_Comm_CommOwn_ID;
                    anonVote.DiscItem_Meeting_DateTime = discItem.Meeting_DateTime;
                    anonVote.DiscItem_Meeting_Comm_ID = discItem.Meeting_Comm_ID;
                    anonVote.Vote = vote;
                    db.AnonVoting.Add(anonVote);
                    //db.SaveChanges();
                    vote = null;
                }
                else
                {
                    discussion.Vote = vote;
                }

                discussion.ActionDateTime = DateTime.Now;
                discussion.HasVoted = "Y";
                TempData["voted"] = "Y";
                db.Discussion.Add(discussion);
                db.SaveChanges();
                return RedirectToAction("Details", "DiscItem", new
                {
                    primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                    primaryKey2 = discItem.Meeting_Comm_ID,
                    primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                    primaryKey4 = discItem.Title
                });
            }

            //ViewBag.DiscItem_Meeting_Comm_CommOwn_ID = new SelectList(db.DiscItem, "Meeting_Comm_CommOwn_ID", "Description", discussion.DiscItem_Meeting_Comm_CommOwn_ID);
            //ViewBag.SysUser_Email = new SelectList(db.SysUser, "Email", "FirstName", discussion.SysUser_Email);
            return View("Details", "DiscItem", new
            {
                primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                primaryKey2 = discItem.Meeting_Comm_ID,
                primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                primaryKey4 = discItem.Title
            });
        }

        //POST: /Discussion/MarkRead

        [HttpPost]
        [CommitteeMember]
        public ActionResult MarkRead(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "", string primaryKey4 = "")
        {
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            DiscItem discItem = db.DiscItem.Find(primaryKey1, primaryKey2, meetingPKDateTime, primaryKey4);
            if (discItem == null)
            {
                return HttpNotFound();
            }

            if (discItem.IsRead == "N")
            {
                return RedirectToAction("Details", "DiscItem", new
                {
                    primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                    primaryKey2 = discItem.Meeting_Comm_ID,
                    primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                    primaryKey4 = discItem.Title
                });
            }

            if (ModelState.IsValid)
            {

                Discussion discussion = new Discussion();
                discussion.DiscItem_Meeting_Comm_CommOwn_ID = primaryKey1;
                discussion.DiscItem_Meeting_Comm_ID = primaryKey2;
                discussion.DiscItem_Meeting_DateTime = meetingPKDateTime;
                discussion.DiscItem_Title = primaryKey4;
                discussion.SysUser_Email = User.Identity.Name;

                // Check if the user already read this discussion item
                if (db.Discussion.Any(di => di.DiscItem_Meeting_Comm_CommOwn_ID == discussion.DiscItem_Meeting_Comm_CommOwn_ID &&
                                        di.DiscItem_Meeting_Comm_ID == discussion.DiscItem_Meeting_Comm_ID &&
                                        di.DiscItem_Meeting_DateTime == discussion.DiscItem_Meeting_DateTime &&
                                        di.DiscItem_Title == discussion.DiscItem_Title &&
                                        di.SysUser_Email == discussion.SysUser_Email &&
                                        di.HasRead == "Y"))
                {
                    TempData["read"] = "Y";
                    return RedirectToAction("Details", "DiscItem", new
                    {
                        primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                        primaryKey2 = discItem.Meeting_Comm_ID,
                        primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                        primaryKey4 = discItem.Title
                    });
                }

                discussion.ActionDateTime = DateTime.Now;
                discussion.HasRead = "Y";
                db.Discussion.Add(discussion);
                db.SaveChanges();
                TempData["read"] = "Y";
                return RedirectToAction("Details", "DiscItem", new
                {
                    primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                    primaryKey2 = discItem.Meeting_Comm_ID,
                    primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                    primaryKey4 = discItem.Title
                });
            }

            //   ViewBag.DiscItem_Meeting_Comm_CommOwn_ID = new SelectList(db.DiscItem, "Meeting_Comm_CommOwn_ID", "Description", discussion.DiscItem_Meeting_Comm_CommOwn_ID);
            // ViewBag.SysUser_Email = new SelectList(db.SysUser, "Email", "FirstName", discussion.SysUser_Email);
            return View("Details", "DiscItem", new
            {
                primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                primaryKey2 = discItem.Meeting_Comm_ID,
                primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                primaryKey4 = discItem.Title
            });
        }

        //
        // POST: /Discussion/AddComment

        [HttpPost]
        [CommitteeMember]
        public ActionResult AddComment(string comment, int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "", string primaryKey4 = "")
        {
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            DiscItem discItem = db.DiscItem.Find(primaryKey1, primaryKey2, meetingPKDateTime, primaryKey4);
            if (discItem == null)
            {
                return HttpNotFound();
            }

            // Check if comment is not null or whitespace.
            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["invalidComment"] = "Y";
                return RedirectToAction("Details", "DiscItem", new
                {
                    primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                    primaryKey2 = discItem.Meeting_Comm_ID,
                    primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                    primaryKey4 = discItem.Title
                });
            }

            if (ModelState.IsValid)
            {
                Discussion discussion = new Discussion();
                discussion.DiscItem_Meeting_Comm_CommOwn_ID = discItem.Meeting_Comm_CommOwn_ID;
                discussion.DiscItem_Meeting_Comm_ID = discItem.Meeting_Comm_ID;
                discussion.DiscItem_Meeting_DateTime = discItem.Meeting_DateTime;
                discussion.DiscItem_Title = discItem.Title;
                discussion.SysUser_Email = User.Identity.Name;
                discussion.Comment = comment;
                discussion.ActionDateTime = DateTime.Now;

                db.Discussion.Add(discussion);
                db.SaveChanges();
                return RedirectToAction("Details", "DiscItem", new
                {
                    primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                    primaryKey2 = discItem.Meeting_Comm_ID,
                    primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                    primaryKey4 = discItem.Title
                });
            }

            return View("Details", "DiscItem", new
            {
                primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                primaryKey2 = discItem.Meeting_Comm_ID,
                primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                primaryKey4 = discItem.Title
            });

        }
    }
}