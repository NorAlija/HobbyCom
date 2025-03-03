using System.Text.RegularExpressions;

namespace HobbyCom.Presenter.API.src.Utilities
{
    /// <summary>
    /// Transforms parameter names to spin-case
    /// </summary>
    public class SpinCaseTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value is null) return null;

            // Convert to spin-case
            var stringValue = value.ToString();
            return stringValue is null ? null : Regex.Replace(stringValue, "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}