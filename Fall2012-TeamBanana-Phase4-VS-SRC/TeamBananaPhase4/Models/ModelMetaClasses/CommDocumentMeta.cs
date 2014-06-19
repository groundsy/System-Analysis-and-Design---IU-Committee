/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: CommDocumentMeta.cs
 * Created by: Joel Haubold
 * Created date: 11-17-12
 * Primary Programmer: 
 * File description: Contains model validation meta data
 * 
 * Change Log:
 * Date programmer    change
 * 11/30/2012 Justin  Added req'd and display tags
 * 
*************************************************/

using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;

namespace TeamBananaPhase4.Models
{
	[MetadataType(typeof(CommDocumentMeta))]
    public partial class CommDocument
    {
		
        
    }
	
	public class CommDocumentMeta
	{
        public int Comm_CommOwn_ID { get; set; }

        public int Comm_ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Date and Time of document")]
        public System.DateTime DisplayDate { get; set; }

        public string Tags { get; set; }
        
        public string Filename { get; set; }

        [Display(Name = "File")]
        public byte[] FileImage { get; set; }

        [Required]
        [Display(Name = "Document Category Type")]
        public string Category { get; set; }

        //[Required]
        public string IsPublic { get; set; }
                
        public string IsProtected { get; set; }

        public string UploadedBy { get; set; }

        public System.DateTime UploadedDate { get; set; }

        public string IsArchived { get; set; }

        public string ArchivedBy { get; set; }

        public Nullable<System.DateTime> ArchivedDate { get; set; }

        public string ContentType { get; set; }
    }
}
