using Moriarty.Web.Components;
using Moriarty.Web.Data;
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

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddSingleton<PromptLoader>();
builder.Services.AddSingleton<MarkdownService>();
builder.Services.AddSingleton<GameBoardService>();
builder.Services.AddScoped<SemanticKernelService>();
builder.Services.AddScoped<HttpClient>();

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
