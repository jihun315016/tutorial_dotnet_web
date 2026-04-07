namespace TodoApi.fromKeyedServices
{
    public class SmallCache : ICache
    {
        public object Get(string key) => $"Resolving {key} from small cache.";
    }
}
