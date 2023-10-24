using Microsoft.EntityFrameworkCore;

namespace volcanes_api.Utilidades;

public static class HttpContextExtensions
{
    public async static Task InsertarParametros<T>(this HttpContext httpContext,IQueryable<T> queryable)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));
        var cantidad = await queryable.CountAsync();
        httpContext.Response.Headers.Add("cantidadTotalRegistros",cantidad.ToString());
    }
}