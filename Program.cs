using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CNG462_A2
{
    class Program
    {
        static char[][] Matrix;
        static List<String> Domain;

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: CNG462_A2 <mapfilename> <wordlist>");
                Environment.Exit(1);
            }

            ReadFiles(args[0], args[1]);

            try
            {
                char[][] ResultMatrix = new CSP(Matrix, Domain).Solve();
                Console.WriteLine("Solution has been found!");
                Console.WriteLine("");

                CSP.PrintMatrix(ResultMatrix);
            } catch (InvalidOperationException)
            {
                Console.WriteLine("A valid solution could not be found!");
            }
        }

        static void ReadFiles(string MapTxtFileName, string AvailableWordsFileName)
        {
            string[] MapTxtFileContent = null;
            string[] AvailableWordsFileContent = null;

            try
            {
                MapTxtFileContent = File.ReadAllLines(MapTxtFileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File " + MapTxtFileName + " not found!");
                Environment.Exit(1);
            }

            Matrix = MapTxtFileContent.Select(item => item.ToArray()).ToArray();

            try
            {
                AvailableWordsFileContent = File.ReadAllLines(AvailableWordsFileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File " + MapTxtFileName + " not found!");
                Environment.Exit(1);
            }

            Domain = new List<string>(AvailableWordsFileContent);
        }
    }
}
