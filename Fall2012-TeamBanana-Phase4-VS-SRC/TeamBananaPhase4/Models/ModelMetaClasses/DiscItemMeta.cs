/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: DiscItemMeta.cs
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
	[MetadataType(typeof(DiscItemMeta))]
	public partial class DiscItem
	{
	}
	public class DiscItemMeta
	{

		public int Meeting_Comm_CommOwn_ID { get; set; }
		public int Meeting_Comm_ID { get; set; }
		public System.DateTime Meeting_DateTime { get; set; }
		
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
		
        public string Decision { get; set; }
		public System.DateTime DueDate { get; set; }

        public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }

        [Required]
        public string IsVoted { get; set; }

        [Required]
        public string IsAnonVoting { get; set; }
        public string IsArchived { get; set; }

        [Required]
		public string IsRead { get; set; }
	}
}
