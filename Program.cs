using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using AbiaSoft.Services;

var builder = WebApplication.CreateBuilder(args);

var emailSettingsSourceIndex = builder.Configuration.Sources
    .ToList()
    .FindIndex(source => source is EnvironmentVariablesConfigurationSource);
if (emailSettingsSourceIndex < 0)
{
    emailSettingsSourceIndex = builder.Configuration.Sources.Count;
}

builder.Configuration.Sources.Insert(emailSettingsSourceIndex, new JsonConfigurationSource
{
    Path = "appsettings.Email.json",
    Optional = false,
    ReloadOnChange = true
});

builder.Services.AddRazorPages();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Run();
