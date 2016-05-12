using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class SubProjectsViewModels
    {
        public int subProjectID { get; set; }
        public int projectID { get; set; }
        public string subProjectName { get; set; }
        public double percentage { get; set; }

        public HttpPostedFileBase inputFile { get; set; }
        public byte[] inputFileBytes { get; set; }
        public string inputFileName { get; set; }
        public string inputContentType { get; set; }
        public HttpPostedFileBase outputFile { get; set; }
        public byte[] outputFileBytes { get; set; }
        public string outputFileName { get; set; }
        public string outputContentType { get; set; }

        public int nrOfResponses { get; set; }
        public string  projectName { get; set; }

        public int memberCount { get; set; }
        
        
    }
}