using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Models
{
    public class PartResponseViewModels
    {
        public int partResponseID { get; set; }
        public int subProjectID { get; set; }
        public string subProjectName { get; set; }
        public int projectID { get; set; }

        public int responseID { get; set; }
        public DateTime date { get; set; }
        public HttpPostedFileBase file { get; set; }
        public byte[] fileBytes { get; set; }
        public string fileName { get; set; }
        public string fileContentType { get; set; }
        public string userID { get; set; }
    }
}