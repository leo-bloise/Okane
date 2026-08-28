using System.Diagnostics;

namespace Okane.Api.Infrastructure.Persistence;

public class DatabaseObservability
{
    public static readonly ActivitySource Source = new ActivitySource("Database");
}
