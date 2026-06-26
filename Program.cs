using SoporteMida.Api.Config;
using SoporteMida.Api.Services;
using SoporteMida.Api.Integrations.Contpaqi.Services;
using SoporteMida.Api.Workers;
using SoporteMida.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase")
);
builder.Services.Configure<ContpaqiSqlSettings>(
    builder.Configuration.GetSection("ContpaqiSql")
);
builder.Services.AddScoped<ContpaqiSqlService>();
builder.Services.AddScoped<ContpaqiCustomerSyncService>();
builder.Services.AddScoped<ContpaqiContactSyncService>();
builder.Services.AddScoped<ContpaqiAgentSyncService>();
builder.Services.AddHostedService<ContpaqiSyncWorker>();
builder.Services.Configure<ContpaqiSyncOptions>(
    builder.Configuration.GetSection("ContpaqiSync"));


builder.Services.AddSingleton<SupabaseClientService>();
builder.Services.AddScoped<CompanyService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
