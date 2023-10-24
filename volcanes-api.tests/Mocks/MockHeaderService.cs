using Microsoft.AspNetCore.Http;
using volcanes_api.Interfaces;

namespace volcanes_api.tests.Mocks;

public class MockHeaderService : IHeaderService
{
    public Task InsertarParametros<T>(HttpContext httpContext, IQueryable<T> queryable)
    {
        return Task.FromResult("");
    }
}