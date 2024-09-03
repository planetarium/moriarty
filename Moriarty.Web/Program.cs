using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Moriarty.Web.Components;
using Moriarty.Web.Data;
using Moriarty.Web.LLMPlugins;
using Moriarty.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

string dbProvider = builder.Configuration["DatabaseProvider"];
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("AppDbContext");
    switch (dbProvider)
    {
        case "SQLite":
            options.UseSqlite(connectionString);
            break;
        case "Postgres":
            options.UseNpgsql(connectionString);
            break;
    }
});

builder.Services.AddSingleton<PromptLoader>();
builder.Services.AddSingleton<MarkdownService>();
builder.Services.AddSingleton<GameBoardService>();
builder.Services.AddScoped<GameMaster>();
builder.Services.AddScoped<SceneDirector>();
builder.Services.AddScoped<CampaignBuilder>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<CampaignPlugin>();
builder.Services.AddScoped<GameBoardPlugin>();
builder.Services.AddScoped(sp =>
{
    IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
    ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    string apiKey = configuration["OpenAI:ApiKey"];
    string chatCompletionModel = configuration["OpenAI:ChatCompletionModel"];
    string textToImageModel = configuration["OpenAI:TextToImageModel"];

    IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId: chatCompletionModel, apiKey: apiKey)
        .AddOpenAITextToImage(modelId: textToImageModel, apiKey: apiKey)
        .AddOpenAIFiles(apiKey);

    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<CampaignPlugin>());
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<GameBoardPlugin>());
    kernelBuilder.Services.AddSingleton(loggerFactory);
    return kernelBuilder.Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
