using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace volcanes_api.Utilidades;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class DynamicRouteAttribute : Attribute, IRouteTemplateProvider
{
    public DynamicRouteAttribute(string environmentVariable)
    {
        ArgumentNullException.ThrowIfNull(environmentVariable);

        Template = Environment.GetEnvironmentVariable("BASE_PATH") + environmentVariable;
    }
    
    public string? Template { get; }
    public int? Order { get; }
    public string? Name { get; }
}