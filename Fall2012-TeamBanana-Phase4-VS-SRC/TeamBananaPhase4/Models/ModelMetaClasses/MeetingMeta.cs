/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: MeetingMeta.cs
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
using System.ComponentModel;

namespace TeamBananaPhase4.Models
{
	[MetadataType(typeof(MeetingMeta))]
	public partial class Meeting
	{
		public class PrimaryKey
		{
			public int Comm_CommOwn_ID { get; set; }
			public int Comm_ID { get; set; }
			public DateTime DateTime { get; set; }
		}
	}

	public class MeetingMeta
	{
		public int Comm_CommOwn_ID { get; set; }
		public int Comm_ID { get; set; }

		[Required]
		[Display(Name = "Date and Time of meeting")]
		public System.DateTime DateTime { get; set; }
		
		[Required]
		public string Location { get; set; }

		[Required(ErrorMessage = "Please Enter Y or N")]
		[Display(Name="Listed publicly?")]
		[EnumDataType(typeof(TeamBananaPhase4.Controllers.enumYN),ErrorMessage="Please Enter Y or N")]
		public string IsListedPublicly { get; set; }

		[Display(Name="Final Agenda")]
		public string FinalAgenda { get; set; }

		[Display(Name="Created by")]
		public string CreatedBy { get; set; }

		[Display(Name="Created Date")]
		public System.DateTime CreatedDate { get; set; }
	}
}
