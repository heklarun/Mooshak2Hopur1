using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Models
{
    public class ResponseViewModels
    {
        public int responseID { get; set; }
        public int projectID { get; set; }

        public string fileName { get; set; }
        public int partResponseID { get; set; }
        public string subProjectName { get; set; }
        public string handedIn { get; set; }
        public string members { get; set; }
        public int status { get; set; }
    }
}