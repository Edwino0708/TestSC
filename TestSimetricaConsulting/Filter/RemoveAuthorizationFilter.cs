using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TestSimetricaConsulting.Filter
{
    public sealed class RemoveAuthorizationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verifica si el método tiene el atributo AllowAnonymous
            var allowAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if (allowAnonymous)
            {
                // Remueve la seguridad para el método
                operation.Security = null;
            }
        }
    }
}
