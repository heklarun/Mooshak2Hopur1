using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class SubProjects
    {
        [Key]
        public int subProjectID { get; set; }
        public int projectID { get; set; }
        public string subProjectName { get; set; }
        public double percentage { get; set; }
        public byte[] inputFile { get; set; }
        public string inputFileContentType { get; set; }
        public string inputFileName { get; set; }
        public byte[] outputFile { get; set; }
        public string outputFileContentType { get; set; }
        public string outputFileName { get; set; }
    }
}