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

        //Compile code function, gets the code from the student and compiles
        [HttpPost]
        public ActionResult CompileCode(FormCollection data, int? partResponseID)
        {
            // To simplify matters, we declare the code here.
            // The code would of course come from the student!   
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            SubProjectsViewModels sub = projectService.DownloadPartResponseFile(partResponseID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            var sr = new StreamReader(ms);
            string myStr = sr.ReadToEnd();

            var code = myStr;

            // Set up our working folder, and the file names/paths.
            // In this example, this is all hardcoded, but in a
            // real life scenario, there should probably be individual
            // folders for each user/assignment/milestone.
            string serverPath = Server.MapPath("~");
            var workingFolder = ConfigurationManager.AppSettings["workingFolder"]; //býr til möppuna

            string filePath = serverPath + workingFolder + "\\" + appUser.UserName; //það sem exeFilePath var

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var username = appUser;
            var cppFileName = partResponseID + "_" + appUser.UserName + ".cpp";  //Aðgerð til að skrifa í cpp skrá

            //var exeFilePath = workingFolder + "Hello.exe";  // Hvað er exe file?
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
                    processExe.StandardInput.WriteLine(); //Það sem var kommentað
                    processExe.Start();
                    processExe.StandardInput.WriteLine("\"" + compilerFolder + "vcvars32.bat" + "\"");
                    processExe.StandardInput.WriteLine("cl.exe /nologo /EHsc " + cppFileName);
                    processExe.StandardInput.WriteLine("exit");
                    // to above.

                    // We then read the output of the program:
                    var lines = new List<string>();
                    while (!processExe.StandardOutput.EndOfStream)
                    {
                        lines.Add(processExe.StandardOutput.ReadLine());
                        Directory.CreateDirectory(filePath);
                    }

                    ViewBag.Output = lines;
                }
            }

            else
            {
                //ef skrá er ekki til hvað þá?
            }

            // TODO: We might want to clean up after the process, there
            // may be files we should delete etc.
            // Delete þeim skrám sem búnar hafa verið til í kjöæfar þess að keyra kóðann
            // Búa til fall sem tékkar á tímanum sem forritið er að keyrast. Ef 10 sec + þá executea


            return View("ProjectPartPartial");
        }
    }
}