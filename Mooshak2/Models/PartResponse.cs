using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class PartResponse
    {
        [Key]
        public int partResponseID { get; set; }
        public int subProjectID { get; set; }
        public int responseID { get; set; }
        public byte[] file { get; set; }
        public DateTime date { get; set; }
        public string fileMimeType { get; set; }
        public string fileFileName { get; set; }
        public string handedIn { get; set; }
        public int status { get; set; }
    }
}