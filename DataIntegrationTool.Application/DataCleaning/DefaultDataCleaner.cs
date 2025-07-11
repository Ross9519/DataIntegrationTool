using System.Collections;
using System.Reflection;
using DataIntegrationTool.Application.Interfaces;
using static DataIntegrationTool.Shared.Utils.DataCleaningHelper;

namespace DataIntegrationTool.Application.DataCleaning
{
    public class DefaultDataCleaner<T> : IDataCleaner<T> where T : class
    {
        public void Clean(IEnumerable<T> raws)
        {
            raws = RemoveDuplicates(raws);

            raws.ToList()
                .ForEach(Clean);
        }

        public void Clean(T raw)
        {
            ArgumentNullException.ThrowIfNull(raw);

            Type type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(raw);

                if (value == null)
                    continue;

                if (value is string str)
                {
                    prop.SetValue(raw, CleanString(str));
                    continue;
                }

                if (value is ICleanable cleanable)
                {
                    cleanable.Clean();
                    continue;
                }

                if (value is IEnumerable enumerable && value is not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;
                        CleanObject(item);
                    }
                    continue;
                }

                if (IsClassType(value.GetType()))
                {
                    CleanObject(value);
                }
            }
        }

        private static void CleanObject(object obj)
        {
            var cleanerType = typeof(DefaultDataCleaner<>).MakeGenericType(obj.GetType());
            var cleaner = Activator.CreateInstance(cleanerType);
            var method = cleanerType.GetMethod("Clean", [obj.GetType()]);

            method?.Invoke(cleaner, [obj]);
        }

        private static bool IsClassType(Type type)
        {
            return type.IsClass &&
                   type != typeof(string) &&
                   type.Namespace != null &&
                   !type.Namespace.StartsWith("System");
        }
    }
}
