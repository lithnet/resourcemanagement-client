using System;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    internal class PullHelper : IPullControl, IPullResponse
    {
        public ItemListType Items { get; set; }

        public string MaxCharacters { get; set; }

        public string MaxElements { get; set; }

        public string MaxTime { get; set; }
    }
}