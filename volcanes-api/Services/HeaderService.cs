using Microsoft.EntityFrameworkCore;
using volcanes_api.Interfaces;
using volcanes_api.Utilidades;

namespace volcanes_api.Services;

public  class HeaderService: IHeaderService
{
    public HeaderService()
    {
    }

    public async Task InsertarParametros<T>(HttpContext httpContext, IQueryable<T> queryable)
    {
            var cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegistros",cantidad.ToString());
    }
}