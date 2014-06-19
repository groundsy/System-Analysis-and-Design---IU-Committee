/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: CommMemberMeta.cs
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
	[MetadataType(typeof(CommMemberMeta))]
	public partial class CommMember
    {
	}
	public class CommMemberMeta
	{
        public int Comm_CommOwn_ID { get; set; }
        public int Comm_ID { get; set; }

        [Required]
        public string Member_Email { get; set; }

        public string IsAdministrator { get; set; }
        public string IsConvener { get; set; }
        
        [Required]
        public string MemberRole_Role { get; set; }

        [Required]
        public string Voting_Non_Voting { get; set; }

        public Nullable<int> Representing { get; set; }

        [Required]
        public System.DateTime StartDate { get; set; }
        
        [Required]
        public System.DateTime EndDate { get; set; }
        
        public string LastAssignedBy { get; set; }
        public System.DateTime LastAssignedDate { get; set; }
	}
}
