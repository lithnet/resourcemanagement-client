namespace Lithnet.ResourceManagement.Client
{
    public interface IPullControl
    {
        string MaxCharacters { get; set; }

        string MaxElements { get; set; }

        string MaxTime { get; set; }
    }
}
