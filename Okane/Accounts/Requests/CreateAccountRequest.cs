using Okane.Authentication;

namespace Okane.Accounts.Requests;

public interface ICreateAccountRequest
{
    public string Name { get; set; }

    public string Description { get; set; }

    public User User { get; set; }
}
