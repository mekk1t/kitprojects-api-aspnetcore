using Microsoft.AspNetCore.Routing;

namespace KP.Api.AspNetCore.ControllerTokenTransformers
{
    /// <summary>
    /// Приводит токен контроллера к нижнему регистру.
    /// </summary>
    public class LowercaseControllerTokenTransformer : IOutboundParameterTransformer
    {
        /// <inheritdoc />
        public string TransformOutbound(object value) => value?.ToString().ToLower();
    }
}
