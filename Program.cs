using SoporteMida.Api.Config;
using SoporteMida.Api.Services;
using SoporteMida.Api.Integrations.Contpaqi.Services;
using SoporteMida.Api.Workers;
using SoporteMida.Api.Configuration;
using SoporteMida.Api.Services.Sync.Pipeline;
using SoporteMida.Api.Integrations.Contpaqi.Services.Reverse;


var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfiguredOrigins", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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
builder.Services.AddScoped<ContpaqiReverseSyncService>();
builder.Services.Configure<ContpaqiSyncOptions>(
    builder.Configuration.GetSection("ContpaqiSync"));


builder.Services.AddSingleton<SupabaseClientService>();
builder.Services.AddScoped<CompanyService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<SyncPipeline>();

builder.Services.AddScoped<ISyncStage, CompanySyncStage>();
builder.Services.AddScoped<ISyncStage, ContactSyncStage>();
builder.Services.AddScoped<ISyncStage, AgentSyncStage>();
builder.Services.AddScoped<ISyncStage, ReverseCompanySyncStage>();
builder.Services.AddScoped<ISyncStage, ReverseContactSyncStage>();
builder.Services.AddScoped<ISyncStage, ReverseAgentSyncStage>();
builder.Services.AddScoped<ReverseCompanySyncService>();
builder.Services.AddScoped<ReverseContactSyncService>();
builder.Services.AddScoped<ReverseAgentSyncService>();
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

//app.UseHttpsRedirection();
app.UseCors("AllowConfiguredOrigins");
app.UseAuthorization();
app.MapControllers();

app.Run();
