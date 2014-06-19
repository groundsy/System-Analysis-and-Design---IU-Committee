using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamBananaPhase4.Models
{
	public class DiscItemDocumentWithoutImage
	{
		public int DiscItem_Meeting_Comm_CommOwn_ID { get; set; }
		public int DiscItem_Meeting_Comm_ID { get; set; }
		public System.DateTime DiscItem_Meeting_DateTime { get; set; }
		public string DiscItem_Title { get; set; }
		public string Filename { get; set; }
		public string Tags { get; set; }
	}
}