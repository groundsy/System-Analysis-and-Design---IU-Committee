/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: AuditLogMeta.cs

 * Created by: Joel Haubold
 * Created date: 11-17-12
 * Primary Programmer: 
 * File description: Contains model validation meta data
 * 
 * Change Log:
 * Date programmer    change
 * 
 * 
*************************************************/
using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;


namespace TeamBananaPhase4.Models
{
	[MetadataType(typeof(AuditLogMeta))]
	public partial class AuditLog
    {
	}

	public class AuditLogMeta
	{
        public int ID { get; set; }
        public string Action { get; set; }
        public System.DateTime ActionDateTime { get; set; }
        public string ActionDescription { get; set; }
        public string SysUser_Email { get; set; }
    }
}
