using Microsoft.Extensions.Options;
using SoporteMida.Api.Config;
using Supabase;

namespace SoporteMida.Api.Services;

public class SupabaseClientService
{
    private readonly Supabase.Client _client;

    public SupabaseClientService(IOptions<SupabaseSettings> settings)
    {
        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false
        };

        _client = new Supabase.Client(
            settings.Value.Url,
            settings.Value.Key,
            options
        );
    }

    public Supabase.Client Client => _client;
}