public record RegistrationData(
  string name,
  string client_id,
  string identifier,
  SigningKeys[] signing_keys
);

// missing current tenant (see https://github.com/auth0/auth0-cli/issues/773)

public record SigningKeys(
  string subject
);