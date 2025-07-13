using System.Collections;
using System.Reflection;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Application.DataValidation
{
    public class DefaultDataValidator<T> : IDataValidator<T> where T : class
    {
        public InvalidItemsReport<T> Validate(IEnumerable<T> raws)
        {
            ArgumentNullException.ThrowIfNull(raws);

            var validItems = new List<T>();
            var invalidItems = new List<(T, ValidationResult)>();

            foreach (var raw in raws)
            {
                var result = Validate(raw);

                if (result.IsValid)
                    validItems.Add(raw);
                else
                    invalidItems.Add((raw, result));
            }

            return new InvalidItemsReport<T>
            {
                ValidItems = validItems,
                InvalidItems = invalidItems
            };
        }

        public ValidationResult Validate(T raw)
        {
            ArgumentNullException.ThrowIfNull(raw);
            var result = new ValidationResult();

            Type type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(raw);

                if (value is IValidatable validatable)
                {
                    var subResult = validatable.Validate();
                    result.Add(prop.Name, subResult);
                    continue;
                }

                if (value is IEnumerable enumerable && value is not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;
                        ValidateObject(item, result, prop.Name);
                    }
                    continue;
                }

                if (value != null && IsClassType(value.GetType()))
                {
                    ValidateObject(value, result, prop.Name);
                }
            }

            return result;
        }

        private static void ValidateObject(object obj, ValidationResult result, string prefix)
        {
            var validatorType = typeof(DefaultDataValidator<>).MakeGenericType(obj.GetType());
            var validator = Activator.CreateInstance(validatorType);
            var method = validatorType.GetMethod("Validate", [obj.GetType()]);

            if (method?.Invoke(validator, [obj]) is ValidationResult subResult)
            {
                result.Add(prefix, subResult);
            }
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
