/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: MemberRoleMeta.cs
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
    [MetadataType(typeof(MemberRoleMeta))]
    public partial class MemberRole
    {
	}

	public class MemberRoleMeta
	{
        public string Role { get; set; }
        public string Description { get; set; }
    }
}
