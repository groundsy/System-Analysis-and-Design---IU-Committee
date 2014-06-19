/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: DiscItemController.cs
 * Created by: Eric Grounds
 * Created date: 11-30-12
 * Primary Programmer: Eric Grounds & Justin Ashdown 
 * File description: Controller for discussion items
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
    public class DiscItemController : Controller
    {
        private jashdownEntities db = new jashdownEntities();

        //
        // GET: /DiscItem/Details/5
        [CommitteeMember]
        public ActionResult Details(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "", string primaryKey4 = "")
        {
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            DiscItem discItem = db.DiscItem.Find(primaryKey1, primaryKey2, meetingPKDateTime, primaryKey4);
            if (discItem == null)
            {
                return HttpNotFound();
            }
            else
            {

                //determine if current user is a current CA of meetings and send result to view
                if (discItem.Meeting.Comm.CommMember.Any(cm => cm.Member_Email == User.Identity.Name &&
                                                 (cm.IsAdministrator == "Y" || cm.IsConvener == "Y") &&
                                                  cm.StartDate <= DateTime.Today &&
                                                  cm.EndDate >= DateTime.Today))
                {
                    ViewBag.isCommitteeAdmin = true;
                }
                else
                {
                    ViewBag.isCommitteeAdmin = false;
                }

                // Check if there is a file for this discuission item
                DiscItemDocument discItemDoc = new DiscItemDocument();
                discItemDoc = db.DiscItemDocument.Find(discItem.Meeting_Comm_CommOwn_ID, discItem.Meeting_Comm_ID, discItem.Meeting_DateTime, discItem.Title);

                if (discItemDoc != null && discItem.DiscItemDocument.FileImage.Length > 0)
                {
                    ViewBag.fileExists = true;
                    ViewBag.fileName = discItem.DiscItemDocument.Filename;
                }
                else
                {
                    ViewBag.fileExists = false;
                }

				//check if user is voting member
				if (discItem.Meeting.Comm.CommMember.Any(cm => cm.Member_Email == User.Identity.Name &&
 												  cm.Voting_Non_Voting == "V" &&
 												  cm.StartDate <= DateTime.Today &&
 												  cm.EndDate >= DateTime.Today))
 				{
 					ViewBag.isVotingMember = true;
 				}
 				else
 				{
 					ViewBag.isVotingMember = false;
 				}
                // Get a list of all discussion associated with this discussion item
                List<Discussion> discItemDisc = new List<Discussion>();
                discItemDisc = discItem.Discussion.ToList();

                ViewBag.voted = "N";
                if (discItem.IsVoted == "Y")
                {
                    // Check if the user voted on this discussion item
                    foreach (var disc in discItemDisc)
                    {
                        if (disc.SysUser_Email == User.Identity.Name &&
                            disc.HasVoted == "Y")
                        {
                            ViewBag.voted = "Y";
                            break;
                        }
                    }
                }

                ViewBag.read = "N";
                if (discItem.IsRead == "Y")
                {
                    // Check if the user read this discussion item
                    foreach (var disc in discItemDisc)
                    {
                        if (disc.DiscItem_Meeting_Comm_CommOwn_ID == discItem.Meeting_Comm_CommOwn_ID &&
                            disc.DiscItem_Meeting_Comm_ID == discItem.Meeting_Comm_ID &&
                            disc.DiscItem_Meeting_DateTime == discItem.Meeting_DateTime &&
                            disc.DiscItem_Title == discItem.Title &&
                            disc.SysUser_Email == User.Identity.Name &&
                            disc.HasRead == "Y")
                        {
                            ViewBag.read = "Y";
                            break;
                        }
                    }
                }

                ViewBag.votetypes = new SelectList(discItem.VoteType, "Type", "Type");
                ViewBag.votable = discItem.IsVoted;
                ViewBag.readable = discItem.IsRead;
                ViewBag.IsAnonVoting = discItem.IsAnonVoting;
                return View(discItem);
            }
        }

        //
        // GET: /DiscItem/Create
        [CommitteeAdmin]
        public ActionResult Create(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "")
        {
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            Meeting meeting = db.Meeting.Find(primaryKey1, primaryKey2, meetingPKDateTime);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            DiscItem newDiscItem = new DiscItem();
            newDiscItem.Meeting_Comm_CommOwn_ID = primaryKey1;
            newDiscItem.Meeting_Comm_ID = primaryKey2;
            newDiscItem.Meeting_DateTime = meeting.DateTime;
            newDiscItem.DueDate = DateTime.Now.Date;
            newDiscItem.CreatedBy = HttpContext.User.Identity.Name;
            newDiscItem.CreatedDate = DateTime.Now;
            newDiscItem.IsArchived = "N";
            ViewBag.MeetingTime = db.Meeting.Find(meeting.Comm_CommOwn_ID, meeting.Comm_ID, meeting.DateTime).DateTime;
            ViewData["voteTypes"] = new SelectList(db.VoteType, "Type", "Type").ToList();

            return View(newDiscItem);
        }

        //
        // POST: /DiscItem/Create
        [HttpPost]
        [CommitteeAdmin]
        public ActionResult Create(DiscItem discItem, String Tags, string[] voteTypes)
        {
            discItem.CreatedDate = DateTime.Now;
            var di = db.DiscItem.Include(d => d.Meeting).ToList();

            if (ModelState.IsValid)
            {
                db.DiscItem.Add(discItem);

                // Add vote types to DiscItemVoteType table if discussion item is votable.
                if (discItem.IsVoted == "Y")
                {
                    if (voteTypes == null)
                    {
                        ModelState.AddModelError("VoteType", "You must select the vote types");
                        ViewData["voteTypes"] = new SelectList(db.VoteType, "Type", "Type");
                        ViewBag.MeetingTime = discItem.Meeting_DateTime;
                        return View(discItem);
                    }

                    VoteType newVoteType;
                    foreach (string vt in voteTypes)
                    {
                        newVoteType = new VoteType { Type = vt };
                        db.VoteType.Attach(newVoteType);
                        discItem.VoteType.Add(newVoteType);
                    }
                }

                // Check if the title is valid
                if (db.DiscItem.Any(disc => disc.Meeting_Comm_CommOwn_ID == discItem.Meeting_Comm_CommOwn_ID &&
                                        disc.Meeting_Comm_ID == discItem.Meeting_Comm_ID &&
                                        disc.Meeting_DateTime == discItem.Meeting_DateTime &&
                                        disc.Title == discItem.Title))
                {

                    ModelState.AddModelError("Title", "A discussion item already exists with this title");
                    ViewData["voteTypes"] = new SelectList(db.VoteType, "Type", "Type");
                    return View(discItem);
                }


                HttpPostedFileBase file = Request.Files[0];

                DiscItemDocument discItemDoc = new DiscItemDocument();

                if (file.ContentLength > 0) // checks if file was uploaded to the form
                {
                    DateTime discItemDateTime = discItem.Meeting_DateTime;

                    // sets the PKs of the disc Item Document
                    discItemDoc.DiscItem_Meeting_Comm_CommOwn_ID = discItem.Meeting_Comm_CommOwn_ID;
                    discItemDoc.DiscItem_Meeting_Comm_ID = discItem.Meeting_Comm_ID;
                    discItemDoc.DiscItem_Meeting_DateTime = discItemDateTime;
                    discItemDoc.DiscItem_Title = discItem.Title;

                    // getting size of file
                    int fileLen = file.ContentLength;

                    // create write buffer
                    byte[] byteFile = new byte[fileLen];
                    file.InputStream.Read(byteFile, 0, fileLen);

                    // write file to discItemDoc
                    discItemDoc.FileImage = byteFile;
                    discItemDoc.Filename = file.FileName;
                    discItemDoc.ContentType = file.ContentType;

                    // gets tags of file
                    discItemDoc.Tags = Tags;
                }
                
                if (discItemDoc != null && file.ContentLength > 0)
                {
                    db.DiscItemDocument.Add(discItemDoc);
                }
                
                db.SaveChanges();

                return RedirectToAction("Details", "DiscItem", new
                {
                    primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                    primaryKey2 = discItem.Meeting_Comm_ID,
                    primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                    primaryKey4 = discItem.Title
                });
                
            }


            ViewData["voteTypes"] = new SelectList(db.VoteType, "Type", "Type");
            ViewBag.MeetingTime = db.Meeting.Find(discItem.Meeting.Comm_CommOwn_ID, discItem.Meeting.Comm_ID, discItem.Meeting.DateTime).DateTime;
            return View("Details", "DiscItem", new
            {
                primaryKey1 = discItem.Meeting_Comm_CommOwn_ID,
                primaryKey2 = discItem.Meeting_Comm_ID,
                primaryKey3 = discItem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                primaryKey4 = discItem.Title
            });
        }


        //
        // GET: /DiscItem/Edit/5
        [CommitteeAdmin]
        public ActionResult Edit(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "", string primaryKey4 = "")
        {
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            string discItemPKTitle = HttpUtility.UrlDecode(primaryKey4);

            DiscItem discItem = db.DiscItem.Find(primaryKey1, primaryKey2, meetingPKDateTime, discItemPKTitle);
            if (discItem == null)
            {
                return HttpNotFound();
            }

            ViewData["voteTypes"] = new SelectList(db.VoteType, "Type", "Type");
            ViewBag.Category = new SelectList(db.Category, "Type", "Description");

            ViewData["voteTypes"] = new SelectList(db.VoteType, "Type", "Type");
            //Find if a discussion item document exists for this discussion item
            DiscItemDocument discItemDoc = new DiscItemDocument();
            discItemDoc = db.DiscItemDocument.Find(discItem.Meeting_Comm_CommOwn_ID, discItem.Meeting_Comm_ID, discItem.Meeting_DateTime, discItem.Title);

            if (discItemDoc != null && discItem.DiscItemDocument.FileImage.Length > 0)
            {
                Session["fileImage"] = discItem.DiscItemDocument.FileImage;
                Session["contentType"] = discItem.DiscItemDocument.ContentType;
                Session["fileLength"] = discItem.DiscItemDocument.FileImage.Length;
            }
            return View(discItem);
        }

        //
        // POST: /DiscItem/Edit/5
        [CommitteeAdmin]
        [HttpPost]
        public ActionResult Edit(DiscItem discitem, String isCommDocument, String comDocTitle, String category, string[] voteTypes)
        {
            HttpPostedFileBase file = Request.Files[0];

            int oldFileLength = (int)Session["fileLength"];
            byte[] oldFileImage = new byte[oldFileLength];
            oldFileImage = (byte[])Session["fileImage"];

            if (file.ContentLength > 0) // checks if file was uploaded to the form, maked it the new document
            {
                // getting size of file
                int fileLen = file.ContentLength;

                // create write buffer
                byte[] byteFile = new byte[fileLen];
                file.InputStream.Read(byteFile, 0, fileLen);

                // write file to discItemDoc
                discitem.DiscItemDocument.FileImage = byteFile;
                discitem.DiscItemDocument.Filename = file.FileName;
                discitem.DiscItemDocument.ContentType = file.ContentType;

                discitem.DiscItemDocument.DiscItem_Title = discitem.Title;
                discitem.DiscItemDocument.DiscItem_Meeting_Comm_CommOwn_ID = discitem.Meeting_Comm_CommOwn_ID;
                discitem.DiscItemDocument.DiscItem_Meeting_Comm_ID = discitem.Meeting_Comm_ID;
                discitem.DiscItemDocument.DiscItem_Meeting_DateTime = discitem.Meeting_DateTime;
            }
            else if (oldFileImage != null && oldFileImage.Length > 0) // keeps old document
            {
                //discitem.DiscItemDocument.Filename = file.FileName;
                discitem.DiscItemDocument.FileImage = (byte[])Session["fileImage"];
                discitem.DiscItemDocument.ContentType = (string)Session["contentType"];

                discitem.DiscItemDocument.DiscItem_Title = discitem.Title;
                discitem.DiscItemDocument.DiscItem_Meeting_Comm_CommOwn_ID = discitem.Meeting_Comm_CommOwn_ID;
                discitem.DiscItemDocument.DiscItem_Meeting_Comm_ID = discitem.Meeting_Comm_ID;
                discitem.DiscItemDocument.DiscItem_Meeting_DateTime = discitem.Meeting_DateTime;
            }
            else // there was and still is no document for this discussion
            {
                discitem.DiscItemDocument = null;
            }

            
            

            CommDocument commDoc = new CommDocument();

            bool commDocumentAdded = false;

            

            // clears out the sessions, data in session no loner needed
            Session["fileImage"] = null;
            Session["contentType"] = null;
            Session["fileLength"] = null;

            if (isCommDocument != "N")
            {
                commDoc.Comm_CommOwn_ID = discitem.Meeting_Comm_CommOwn_ID;
                commDoc.Comm_ID = discitem.Meeting_Comm_ID;
                commDoc.Title = comDocTitle;
                commDoc.DisplayDate = DateTime.Now;
                commDoc.Tags = discitem.DiscItemDocument.Tags;
                commDoc.Filename = discitem.DiscItemDocument.Filename;
                commDoc.FileImage = discitem.DiscItemDocument.FileImage;
                commDoc.UploadedBy = User.Identity.Name;
                commDoc.UploadedDate = DateTime.Now;
                commDoc.IsArchived = "N";
                commDoc.ArchivedBy = null;
                commDoc.ArchivedDate = null;
                commDoc.ContentType = discitem.DiscItemDocument.ContentType;

                if (db.CommDocument.Find(discitem.Meeting_Comm_CommOwn_ID, discitem.Meeting_Comm_ID, comDocTitle) != null)
                {
                    ModelState.AddModelError("docTitle", "A committee document already exists with that title.");

                    ViewBag.Title = new SelectList(db.DiscItem, "Title", discitem.Title);
                    ViewBag.Meeting_Comm_CommOwn_ID = new SelectList(db.Meeting, "Comm_CommOwn_ID", "Location", discitem.Meeting_Comm_CommOwn_ID);
                    ViewBag.CreatedBy = new SelectList(db.SysUser, "Email", "FirstName", discitem.CreatedBy);
                    ViewBag.Category = new SelectList(db.Category, "Type", "Description");

                    Session["fileImage"] = discitem.DiscItemDocument.FileImage;
                    Session["contentType"] = discitem.DiscItemDocument.ContentType;
                    Session["fileLength"] = discitem.DiscItemDocument.FileImage.Length;
                    return View(discitem);
                }

                if (category == "")
                {
                    ModelState.AddModelError("Category", "Select a category");

                    ViewBag.Title = new SelectList(db.DiscItem, "Title", discitem.Title);
                    ViewBag.Meeting_Comm_CommOwn_ID = new SelectList(db.Meeting, "Comm_CommOwn_ID", "Location", discitem.Meeting_Comm_CommOwn_ID);
                    ViewBag.CreatedBy = new SelectList(db.SysUser, "Email", "FirstName", discitem.CreatedBy);
                    ViewBag.Category = new SelectList(db.Category, "Type", "Description");

                    Session["fileImage"] = discitem.DiscItemDocument.FileImage;
                    Session["contentType"] = discitem.DiscItemDocument.ContentType;
                    Session["fileLength"] = discitem.DiscItemDocument.FileImage.Length;
                    return View(discitem);
                }

                commDoc.Category = category;

                commDocumentAdded = true;

                if (isCommDocument == "Ypub")
                {
                    commDoc.IsPublic = "Y";
                    commDoc.IsProtected = "N";
                }
                else
                {
                    commDoc.IsPublic = "N";
                    commDoc.IsProtected = "Y";
                }
            }

            // for testing, this need to be true and it is not
            bool lookAtModelStateValid = ModelState.IsValid;

            //if (ModelState.IsValid)
            //{
                db.Entry(discitem).State = EntityState.Modified;


                if (discitem.DiscItemDocument != null)
                {
                    if (oldFileImage == null)
                        db.DiscItemDocument.Add(discitem.DiscItemDocument);
                    else
                        db.Entry(discitem.DiscItemDocument).State = EntityState.Modified;
                }
                
                if (commDocumentAdded)
                {
                    db.CommDocument.Add(commDoc);
                }

                db.SaveChanges();

                return RedirectToAction("Details", "DiscItem", new
                {
                    primaryKey1 = discitem.Meeting_Comm_CommOwn_ID,
                    primaryKey2 = discitem.Meeting_Comm_ID,
                    primaryKey3 = discitem.Meeting_DateTime.ToString("MM-dd-yyyy h.mm.ss t\\M"),
                    primaryKey4 = discitem.Title
                });
            //}

            //ViewBag.Title = new SelectList(db.DiscItemDocument, "DiscItem_Meeting_Comm_CommOwn_ID", "Filename", discitem.Title);
            //ViewBag.Meeting_Comm_CommOwn_ID = new SelectList(db.Meeting, "Comm_CommOwn_ID", "Location", discitem.Meeting_Comm_CommOwn_ID);
            //ViewBag.CreatedBy = new SelectList(db.SysUser, "Email", "FirstName", discitem.CreatedBy);
            //ViewBag.Category = new SelectList(db.Category, "Type", "Description");

            //Session["fileImage"] = discitem.DiscItemDocument.FileImage;
            //Session["contentType"] = discitem.DiscItemDocument.ContentType;

            //return View(discitem);
        }


        // POST: /DiscItem/Archive/5
        [CommitteeAdmin]
        public ActionResult Archive(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "", string primaryKey4 = "")
        {
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            DiscItem discitem = db.DiscItem.Find(primaryKey1, primaryKey2, meetingPKDateTime, primaryKey4);

            if (discitem == null)
            {
                return HttpNotFound();
            }

            discitem.IsArchived = "Y";
            db.Entry(discitem).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Meetings", new { primaryKey1 = primaryKey1, primaryKey2 = primaryKey2, primaryKey3 = primaryKey3 });
        }

        [CommitteeMember]
        public ActionResult Download(int primaryKey1 = 0, int primaryKey2 = 0, string primaryKey3 = "", string primaryKey4 = "")
        {
            DateTime meetingPKDateTime;
            if (!DateTime.TryParse(primaryKey3, out meetingPKDateTime))
                return HttpNotFound();

            DiscItemDocument discitemdoc = db.DiscItemDocument.Find(primaryKey1, primaryKey2, meetingPKDateTime, primaryKey4);

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = discitemdoc.Filename,

                // the browser always to try to show the file inline
                // set to false if you want to always prompt the user for downloading,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(discitemdoc.FileImage, discitemdoc.ContentType);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}