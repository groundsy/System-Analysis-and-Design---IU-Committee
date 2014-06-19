/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: AnonVotingMeta.cs
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
	[MetadataType(typeof(AnonVotingMeta))]
	public partial class AnonVoting
    {
	}
	public class AnonVotingMeta
	{
        public int ID { get; set; }
        public string DiscItem_Title { get; set; }
        public int DiscItem_Meeting_Comm_CommOwn_ID { get; set; }
        public System.DateTime DiscItem_Meeting_DateTime { get; set; }
        public int DiscItem_Meeting_Comm_ID { get; set; }
        public string Vote { get; set; }
    }
}
