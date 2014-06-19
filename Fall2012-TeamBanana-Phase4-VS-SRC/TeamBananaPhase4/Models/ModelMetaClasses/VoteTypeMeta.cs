/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: VoteTypeMeta.cs

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
	[MetadataType(typeof(VoteTypeMeta))]
    public partial class VoteType
    {
	}

	public class VoteTypeMeta
	{   public string Type { get; set; }
        public string Description { get; set; }
    }
}
