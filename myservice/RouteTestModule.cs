using Microsoft.AspNetCore.Http.HttpResults;
public static class RouteTestModule
{
    public static void RegisterTestEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/search", async (CancellationToken token, IHttpClientFactory httpClientFactory) =>
        {
            var httpResponseMessage = await httpClientFactory.CreateClient("default").GetAsync("weatherforecast");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return await httpResponseMessage.Content.ReadAsStringAsync(token);
            }

            return "Nothing found";
        }).WithName("Search").WithOpenApi();

        endpoints.MapGet("item/{id:int:range(1,394958)}", Results<Ok<string>, BadRequest> (int id) =>
        {
            return TypedResults.Ok($"Item {id} found");
        }).AddEndpointFilter(async (invocationContext, next) =>
        {
            var id = invocationContext.GetArgument<int>(0);
            if (id == default)
            {
                return TypedResults.BadRequest();
            }
            return await next(invocationContext);
        }).WithName("GetItem").WithOpenApi(generatedOperation =>
        {
            return generatedOperation;
        });
    }
}