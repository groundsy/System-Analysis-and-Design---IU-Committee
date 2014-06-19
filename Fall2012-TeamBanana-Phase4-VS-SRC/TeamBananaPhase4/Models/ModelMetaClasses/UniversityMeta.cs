/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: UniversityMeta.cs
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
	using System;
	using System.Collections.Generic;

	[MetadataType(typeof(UniversityMeta))]
	public partial class University
	{
	}

	public class UniversityMeta
	{
		public string Code { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public int CommOwn_ID { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }
		public virtual ICollection<Campus> Campus { get; set; }
		public virtual CommOwn CommOwn { get; set; }
		public virtual SysUser SysUser { get; set; }
	}
}
