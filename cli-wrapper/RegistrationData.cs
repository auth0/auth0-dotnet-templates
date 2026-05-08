using System;

public class RegistrationData
{
    public string name { get; set; } = string.Empty;
    public string client_id { get; set; } = string.Empty;
    public string client_secret { get; set; } = string.Empty;
    public string identifier { get; set; } = string.Empty;
    public SigningKeys[] signing_keys { get; set; } = Array.Empty<SigningKeys>();

    public RegistrationData() { }

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
    public string subject { get; set; } = string.Empty;

    public SigningKeys() { }

    public SigningKeys(string subject)
    {
        this.subject = subject;
    }
}