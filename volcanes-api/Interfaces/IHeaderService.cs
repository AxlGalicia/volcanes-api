namespace volcanes_api.Interfaces;

public interface IHeaderService
{
    Task InsertarParametros<T>(HttpContext httpContext, IQueryable<T> queryable);
}