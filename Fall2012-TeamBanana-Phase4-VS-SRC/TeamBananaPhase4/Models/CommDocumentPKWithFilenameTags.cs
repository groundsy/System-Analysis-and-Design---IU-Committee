using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamBananaPhase4.Models
{
	public class CommDocumentPKWithFilenameTags
	{
		public int Comm_CommOwn_ID { get; set; }
		public int Comm_ID { get; set; }
		public string Title { get; set; }
		public string Tags { get; set; }
		public string Filename { get; set; }
		public string Comm_Name { get; set; }
		public string IsPublic { get; set; }
		public string IsArchived { get; set; }
		public DateTime DisplayDate { get; set; }
		public string Category { get; set; }
	}
}