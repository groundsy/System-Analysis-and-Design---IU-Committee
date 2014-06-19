/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: CampusMeta.cs

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
	[MetadataType(typeof(CampusMeta))]
	public partial class Campus
    {
	}
	public class CampusMeta
	{
        public string University_Code { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int CommOwn_ID { get; set; }
        public int Employer_ID { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}
