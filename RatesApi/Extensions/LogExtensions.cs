using Microsoft.Extensions.Configuration;
using System.IO;

namespace RatesApi.Extensions
{
    public static class LogExtensions
    {
        public const string catalogName = "Logs";
        private const string _fileNameAndFormantForLog = "Log-.txt";

       public static string GetPathToFile(this IConfigurationBuilder config)
        {
            var pathToFolder = CheckFolderIfAbsentThenCreate();
            var path = Path.Combine(pathToFolder, _fileNameAndFormantForLog);
            return path;
        }
        private static string GetPathToFolder()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            string pathToFolder = Path.Combine(currentDirectory, catalogName);

            return pathToFolder;
        }
        private static string CheckFolderIfAbsentThenCreate()
        {
            var pathToFolder = GetPathToFolder();
            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }
            return pathToFolder;
        }
    }
}
