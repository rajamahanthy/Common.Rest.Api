namespace Common.Rest.Shared.CustomValidations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class AnyOfAttribute : ValidationAttribute
    {
        private readonly string[] _propertyNames;

        public AnyOfAttribute(params string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
            {
                throw new ArgumentException(
                    "At least one property name must be specified for AnyOf validation.");
            }

            _propertyNames = propertyNames;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value == null)
                return ValidationResult.Success;

            var runtimeType = value.GetType();

            // Validate that all configured properties exist
            var properties = _propertyNames
                .Select(name => runtimeType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance))
                .ToArray();

            if (properties.Any(p => p == null))
            {
                var missing = _propertyNames.Where((n, i) => properties[i] == null);
                return new ValidationResult(
                    $"AnyOf validation misconfigured. Missing properties: {string.Join(", ", missing)}");
            }

            // JSON Schema semantics:
            // at least ONE property must be present and non-empty
            var anyPresent = properties.Any(p =>
            {
                var propValue = p!.GetValue(value);

                return propValue switch
                {
                    null => false,
                    string s => !string.IsNullOrWhiteSpace(s),
                    _ => true
                };
            });

            if (!anyPresent)
            {
                return new ValidationResult(
                    $"At least one of the following properties must be supplied: {string.Join(", ", _propertyNames)}");
            }

            return ValidationResult.Success;
        }
    }
}
