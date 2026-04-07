namespace TodoApi.fromKeyedServices
{
    public class BigCache : ICache
    {
        public object Get(string key) => $"Resolving {key} from big cache.";
    }
}
