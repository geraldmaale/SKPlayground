using AzureAI.Options;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder
    .Services.AddOptions<AzureOpenAI>()
    .Bind(builder.Configuration.GetSection(nameof(AzureOpenAI)))
    .ValidateDataAnnotations()
    .ValidateOnStart();


var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapPost("/upload", async ([AsParameters] FileUploadForm form, HttpContext context, IOptions<AzureOpenAI> options) =>
{
    var request = context.Request;
    if (!request.HasFormContentType) return Results.BadRequest("Please upload a valid file");

    var filePath = await UploadFile(request, form.Name);
    if (filePath is null) return Results.BadRequest("file cannot be empty");

    // Predict
    var result = await PredictImage(filePath, options.Value.ApiKey, options.Value.Endpoint);

    if (result.IsSuccessStatusCode)
    {
        var content = await result.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<dynamic>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return Results.Ok(new ApiResult(filePath, responseData));
    }
    else
    {
        return TypedResults.BadRequest($"Error: {result.StatusCode}, {result.ReasonPhrase}");
    }
}).Accepts<IFormFile>("multipart/form-data");


app.Run();

static async Task<HttpResponseMessage> PredictImage(string filePath, string apiKey, string endpoint)
{
    var encodedImage = Convert.ToBase64String(File.ReadAllBytes(filePath));
    using var httpClient = new HttpClient();

    httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
    var payload = new
    {
        messages = new object[]
        {
                  new {
                      role = "system",
                      content = new object[] {
                          new {
                              type = "text",
                              text = "You are an AI assistant that can describe an image."
                          }
                      }
                  },
                  new {
                      role = "user",
                      content = new object[] {
                          new {
                              type = "image_url",
                              image_url = new {
                                  url = $"data:image/jpeg;base64,{encodedImage}"
                              }
                          },
                          new {
                              type = "text",
                              text = "briefly describe this image?"
                          }
                      }
                  }
        },
        temperature = 0.7,
        top_p = 0.95,
        max_tokens = 800,
        stream = false
    };

    var response = await httpClient.PostAsync(endpoint, new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

    return response;
}

static async Task<string> SaveFileWithName(IFormFile file, string fileSaveName)
{
    var filePath = GetOrCreateFilePath(fileSaveName);
    await using var fileStream = new FileStream(filePath, FileMode.Create);
    await file.CopyToAsync(fileStream);
    return filePath;
}

static string GetOrCreateFilePath(string fileName, string filesDirectory = "uploadFiles")
{
    var directoryPath = Path.Combine("wwwroot", filesDirectory);
    Directory.CreateDirectory(directoryPath);
    return Path.Combine(directoryPath, fileName);
}

static async Task<string> UploadFile(HttpRequest request, string? fileName)
{
    var form = await request.ReadFormAsync();

    if (form.Files.Any() == false)
        return string.Empty;

    var file = form.Files.FirstOrDefault();
    var fileExtension = Path.GetExtension(file!.FileName);
    if (file is null || file.Length == 0)
        return string.Empty;

    var filePath = await SaveFileWithName(file, string.IsNullOrWhiteSpace(fileName) ? file!.FileName : $"{fileName}{fileExtension}");
    return filePath ?? string.Empty;
}

public record FileUploadForm(string? Name);

public record ApiResult(string FilePath, object? Result);

