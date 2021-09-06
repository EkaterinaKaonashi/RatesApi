using Microsoft.Extensions.Configuration;
using System.IO;

namespace RatesApi.Extensions
{
    public static class ConfigurationExtensions
    {
        public const string catalogName = "Logs";
        private const string _fileNameAndFormantForLogInformation = "Log-.txt";
        private const string _fileNameAndFormantForLogError = "errorLog-.txt";

       public static string GetPathToInformationFile(this IConfigurationBuilder config)
        {
            var pathToFolder = CheckFolderIfAbsentThenCreate();
            var path = Path.Combine(pathToFolder, _fileNameAndFormantForLogInformation);
            return path;
        }
        public static string GetPathToErrorFile(this IConfigurationBuilder config)
        {
            var pathToFolder = CheckFolderIfAbsentThenCreate();
            var path = Path.Combine(pathToFolder, _fileNameAndFormantForLogError);
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
