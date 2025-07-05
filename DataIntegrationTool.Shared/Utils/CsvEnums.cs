namespace DataIntegrationTool.Shared.Utils
{
    public static class CsvEnums
    {
        public enum CsvErrorType
        {
            FileNotFound,
            MissingField,
            TypeConversion,
            HeaderValidation,
            MissingHeader,
            MissingAllHeaders,
            DuplicateHeader,
            IncorrectFilePath,
            Generic
        }

        public enum InputType
        {
            /// <summary>
            /// Legge i dati da un file locale (es. .csv)
            /// </summary>
            File,

            /// <summary>
            /// Riceve il csv direttamente da configurazioni, inserito come testo
            /// </summary>
            String,

            /// <summary>
            /// Scarica i dati da una URL remota
            /// </summary>
            Http,

            /// <summary>
            /// Legge i dati eseguendo una query su un database relazionale
            /// </summary>
            Database
        }

        public enum OutputType
        {
            /// <summary>
            /// Scrive i dati trasformati su file locale
            /// </summary>
            File,

            /// <summary>
            /// Inserisce i dati in un database relazionale
            /// </summary>
            Database,

            /// <summary>
            /// Invia i dati a una REST API esterna
            /// </summary>
            Api
        }

        public enum OutputFormat
        {
            /// <summary>
            /// Formato JSON (standard moderno e leggibile)
            /// </summary>
            Json,

            /// <summary>
            /// Formato CSV (testo tabellare, compatibile con Excel)
            /// </summary>
            Csv,

            /// <summary>
            /// Formato XML (strutturato, legacy compatibile)
            /// </summary>
            Xml
        }

        public enum TransformationType
        {
            /// <summary>
            /// Rimuove spazi iniziali/finali da tutti i campi stringa
            /// </summary>
            TrimFields,

            /// <summary>
            /// Valida che i campi richiesti non siano null o vuoti
            /// </summary>
            ValidateRequiredFields,

            /// <summary>
            /// Converte varianti di nomi di paese in forma standard (es. "ITA" → "Italy")
            /// </summary>
            NormalizeCountry,

            /// <summary>
            /// Rinomina o rimappa i nomi dei campi di input
            /// </summary>
            MapFields,

            /// <summary>
            /// Esegue una logica personalizzata, definita via script o estensione
            /// </summary>
            CustomScript
        }
    }
}
