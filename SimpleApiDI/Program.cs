var builder = WebApplication.CreateBuilder(args);

// We need to register our service with our application

//builder.Services.AddSingleton<IMyService, MyService>();

//builder.Services.AddScoped<IMyService, MyService>();//you use Scoped if you want a service that is scoped for each request.

builder.Services.AddTransient<IMyService, MyService>();//you use transient if you want a service instance for each middleware. 

var app = builder.Build();

app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    myService.LogCreation("First Middleware");
    await next.Invoke();
});

app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    myService.LogCreation("second Middleware");
    await next.Invoke();
});

app.MapGet("/", (IMyService MyService) =>
{
    //Will show this in the console.
    MyService.LogCreation("Root");
    //Will show this in result of the GET request.
    return Results.Ok("Check the console for service creation logs");
});

app.Run();

//create an interface that has a simple method declaration
public interface IMyService
{
    void LogCreation(string message);
}

//we create a class that implements the interface and has a method that returns a random service ID
public class MyService : IMyService
{
    private readonly int _serviceId;

    public MyService()
    {
        _serviceId = new Random().Next(100000, 999999);
    }

    public void LogCreation(string message)
    {
        Console.WriteLine($"{message} - Service ID:{_serviceId}");
    }
}