using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Masking;

namespace IPMaskingTool
{
    class Program
    {
        static void Main(string[] args)
        {
            MaskingEntity me = new MaskingEntity();
            string textFromFile;
            string newFile;
            string fullPathFile;

            do
            {
                Console.WriteLine(@"Please enter valid log file full path.");
                fullPathFile = Console.ReadLine();
            }

            while (!getTextFromLogFile(fullPathFile, out textFromFile, out newFile));
            
            string newText = me.Mask(textFromFile);

            try
            {
                File.AppendAllText(newFile, newText);
                Console.WriteLine("Do not worry! The IPs have been masked!");
                Console.WriteLine("New file created: {0}", newFile);
            }

            catch (Exception e)
            {
                ErrorMSG("Problem occurred while trying to write to: " + newFile + "\n" + e.Message);
            }
        }

        public static bool getTextFromLogFile(string fullPathFile, out string textFromFile, out string newFile)
        {
            string text = string.Empty;
            string fullPathRegex = @"^([a-zA-Z]:\\(\w+\\)*)(\w+)\.log$";
            Match matchFile = Regex.Match(fullPathFile, fullPathRegex);
            textFromFile = string.Empty;
            newFile = string.Empty;

            if (matchFile.Success)
            {
                if (File.Exists(fullPathFile))
                {
                    FileInfo fileInfo = new FileInfo(fullPathFile);
                        
                    if (fileInfo.Length <= 5242880)
                    {
                        string directory = matchFile.Groups[1].ToString();
                        string nameOfFile = matchFile.Groups[3].ToString();

                        try
                        {
                            textFromFile = File.ReadAllText(fullPathFile);
                            newFile = directory + "Masked_" + nameOfFile + ".log";

                            if (File.Exists(newFile))
                            {
                                File.Delete(newFile);
                            }

                            return true;
                        }

                        catch (Exception e)
                        {
                            return ErrorMSG(e.Message);
                        }
                    }

                    else
                    {
                        return ErrorMSG("Wrong input! File is too big (more than 5MB)!");
                    }
                }

                else
                {
                    return ErrorMSG("Wrong input! File does not exist!");
                }
            }

            else
            {
                return ErrorMSG("Wrong input! Invalid file path!");
            }
        }

        public static bool ErrorMSG(string error)
        {
            Console.Clear();
            Console.WriteLine(error);
            return false;
        }
    }
}
