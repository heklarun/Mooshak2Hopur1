using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak2.DAL;
using Mooshak2.Models;
using SecurityWebAppTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak2.Services;
using System.Diagnostics;
using System.Configuration;
using System.IO;

namespace Mooshak2.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IdentityManager man = new IdentityManager();

        UserService userService = new UserService();
        ProjectService projectService = new ProjectService();
        CourseService courseService = new CourseService();

        //Compile code function, gets the code from the student and compiles the code.
        //The user will get a response wich is eather accepted, wrong answer, compile time error or a memmory error.
        [HttpPost]
        public ActionResult CompileCode(FormCollection data, int? partResponseID, PartResponseViewModels response)
        {
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            SubProjectsViewModels sub = projectService.DownloadPartResponseFile(partResponseID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            var sr = new StreamReader(ms);
            string myStr = sr.ReadToEnd();
            var code = myStr;

            string serverPath = Server.MapPath("~");
            var workingFolder = ConfigurationManager.AppSettings["workingFolder"]; //býr til möppuna

            string filePath = serverPath + workingFolder + "\\" + appUser.UserName; 

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var username = appUser;
            var cppFileName = partResponseID + "_" + appUser.UserName + ".cpp";  //To put the input in .cpp file named after the user

            // Write the code to a file, such that the compiler
            // can find it:
            System.IO.File.WriteAllText(workingFolder + cppFileName, code);

            // In this case, we use the C++ compiler (cl.exe) which ships
            // with Visual Studio. It is located in this folder:
            var compilerFolder = ConfigurationManager.AppSettings["compilerFolder"];
            // There is a bit more to executing the compiler than
            // just calling cl.exe. In order for it to be able to know
            // where to find #include-d files (such as <iostream>),
            // we need to add certain folders to the PATH.
            // There is a .bat file which does that, and it is
            // located in the same folder as cl.exe, so we need to execute
            // that .bat file first.

            // Using this approach means that:
            // * the computer running our web application must have
            //   Visual Studio installed. This is an assumption we can
            //   make in this project.
            // * Hardcoding the path to the compiler is not an optimal
            //   solution. A better approach is to store the path in
            //   web.config, and access that value using ConfigurationManager.AppSettings.

            // Execute the compiler:
            Process compiler = new Process();
            compiler.StartInfo.FileName = "cmd.exe";
            compiler.StartInfo.WorkingDirectory = workingFolder;
            compiler.StartInfo.RedirectStandardInput = true;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.StartInfo.UseShellExecute = false;

            compiler.Start();
            compiler.StandardInput.WriteLine("\"" + compilerFolder + "vcvars32.bat" + "\"");
            compiler.StandardInput.WriteLine("cl.exe /nologo /EHsc " + cppFileName);
            compiler.StandardInput.WriteLine("exit");

            string output = compiler.StandardOutput.ReadToEnd();
            compiler.WaitForExit();
            compiler.Close();

            // Check if the compile succeeded, and if it did,
            // we try to execute the code:
            if (System.IO.File.Exists(filePath))
            {
                var processInfoExe = new ProcessStartInfo(filePath, "");
                processInfoExe.UseShellExecute = false;
                processInfoExe.RedirectStandardOutput = true;
                processInfoExe.RedirectStandardError = true;
                processInfoExe.CreateNoWindow = true;
                using (var processExe = new Process())
                {
                    processExe.StartInfo = processInfoExe;
                    processExe.Start();
                    // In this example, we don't try to pass any input
                    // to the program, but that is of course also
                    // necessary. We would do that here, using
                    // processExe.StandardInput.WriteLine();
                    /* processExe.StandardInput.WriteLine("\"" + compilerFolder + "vcvars32.bat" + "\"");
                       processExe.StandardInput.WriteLine("cl.exe /nologo /EHsc " + cppFileName);
                       processExe.StandardInput.WriteLine("exit");  */
                    // to above.
                    var lines = new List<string>();
                    var timeError = processExe.WaitForExit(3000);
                    if (!timeError) //If the input runtime is to slow.
                    {
                        string message = "Compile Time Error";
                      //ines.Add(processExe.StandardOutput.ReadLine(message));
                        lines.Add(message);
                        processExe.Kill();
                        //return lines;
                    }

                    if (output != myStr) //If the output from the student is wrong, student output is myStr.
                    {
                        //projectService.submitSubProject(response); 
                        string message = "Wrong Answer";
                        lines.Add(message);
                        processExe.Kill();
                        //return lines;
                    }

                    if (output == myStr) //If the output from the student is correct, student output is myStr.
                    {
                        string message = "Accepted";
                        lines.Add(message);
                        processExe.Kill();
                        //return lines;
                    }

                    // We then read the output of the program:

                    while (!processExe.StandardOutput.EndOfStream)
                    {
                        lines.Add(processExe.StandardOutput.ReadLine());
                        Directory.CreateDirectory(filePath);
                    }

                    ViewBag.Output = lines;
                }
            }

            db.SaveChanges();
            return View("ProjectPartPartial");
        }
    }
}