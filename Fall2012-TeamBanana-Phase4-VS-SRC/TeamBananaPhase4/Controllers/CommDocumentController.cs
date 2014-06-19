using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBananaPhase4.Models;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;

namespace TeamBananaPhase4.Controllers
{
	public class CommDocumentController : Controller
	{
		private jashdownEntities db = new jashdownEntities();

		//
		// GET: /CommDocument/Details/5/2/string
        [CommitteeMember]
		public ActionResult Details(int primaryKey1 = 0, int primaryKey2 = 0, String primaryKey3 = "")
		{
			CommDocument commdocument = db.CommDocument.Find(primaryKey1, primaryKey2, primaryKey3);
			if (commdocument == null || commdocument.IsArchived == "Y")
			{
				return HttpNotFound();
			}

            ViewBag.documentDate = commdocument.DisplayDate.ToString("MMMM dd, yyyy");

            string viewtheviewbag = ViewBag.documentDate;

            //determine if current user is a current CA of documents and send result to view
            if (commdocument.Comm.CommMember.Any(cm => cm.Member_Email == User.Identity.Name &&
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

            return View(commdocument);
		}

        // Receives primary key of Committee that the new document will belong to
		// GET: /CommDocument/Create/2/3
        [CommitteeAdmin]
        public ActionResult Create(int primaryKey1 = -1, int primaryKey2 = -1)
		{
            //find committee that this new document will belong to
            Comm comm = db.Comm.Find(primaryKey1, primaryKey2);
            if (comm == null)
            {
                return HttpNotFound();
            }

            CommDocument commdocument = new CommDocument();
            
            commdocument.Comm_CommOwn_ID = primaryKey1;
            commdocument.Comm_ID = primaryKey2;
            commdocument.UploadedBy = User.Identity.Name;
            commdocument.IsArchived = "N";
            commdocument.ArchivedBy = null;
            commdocument.ArchivedDate = null;
            commdocument.DisplayDate = DateTime.Now.Date;

			ViewBag.Category = new SelectList(db.Category, "Type", "Description");
            
            return View(commdocument);
		}

		//
		// POST: /CommDocument/Create

		[HttpPost]
		public ActionResult Create(CommDocument commdocument)
		{
            commdocument.UploadedDate = DateTime.Now;

            HttpPostedFileBase file = Request.Files["myFile"];

            //make sure no duplicate document exists.
            if (db.CommDocument.Find(commdocument.Comm_CommOwn_ID, commdocument.Comm_ID, commdocument.Title) != null)
            {
                ModelState.AddModelError("Title", "A document already exists with this title.");
                ViewBag.Category = new SelectList(db.Category, "Type", "Description", commdocument.Category);

                return View(commdocument);
            }
            else if (file.ContentLength > 0) // checks if file was uploaded to the form
            {
                // set protected to no if public, protected to yes if not public
                if (commdocument.IsPublic == "Y")
                {
                    commdocument.IsProtected = "N";
                }
                else if (commdocument.IsPublic == "N")
                {
                    commdocument.IsProtected = "Y";
                }

                // getting size of file
                int fileLen = file.ContentLength;

                // create write buffer
                byte[] byteFile = new byte[fileLen];
                file.InputStream.Read(byteFile, 0, fileLen);

                // write file to commdocument
                commdocument.FileImage = byteFile;
                commdocument.Filename = file.FileName;
                commdocument.ContentType = file.ContentType;

                if (ModelState.IsValid)
                {
                    db.CommDocument.Add(commdocument);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Committees", new { primaryKey1 = commdocument.Comm_CommOwn_ID, primaryKey2 = commdocument.Comm_ID });
                }

                ViewBag.Category = new SelectList(db.Category, "Type", "Description", commdocument.Category);
                
                return View(commdocument);
            }
            
			// No file
            ViewBag.Category = new SelectList(db.Category, "Type", "Description", commdocument.Category);

            ModelState.AddModelError("FileImage", "Select a file to upload");
            
            return View(commdocument);
	
		}

		//
		// GET: /CommDocument/Edit/5
        [CommitteeAdmin]
		public ActionResult Edit(int primaryKey1 = -1, int primaryKey2 = -1, String primaryKey3 = "")
		{
			CommDocument commdocument = db.CommDocument.Find(primaryKey1, primaryKey2, primaryKey3);
            if (commdocument == null || commdocument.IsArchived == "Y")
			{
				return HttpNotFound();
			}

            // puts current fileImage and content type into a session, so it can be retreived if user leave file unchanged
            Session["fileImage"] = commdocument.FileImage;
            Session["contentType"] = commdocument.ContentType;

            // stores if document is already private, to make sure it cant be changed then
            Session["isPublic"] = (string)commdocument.IsPublic;

			ViewBag.Category = new SelectList(db.Category, "Type", "Description", commdocument.Category);

			return View(commdocument);
		}

        //
		// POST: /CommDocument/Edit/5

		[HttpPost]
		public ActionResult Edit(CommDocument commdocument)
		{
            HttpPostedFileBase file = Request.Files["myFile"];

            if ( (string)Session["isPublic"] == "Y" && commdocument.IsPublic == "N")
            {
                ModelState.AddModelError("IsPublic", "Can not make a public document protected");

                ViewBag.Category = new SelectList(db.Category, "Type", "Description", commdocument.Category);

                return View(commdocument);
            }

            switch (commdocument.IsPublic)
            {
                case "N":
                    commdocument.IsPublic = "N";
                    commdocument.IsProtected = "Y";
                    commdocument.IsArchived = "N";
                    break;
                case "A":
                    commdocument.IsPublic = "N";
                    commdocument.IsProtected = "N";
                    commdocument.IsArchived = "Y";
                    break;
                case "Y":
                    commdocument.IsPublic = "Y";
                    commdocument.IsProtected = "N";
                    commdocument.IsArchived = "N";
                    break;
            }

            if (file.ContentLength > 0) // checks if file was uploaded to the form
            {
                // getting size of file
                int fileLen = file.ContentLength;

                // create write buffer
                byte[] byteFile = new byte[fileLen];
                file.InputStream.Read(byteFile, 0, fileLen);

                // write file to commdocument, replacing old file
                commdocument.FileImage = byteFile;
                commdocument.Filename = file.FileName;
                commdocument.ContentType = file.ContentType;
            }
            else
            {
                // put old document back and leave unchanged
                commdocument.FileImage = (byte[])Session["fileImage"];
                commdocument.ContentType = (string)Session["contentType"];
            }

            // clears out session data, no longer needed
            Session["fileImage"] = null;
            Session["contentType"] = null;
            Session["isPublic"] = null;
            
            // sets who archived and archive time, if document is flagged for archive by user
            if(commdocument.IsArchived == "Y")
            {
                commdocument.ArchivedBy = User.Identity.Name;
                commdocument.ArchivedDate = DateTime.Now;
            }

            

            if (ModelState.IsValid)
            {
                db.Entry(commdocument).State = EntityState.Modified;
                db.SaveChanges();
                AuditLogController.Add("Committee Document Edit", User.Identity.Name, "Committee Document: " + commdocument.Title + " in committee " + commdocument.Comm_CommOwn_ID + " - " + commdocument.Comm_ID + "was edited");
                return RedirectToAction("Details", "Committees", new { primaryKey1 = commdocument.Comm_CommOwn_ID, primaryKey2 = commdocument.Comm_ID });
            }


            // something went wrong, return to edit page
			ViewBag.Category = new SelectList(db.Category, "Type", "Description", commdocument.Category);
			
			return View(commdocument);
		}

		//
		// GET: /CommDocument/Delete/5
        [CommitteeAdmin]
        public ActionResult Delete(int primaryKey1 = 0, int primaryKey2 = 0, String primaryKey3 = "")
		{
			CommDocument commdocument = db.CommDocument.Find(primaryKey1, primaryKey2, primaryKey3);
			if (commdocument == null || commdocument.IsArchived == "Y")
			{
				return HttpNotFound();
			}
		    return View(commdocument);
		}

		//
		// POST: /CommDocument/Delete/5

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int primaryKey1, int primaryKey2, string primaryKey3)
		{
			CommDocument commdocument = db.CommDocument.Find(primaryKey1, primaryKey2, primaryKey3);
			db.CommDocument.Remove(commdocument);
			db.SaveChanges();

            AuditLogController.Add("Delete Committee Document", User.Identity.Name, "Committee Document: " + commdocument.Title + " in committee " + commdocument.Comm_CommOwn_ID + " - " + commdocument.Comm_ID + "was edited");

            return RedirectToAction("Details", "Committees", new { primaryKey1 = commdocument.Comm_CommOwn_ID, primaryKey2 = commdocument.Comm_ID });
		}

        
        public ActionResult Download(int primaryKey1 = 0, int primaryKey2 = 0, String primaryKey3 = "")
        {
            CommDocument commdocument = db.CommDocument.Find(primaryKey1, primaryKey2, primaryKey3);

            // prevents download of archived documents and non-public documents by non-committee members
            if (commdocument.IsArchived == "Y" || (commdocument.IsPublic == "N" && !(commdocument.Comm.CommMember.Any(cm => cm.Member_Email == User.Identity.Name &&                                                 
                                                  cm.StartDate <= DateTime.Today &&
                                                  cm.EndDate >= DateTime.Today))))
                return HttpNotFound();
            
            var cd = new System.Net.Mime.ContentDisposition{                            
                            // for example foo.bak
                            FileName = commdocument.Filename, 
                        
                            // the browser always to try to show the file inline
                            // set to false if you want to always prompt the user for downloading, 
                            Inline = true, 
                            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(commdocument.FileImage, commdocument.ContentType);
        }

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}