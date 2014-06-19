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
    public class AuditLogController : Controller
    {
        

        public static void Add(string action, string sysUser_Email, string actionDescription = "")
        {
            jashdownEntities db = new jashdownEntities();
            var auditlog = db.AuditLog.Include(a => a.SysUser);
            AuditLog newAuditLog = new AuditLog();
            newAuditLog.Action = action;
            newAuditLog.ActionDescription = actionDescription;
            newAuditLog.SysUser_Email = sysUser_Email;
            newAuditLog.ActionDateTime = DateTime.Now;
            db.AuditLog.Add(newAuditLog);
            db.SaveChanges();
            db.Dispose();
        }

    }
}