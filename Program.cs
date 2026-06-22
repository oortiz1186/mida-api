using SoporteMida.Api.Config;
using SoporteMida.Api.Services;
using SoporteMida.Api.Integrations.Contpaqi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase")
);
builder.Services.Configure<ContpaqiSqlSettings>(
    builder.Configuration.GetSection("ContpaqiSql")
);
builder.Services.AddScoped<ContpaqiSqlService>();


builder.Services.AddSingleton<SupabaseClientService>();
builder.Services.AddScoped<CompanyService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
