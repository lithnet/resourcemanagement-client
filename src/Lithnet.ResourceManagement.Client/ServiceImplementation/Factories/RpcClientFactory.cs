using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Microsoft.Win32;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    internal class RpcClientFactory : IClientFactory
    {
        public static bool IsFullFramework => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);

        private NamedPipeClientStream clientPipe;
        private string pipeName;

        public IResourceClient ResourceClient
        {
            get; private set;
        }

        public IResourceFactoryClient ResourceFactoryClient
        {
            get; private set;
        }

        public ISearchClient SearchClient
        {
            get; private set;
        }

        public ISchemaClient SchemaClient
        {
            get; private set;
        }

        public string BaseUri
        {
            get; set;
        }

        public string Spn
        {
            get; set;
        }

        public int ConcurrentConnections
        {
            get; set;
        }

        public TimeSpan ConnectTimeout
        {
            get; set;
        }

        public int SendTimeout
        {
            get; set;
        }

        public int RecieveTimeout
        {
            get; set;
        }

        public NetworkCredential Credentials
        {
            get; set;
        }

        private bool UseComHost { get; set; } = false;

        private bool UseExeHost { get; set; } = true;

        private Process hostProcess;


        [DllImport(@"comhost\Lithnet.ResourceManagement.Client.Host.dll")]
        private static extern void OpenPipe(string pipeName);

        public async Task InitializeClientsAsync(ResourceManagementClient rmc)
        {
            var pipe = await GetOrCreateClientPipeAsync(ConnectTimeout, CancellationToken.None).ConfigureAwait(false);
            var jsonClient = new JsonRpc(RpcCore.GetMessageHandler(pipe));

            jsonClient.TraceSource.Switch.Level = SourceLevels.Warning;
            jsonClient.Disconnected += JsonClient_Disconnected;
            jsonClient.ExceptionStrategy = ExceptionProcessing.ISerializable;
            jsonClient.AllowModificationWhileListening = true;
            jsonClient.StartListening();

            var server = jsonClient.Attach<IRpcServer>(new JsonRpcProxyOptions { MethodNameTransform = x => $"Control_{x}" });

            await server.InitializeClientsAsync(BaseUri, Spn, ConcurrentConnections, SendTimeout, RecieveTimeout, Credentials?.UserName, Credentials?.Password).ConfigureAwait(false);

            var resourceChannel = jsonClient.Attach<IResource>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ResourceService));
            var resourceFactoryChannel = jsonClient.Attach<IResourceFactory>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ResourceFactoryService));
            var searchChannel = jsonClient.Attach<ISearch>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.SearchService));
            var schemaChannel = jsonClient.Attach<IMetadataExchange>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.MetadataService));

            ResourceClient = new ResourceClient(rmc, resourceChannel);
            ResourceFactoryClient = new ResourceFactoryClient(resourceFactoryChannel);
            SearchClient = new SearchClient(rmc, searchChannel);
            SchemaClient = new SchemaClient(schemaChannel);
        }

        private void JsonClient_Disconnected(object sender, JsonRpcDisconnectedEventArgs e)
        {
            Trace.WriteLine("Server disconnected");
        }

        private async Task<NamedPipeClientStream> GetOrCreateClientPipeAsync(TimeSpan timeout, CancellationToken token)
        {
            if (clientPipe == null || !clientPipe.IsConnected)
            {
                clientPipe = await CreateClientPipeAsync(timeout, token).ConfigureAwait(false);
                clientPipe.WriteByte(255);
            }

            return clientPipe;
        }

        private async Task<NamedPipeClientStream> CreateClientPipeAsync(TimeSpan timeout, CancellationToken token)
        {
            pipeName = Guid.NewGuid().ToString();

            if (UseComHost)
            {
                var comHost = TryCreateComComponent();
                comHost.OpenPipe(pipeName);
            }
            else if (UseExeHost)
            {
                var expectedPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "native\\Lithnet.ResourceManagement.Client.Host.exe");
                if (!File.Exists(expectedPath))
                {
                    expectedPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lithnet.ResourceManagement.Client.Host.exe");
                    if (!File.Exists(expectedPath))
                    {
                        throw new FileNotFoundException(expectedPath);
                    }

                }

                Trace.WriteLine($"Attempting to invoke {expectedPath}");

                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    Arguments = pipeName,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    FileName = expectedPath,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Path.GetDirectoryName(expectedPath),
                    LoadUserProfile = true, 
                    WindowStyle= ProcessWindowStyle.Hidden,
                };

                hostProcess = Process.Start(startInfo);
                hostProcess.OutputDataReceived += Process_OutputDataReceived;
                hostProcess.ErrorDataReceived += Process_ErrorDataReceived;
                hostProcess.Exited += Process_Exited;
                Trace.WriteLine($"Created host process {hostProcess.Id}");

                if (hostProcess.HasExited)
                {
                    throw new Exception("Host process exited");
                }
            }
            else
            {
                RpcClientFactory.OpenPipe(pipeName);
            }

            var pipe = new NamedPipeClientStream(".", string.Format(RpcCore.PipeNameFormatTemplate, pipeName), PipeDirection.InOut, PipeOptions.Asynchronous, System.Security.Principal.TokenImpersonationLevel.Delegation);
            await pipe.ConnectAsync((int)timeout.TotalMilliseconds, token).ConfigureAwait(false);

            return pipe;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("The host process exited");
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"Error data received: {e.Data}");
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"Output data recieved: {e.Data}");
        }

        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsUserAnAdmin();

        private IComHost TryCreateComComponent()
        {
            IComHost comhost;

            try
            {
                comhost = (IComHost)new ComHost();
            }
            catch (Exception ex) when (ex is FileNotFoundException fex || (ex is COMException cex && cex.HResult == -2147221164))
            {
                var expectedPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "native\\Lithnet.ResourceManagement.Client.Host.dll");
                if (!File.Exists(expectedPath))
                {
                    expectedPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lithnet.ResourceManagement.Client.Host.dll");
                    if (!File.Exists(expectedPath))
                    {
                        throw new FileNotFoundException(expectedPath);
                    }
                }
                string[] runtimeAssemblies = Directory.GetFiles(Environment.ExpandEnvironmentVariables(@"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319"), "*.dll");

                var paths = new List<string>(runtimeAssemblies);
                paths.AddRange(Directory.GetFiles(Path.GetDirectoryName(expectedPath), "*.dll"));

                PathAssemblyResolver resolver = new PathAssemblyResolver(paths);
                using (var x = new System.Reflection.MetadataLoadContext(resolver))
                {
                    var assy = x.LoadFromAssemblyPath(expectedPath);
                    var an = assy.GetName();
                    var version = an.Version;
                    var fullName = an.FullName;
                    var codeBase = new Uri(assy.Location).AbsoluteUri;

                    RegisterCOMComponent("{dbdea27a-3d96-483c-97d1-9047c70141ac}", "Lithnet.ResourceManagement.Client.ComHost", "Lithnet.ResourceManagement.Client.Host.ComHost", fullName, codeBase, version.ToString());
                }

                comhost = (IComHost)new ComHost();
            }

            return comhost;
        }

        private void RegisterCOMComponent(string clsid, string prodId, string className, string assembly, string codeBase, string version)
        {
            RegistryKey root = IsUserAnAdmin() ? Registry.ClassesRoot : Registry.CurrentUser.OpenSubKey(@"Software\Classes", true);

            Console.WriteLine($"Registering COM component in {root.Name}");

            var classRootKey = root.CreateSubKey(prodId, true);
            classRootKey.SetValue(null, className);

            var classClsidKey = classRootKey.CreateSubKey("CLSID", true);
            classClsidKey.SetValue(null, clsid);

            var clssesclisidkey = root.OpenSubKey("CLSID", true);
            var classIdRoot = clssesclisidkey.CreateSubKey(clsid, true);
            classIdRoot.SetValue(null, className);

            var inprockey = classIdRoot.CreateSubKey("InprocServer32");
            inprockey.SetValue(null, "mscoree.dll");
            inprockey.SetValue("ThreadingModel", "Both");
            inprockey.SetValue("Class", className);
            inprockey.SetValue("Assembly", assembly);
            inprockey.SetValue("RuntimeVersion", "v4.0.30319");
            inprockey.SetValue("CodeBase", codeBase);

            inprockey = inprockey.CreateSubKey(version);
            inprockey.SetValue("Class", className);
            inprockey.SetValue("Assembly", assembly);
            inprockey.SetValue("RuntimeVersion", "v4.0.30319");
            inprockey.SetValue("CodeBase", codeBase);

            var progIdKey = classIdRoot.CreateSubKey("ProgId");
            progIdKey.SetValue(null, prodId);

            classIdRoot.CreateSubKey("Implemented Categories\\{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}");
        }

        public IResourceFactoryClient CreateApprovalClient(string endpoint)
        {
            //ResourceFactoryClient client = new ResourceFactoryClient(ResourceManagementClient.wsHttpContextBinding, endpoint);

            //if (creds != null)
            //{
            //    client.ClientCredentials.Windows.ClientCredential = creds;
            //}

            ////#pragma warning disable 0618
            ////            // client.ClientCredentials.Windows.AllowNtlm = this.resourceFactoryClient.ClientCredentials.Windows.AllowNtlm;
            ////#pragma warning restore 0618

            //client.Initialize(this);
            //client.OpenAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //return client;

            throw new NotImplementedException();
        }
    }
}
