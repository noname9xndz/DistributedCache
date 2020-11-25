# DistributedCacheFile

+ Register : 

```
public void ConfigureServices(IServiceCollection services)
{
       //Add Distributed Cache
       services.AddDistributedCacheFile(Configuration);
}

```

+ Use :

```
public class TestController : Controller
{
     private readonly IDistributedCache _cache;
     public TestController(IDistributedCache cache)
     {
         _cache = cache
     }
}

```
