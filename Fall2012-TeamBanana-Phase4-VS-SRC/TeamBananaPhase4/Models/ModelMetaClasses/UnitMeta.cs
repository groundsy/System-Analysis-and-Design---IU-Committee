/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: UnitMeta.cs
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
	[MetadataType(typeof(UnitMeta))]
    public partial class Unit
    {

    }

	public class UnitMeta
	{
		public string Campus_University_Code { get; set; }
		public string Campus_Code { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public int CommOwn_ID { get; set; }
		public int Employer_ID { get; set; }
		public string School_Campus_University_Code { get; set; }
		public string School_Campus_Code { get; set; }
		public string School_Code { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }

		public virtual Campus Campus { get; set; }
		public virtual CommOwn CommOwn { get; set; }
		public virtual Employer Employer { get; set; }
		public virtual School School { get; set; }
		public virtual SysUser SysUser { get; set; }
	}
}
