namespace DataIntegrationTool.Shared.Utils
{
    public class Constants
    {
        public const string APPSETTINGS = "appsettings.json";
        public const string ETL = "ETL";
        public const string INPUT = "Input";
        public const string OUTPUT = "Output";
        public const string TRANSFORMATION = "Transformations";
        public const string INPUTTYPE = "Tipo input";
        public const string ERRORMESSAGEPROGRAM = "assente o invalido";
        public const string ERRORMESSAGE = "Errore durante la lettura del file CSV";
        public static readonly char[] firstLineSeparator = ['\r', '\n'];

        public const string FILENOTFOUNDEXCEPTION = "FileNotFound";
        public const string MISSINGFIELDEXCEPTION = "MissingField";
        public const string HEADERVALIDATIONEXCEPTION = "HeaderValidation";
        public const string MISSINGHEADEREXCEPTION = "MissingHeader";
        public const string MISSINGALLHEADERSEXCEPTION = "MissingAllHeaders";
        public const string DUPLICATEHEADEREXCEPTION = "DuplicateHeader";
        public const string INCORRECTFILEPATH = "IncorrectFilePath";
    }
}
