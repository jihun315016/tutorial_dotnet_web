namespace TodoApi.EndPoints
{
    public class TodosEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/test1", async context =>
            {
                await context.Response.WriteAsJsonAsync(new { Message = "All todo items" });
            }).WithName("hi");

            app.MapGet("/test2", (LinkGenerator linker) =>
            {
                // The link to the hello route is /test1
                return $"The link to the hello route is {linker.GetPathByName("hi", values: null)}";
            });
        }
    }
}
