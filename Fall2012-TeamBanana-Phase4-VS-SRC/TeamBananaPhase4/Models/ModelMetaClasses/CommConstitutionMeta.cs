/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: CommConstitutionMeta.cs
 * Created by: Joel Haubold
 * Created date: 11-17-12
 * Primary Programmer: 
 * File description: Contains model validation meta data
 * 
 * Change Log:
 *  Date     Programmer     Change
 * 11-28-12     dt          Added Require to required member 
 * 
*************************************************/

using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;

namespace TeamBananaPhase4.Models
{
    [MetadataType(typeof(CommConstitutionMeta))]
    public partial class CommConstitution
    {
	}

	public class CommConstitutionMeta
	{
        [Required]
        public int Comm_CommOwn_ID { get; set; }

        [Required]
        public int Comm_ID { get; set; }

        [Required]
        public System.DateTime EffectiveDate { get; set; }
        public string Constitution { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}
