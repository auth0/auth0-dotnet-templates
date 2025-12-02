public class TenantData
{
    public bool active { get; set; }
    public string name { get; set; }

    public TenantData(bool active, string name)
    {
        this.active = active;
        this.name = name;
    }
}
