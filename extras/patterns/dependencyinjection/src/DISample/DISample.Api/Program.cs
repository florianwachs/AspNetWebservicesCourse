using DISample.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ITimeService, DefaultTimeService>();
builder.Services.AddSingleton<IBookRepository, DummyBookRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

// Define endpoints
app.MapGet("/api/books", (IBookRepository bookRepository, ITimeService timeService) =>
{
    var currentTime = timeService.Now;
    return bookRepository.All();
});

app.MapGet("/api/books/{id}", (string id, IBookRepository bookRepository, ITimeService timeService) =>
{
    var currentTime = timeService.Now;
    return bookRepository.GetBookById(id);
});

app.MapPost("/api/books", (Book book, IBookRepository bookRepository, ITimeService timeService) =>
{
    var currentTime = timeService.Now;
    return bookRepository.Add(book);
});

app.Run();
