using Microsoft.AspNetCore.Routing;

namespace KitProjects.MasterChef.WebApplication
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
