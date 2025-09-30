public class RegistrationData
{
    public string name { get; set; }
    public string client_id { get; set; }
    public string identifier { get; set; }
    public SigningKeys[] signing_keys { get; set; }

    public RegistrationData(string name, string client_id, string identifier, SigningKeys[] signing_keys)
    {
        this.name = name;
        this.client_id = client_id;
        this.identifier = identifier;
        this.signing_keys = signing_keys;
    }
}

// missing current tenant (see https://github.com/auth0/auth0-cli/issues/773)

public class SigningKeys
{
    public string subject { get; set; }

    public SigningKeys(string subject)
    {
        this.subject = subject;
    }
}