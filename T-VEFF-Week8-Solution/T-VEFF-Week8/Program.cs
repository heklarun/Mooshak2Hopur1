using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace T_VEFF_Week8
{
	class Program
	{
		static void Main(string[] args)
		{

			//7.1
			/*
				Strings in .NET are immutable. This basically means that once we’ve 
				constructed a string, we cannot modify it. We can only construct 
				new strings, perhaps based on other strings.
			*/

			#region 7.1.2 Construction

			string emptyString = String.Empty;
			string emptyString2 = "";
			string noEmptyString = "John Doe";


			//Check for empty or null 
			string stringExample = GetStringFromSomewhereElse();
			if (!string.IsNullOrEmpty(stringExample))
			{
				// String is neither empty nor null
			}

			#endregion

			#region 7.1.3 Length

			/*
				We do not have to traverse the entire string to know its length. 
				The length is stored separately in the memory location preceding the 
				actual string, and can be accessed in constant time
			*/

			int stringLength = noEmptyString.Length;

			#endregion

			#region 7.1.4 String characters

			//Accessing individual characters can be done using operator[]:

			string name = "John Doe";
			char firstChar = name[0];

			//We can also traverse the characters in a string
			foreach (char item in name)
			{
				Console.Write(item);
			}

			/*
				If you need to analyze individual characters in more detail, 
				you can use a variety of functions found in the Char class:
			*/

			if(char.IsLower(firstChar))
			{
				//TODO: do something..
			}
			if (char.IsDigit(firstChar))
			{
				//TODO: do something..
			}

			#endregion

			#region 7.1.5 Comparison

			string leftString = "Application";
			Console.WriteLine("Enter some string: ");
			string rightString = "some other string";  //Console.ReadLine();

			if(leftString == rightString)
			{
				// If the user entered "Application", the code here will be executed
			}

			//Another method of comparison
			if(leftString.Equals(rightString, StringComparison.OrdinalIgnoreCase))
            {

			}

			//Comparing only on a part of each string using string helper functions
			if(leftString.StartsWith("A") && rightString.EndsWith("s"))
			{

			}


			#endregion

			#region 7.1.6 Escape characters

			/*

				When a string contains the backslash character ('\'), the character following it will be
				interpreted as an escape character :
					● \0 represents a null character (i.e. the ASCII value 0)
					● \n represents a newline (ASCII 10)
					● \t represents a tab character
					● \" represents a double quotation mark
					● etc.

				However, if a stringis prefixed with the @ character , it will be interpreted as is
				(which can be useful when hardcoding file paths for instance):
			*/

			string path1 = "c:\\temp\\file.txt";
			string path2 = @"c:\temp\file.txt";
			// Also useful if a string spans multiple lines


			#endregion

			#region 7.1.7 Conversion

			Console.WriteLine("Enter a number");
			string stringNumber = "22"; //Console.ReadLine();
			// We can use System.Convert:
			int number = Convert.ToInt32(stringNumber);


			/*
				However, if stringNumber cannot be converted to a number, 
				an exception will be thrown. In many cases, Int32.TryParse( ) works better.
			*/

			int number2 = 0;
			if (int.TryParse(stringNumber, out number2))
			{
				// Yes, it succeeded
			}

			/*
				Converting any type to a string is much simpler, because all objects support the ToString( )
				function which they inherit from System.Object, and is overridden appropriately in each type:
			*/

			int number3 = 10;
			string stringNumber2 = number3.ToString();

			// We could also use the Convert.ToString method:
			string stringNumber3 = Convert.ToString(number3);

			#endregion

			#region 7.1.8 Concatenation

			//Using the +

			string someName = "John" + "Doe";

			//Using the +=
			someName += " son of the great Jane";

			//Using String.Format() with 1 param
			string fileName = "file.txt";
			string strMessage = string.Format("The file {0} could not be found", fileName);

			//Using String.Format() with 2 params
			int lineCount = 12;
			string message = string.Format("The file {0} contains {1} lines", fileName, lineCount);

			//Using String.Format() with repeated param
			string message2 = string.Format("Today is {0}. I repeat, {0}.", DateTime.Now);


			/*
				At times, we need to construct a single string from a number of other strings. Using operator
				+ / operator += is very inefficient if the number of strings is high, since a new object will be
				created each time (because strings are immutable). Using String.Format() is also quite
				expensive in terms of memory and time if there are more than a handful of parameters.
				When the number of strings exceeds a “handful”, we should use System.Text.StringBuilder
				instead.
			*/

			int rows = 10;
			int cols = 5; 
            StringBuilder strBldr = new StringBuilder(300);
			strBldr.Append("<table>");
			for (int i = 0; i < rows; i++)
			{
				strBldr.Append("<tr>");
				strBldr.Append(Environment.NewLine);
				for (int j = 0; j < cols; j++)
				{
					strBldr.Append("<td>");
					strBldr.Append(i);
					strBldr.Append(" ");
					strBldr.Append(j);
					strBldr.Append("</td>");
				}
				strBldr.Append("</tr>");
			}
			strBldr.Append("</table>");
			// Finally, when we're done, we can call ToString() to get the result:
			String strHTMLTable = strBldr.ToString();

			#endregion

			#region 7.1.9 Examples

			String str = "Nú er gaman";
			str.ToUpper();
			// Has no effect doesn't modify the string...

			String str2 = str.ToUpper();
			// str2 is now in uppercase, str is unmodified

			String str3 = str2.ToLower();
			// str3 is now in lowercase, str2 is unmodified

			String str4 = str.Substring(3, 2);
			// Creates a new 2 letter string, starting in
			// (zerobased) position 3 in the source string

			String str5 = str.Remove(3, 2);
			// str5 now equals "Nú gaman"

			String str6 = str.Replace("er", "var");
			// str6 now equals "Nú var gaman"

			String str7 = str.Insert(6, "ofsalega ");
			// Insert will add the parameter into the string at the
			// location before the position given
			// str7 now equals "Nú er ofsalega gaman"

			int nStringPos = str.IndexOf("gaman");
			// nStringPos equals == 6

			String str8 = "Strengur sem endar á space. ";
			String str9 = str8.Trim();
			// str9 doesn't contain any whitespace, neither at 
			//the beginning nor at the end of the string.

			String[] strArr = str.Split(null);
			// strArr contains an array of strings, in this case containing:
			// "Nú", "er" and "gaman"

			#endregion

			#region 7.3 Files

			//Example uses a stream (writer) to open a text file and append a string to it
			//Make sure to include using System.IO;

			//Open file w/ StreamWriter
			StreamWriter writer = new StreamWriter("C:\\Test\\LogFile.txt", true, Encoding.Default); //note that the location of the file is hardcoded
			//Write string to file
			writer.WriteLine("Hello world");
			//Dispose stream
			writer.Dispose();

			//Another way to write to file using File class
			File.AppendAllText("C:\\Test\\LogFile.txt", "Error occurred!"); //note that the location of the file is hardcoded

			//in region 7.6 we will see how it is important to dispose of any resources used, for example the StreamWriter

			#endregion

			#region 7.4 Encoding

			/*			
				A text file is not just a text file. It may have been written using one of many encodings:
					● ANSI 8 - bit characters
					● Unicode (little endian/big endian) - 16 bit characters (2^4)
					● UTF32 (little endian/big endian) - 32 bit characters (2^5)
					● UTF8 an example of a multibyte encoding, where each char may be 1 byte or more

				When opening a text file, we can specify what encoding was used to create the file (see
				above). The Encoding class contains a few static properties which represent different
				encodings (Unicode, UTF8 etc.):
					● Encoding.UTF32
					● Encoding.UTF8
					● Encoding.ASCII
					● etc.
				If we use Encoding.Default, we are simply using the encoding specified in the control panel
				of the current computer.			
			*/

			//See example files in bin folder with different encoding examples. Use binary editor in Visual Studio to view the different encodings
			//Open file in from top menu or press Ctrl + O, select file and press "Open with" by pressing the arrow next to the open button

			//Same example from 7.3
			//StreamWriter writerWithUTF8 = new StreamWriter("C:\\Test\\LogFile.txt", true, Encoding.UTF8);

			#endregion

			#region 7.5 Exception handling

			//Example function that might throw exception, see function code example
			//DoStuff();

			#endregion

			#region 7.6 Resource management

			//Example that tries to write to file but is wrapped in a try finally block in order to handle exception and dispose resources
			//Note the code bellow does not catch exceptions

			StreamWriter stream = null;
            try
			{
				string examplePath = "C:\\Test2";

				//Example code that creates directory if it does not exist
				if (!Directory.Exists(examplePath))
				{
					Directory.CreateDirectory(examplePath);
				}

				string examplePathFile = examplePath + "\\LogFile2.txt";

				stream = new StreamWriter(examplePathFile, true, Encoding.Default);
				stream.WriteLine("Hello world");								
			}
			finally
			{
				if(stream != null)
				{
					stream.Dispose();
				}
			}

			//Same example using a special construct "using" which will automatically create a try/finally block, and inside the finally block the file will be closed

			using (StreamWriter stream2 = new StreamWriter("C:\\Test\\LogFile.txt", true, Encoding.Default))
			{
				stream2.WriteLine("Hello world");
			}


			//Another example which READS from file located where the program is running
			string path = AppDomain.CurrentDomain.BaseDirectory; 

			//StreamReader reader = new StreamReader(path + "text.ansi.txt");
			//StreamReader reader = new StreamReader(path + "text.unicode.txt");
			//StreamReader reader = new StreamReader(path + "text.unicode.bigendian.txt");
			StreamReader reader = new StreamReader(path + "text.utf8.txt");

			string strline = reader.ReadLine();
			while (strline != null)
			{
				System.Diagnostics.Debug.WriteLine(strline);
				strline = reader.ReadLine();
			}
			reader.Dispose();

			#endregion

			#region 7.7 Configuration files


			//We can use appSettings node in the configuration file to save usefull information that our application might need.

			//Add reference to References in solution explorer and add using statement "using System.Configuration;" to use ConfigurationManager class
			string logFile = ConfigurationManager.AppSettings["LogFile"];
			string email = ConfigurationManager.AppSettings["Email"];

			//TODO: user logFile and email for some purpose

			//We can also get a connection string from our application config file. For example a connection string for a resource.
			string strConn = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;

			//TODO: Connect to 

			//We can also send emails in C#, for this we need to add using System.Net.Mail;

			//This example uses a google account to send the email, see mail settings in configuration file

			/*
				NOTE: Google now blocks such access due to security reasons. 
				In order to enable this example using your own account you will have to change the setting of your google email account			
				https://support.google.com/accounts/answer/6010255
			*/

			//sendEmailEx(email);

			#endregion

			#region 7.8 Logging

			//Will be continued in example web application

			#endregion

			Console.ReadKey();
		}

		public static string GetStringFromSomewhereElse()
		{
			return null;
		}

		public static void DoStuff()
		{
			try
			{
				int anotherResult = CodeThatMightThrowAnException();
				// This line might throw an exception
			}
			/*catch(ArgumentException aex) //We can have more than one catch block
			{

			}*/
			catch (Exception ex) //Note that catch is not mandatory
			{
				string message = ex.Message;					//error message which describes what went wrong
				string stack = ex.StackTrace;					//The name of the function where the exception was thrown,
				string source = ex.Source;						//The name of the location of the error
				Exception innerException = ex.InnerException;   //useful when an exception is wrapped in another exception

				//TODO: log the exception

				//throw ex;	//throw exception again, but from this location.
				//throw; //throw the current exception up the stack.
				
			}
			finally //This code will always execute, both if an error was thrown, and if no error occurred. A finally block does not have to be associated with a catch block,
            {
				//TODO: do some something if there is an exception or not
			}
		}

		public static int CodeThatMightThrowAnException()
		{
			if (true)/* some condition */
			{
				throw new ArgumentException();
			}
		}

		public static void sendEmailEx(string email)
		{
			try
			{
				using (MailMessage message = new MailMessage())
				{
					// See above, email address should
					// be read from appSettings:
					message.To.Add(email);
					message.Subject = "Email subject line";
					message.Body = "Hello world!";
					using (SmtpClient client = new SmtpClient())
					{
						client.EnableSsl = true; // Not always necessary
						client.Send(message);
					}
				}
			}
			catch (Exception ex)
			{
				// TODO: log the error!
			}
		}

		/// <summary>
		/// Example function that exception handling 
		/// </summary>
		static void MyFunc()
		{
			/*try 
            {*/
			MyFunc2();
			/*}
            catch (Exception ex)
            {
                string tempPath = Path.GetTempPath();
                string path = AppDomain.CurrentDomain.BaseDirectory;

                File.AppendAllText(path + "logtext.txt", ex.Message + System.Environment.NewLine + ex.StackTrace);
            }*/
		}

		/// <summary>
		/// Example function that exception handling 
		/// </summary>
		static void MyFunc2()
		{
			/*try
            {*/
			//throw new ArgumentException("Bad argument");

			string denominator = "10";
			int denom = 10;

			Int32.TryParse(denominator, out denom);

			int val = 10 / denom;

			/*}
            catch (DivideByZeroException ex)
            {
                string tempPath = Path.GetTempPath();
                string path = AppDomain.CurrentDomain.BaseDirectory;

                File.AppendAllText(path + "logtext.txt", ex.Message + System.Environment.NewLine + ex.StackTrace);            
            }
            catch (Exception ex)
            {
                string tempPath = Path.GetTempPath();
                string path = AppDomain.CurrentDomain.BaseDirectory;

                File.AppendAllText(path + "logtext.txt", ex.Message + System.Environment.NewLine + ex.StackTrace);
            }*/
		}
	}
}
