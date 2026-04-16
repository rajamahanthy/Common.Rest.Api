namespace Common.Rest.Shared.CustomValidations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NoAdditionalPropertiesAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is null)
                return ValidationResult.Success;

            var runtimeType = value.GetType();

            // Find the [JsonExtensionData] property on the RUNTIME type
            var extensionDataProperty = runtimeType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p =>
                    Attribute.IsDefined(p, typeof(JsonExtensionDataAttribute)));

            if (extensionDataProperty == null)
                return ValidationResult.Success;

            var extensionData = extensionDataProperty.GetValue(value)
                as IDictionary<string, object>;

            if (extensionData == null || extensionData.Count == 0)
                return ValidationResult.Success;

            var unexpectedFields = string.Join(", ", extensionData.Keys);

            return new ValidationResult(
                $"The following properties are not allowed: {unexpectedFields}"
            );
        }
    }
}