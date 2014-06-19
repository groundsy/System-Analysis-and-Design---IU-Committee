/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: CommOwnMeta.cs
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
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBananaPhase4.Models;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;

namespace TeamBananaPhase4.Models
{
 
    [MetadataType(typeof(CommOwnMeta))]
    public partial class CommOwn
    {

	}

	public class CommOwnMeta
	{
		public int ID { get; set; }


    }

    public static class HtmlHelperCommName
    {
        public static MvcHtmlString CommName(this HtmlHelper helper, int CommOwn_ID)
        {
            jashdownEntities db = new jashdownEntities();

            var unitFound = db.Unit.FirstOrDefault(c => c.CommOwn_ID == CommOwn_ID);
            if (unitFound != null)
            {
                return MvcHtmlString.Create(unitFound.Name);
            }

            var schoolFound = db.School.FirstOrDefault(c => c.CommOwn_ID == CommOwn_ID);
            if (schoolFound != null)
            {
                return MvcHtmlString.Create(schoolFound.Name);
            }

            var campusFound = db.Campus.FirstOrDefault(c => c.CommOwn_ID == CommOwn_ID);
            if (campusFound != null)
            {
                return MvcHtmlString.Create(campusFound.Name);
            }

            var universityFound = db.University.FirstOrDefault(c => c.CommOwn_ID == CommOwn_ID);
            if (universityFound != null)
            {
                return MvcHtmlString.Create(universityFound.Name);
            }

            return MvcHtmlString.Create(@"");

        }
    }

}
