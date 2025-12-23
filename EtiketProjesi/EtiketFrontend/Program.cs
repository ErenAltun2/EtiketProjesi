using EtiketFrontend.Services;
using EtiketFrontend.Components;
using EtiketFrontend.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        // Dosya yüklerken baðlantýnýn kopmamasý için süreyi 5 dakikaya çýkarýyoruz
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
        options.HandshakeTimeout = TimeSpan.FromMinutes(2);

        // Büyük resimlerin geçebilmesi için boyut sýnýrýný 100 MB yapýyoruz
        options.MaximumReceiveMessageSize = 100 * 1024 * 1024;
    });

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7267/");
});

builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<ImageApiService>();
builder.Services.AddScoped<EtiketApiService>();
builder.Services.AddScoped<ExportApiService>();


//YENÝ: Authentication Servisleri
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login"; // Yetkisiz giriþlerde buraya yönlendirir
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

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

app.UseAuthentication(); // Kimlik Tespiti
app.UseAuthorization();  // Yetki Kontrolü

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
