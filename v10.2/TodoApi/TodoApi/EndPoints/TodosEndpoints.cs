namespace TodoApi.EndPoints
{
    public class TodosEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/test", async context =>
            {
                await context.Response.WriteAsJsonAsync(new { Message = "All todo items" });
            });
        }
    }
}
