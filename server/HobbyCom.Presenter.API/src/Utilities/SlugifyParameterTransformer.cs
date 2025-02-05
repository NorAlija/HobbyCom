using System.Text.RegularExpressions;

namespace HobbyCom.Presenter.API.src.Utilities
{
    /// <summary>
    /// Transforms route names to lowercase, kebab-case
    /// </summary>
    public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        /// <summary>
        /// Converts route parameter to lowercase, removing 'Controller' suffix
        /// </summary>
        /// <param name="value">Route parameter value</param>
        /// <returns>Transformed route parameter</returns>
        public string? TransformOutbound(object? value)
        {
            // Converts "UsersController" to "users"
            return value == null ? null :
                MyRegex().Replace(value?.ToString() ?? string.Empty, "$1-$2").ToLower()
                    .Replace("controller", "")
                    .Trim('-');
        }

        [GeneratedRegex("([a-z])([A-Z])")]
        private static partial Regex MyRegex();
    }
}