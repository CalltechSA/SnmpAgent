using RemObjects.Mono.Helpers;
using SharpSnmpLib;
using SharpSnmpLib.Messaging;
using SharpSnmpLib.Objects;
using SharpSnmpLib.Pipeline;
using SharpSnmpLib.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SnmpAgent
{
    public partial class MainPage : ContentPage
    {
        public readonly SnmpEngine _engine;
        public MainPage()
        {
            ObjectStore store = new ObjectStore();
            store.Add(new SysDescr());
            store.Add(new SysObjectId());
            store.Add(new SysUpTime());
            store.Add(new SysContact());
            store.Add(new SysName());
            store.Add(new SysLocation());
            store.Add(new SysServices());
            store.Add(new SysORLastChange());
            store.Add(new SysORTable());
            store.Add(new IfNumber());
            store.Add(new IfTable());

            UserRegistry users = new UserRegistry();
            users.Add(new OctetString("neither"), DefaultPrivacyProvider.DefaultPair);
            users.Add(new OctetString("authen"), new DefaultPrivacyProvider(new MD5AuthenticationProvider(new OctetString("authentication"))));
            if (DESPrivacyProvider.IsSupported)
            {
                users.Add(new OctetString("privacy"), new DESPrivacyProvider(new OctetString("privacyphrase"),
                                                                             new MD5AuthenticationProvider(new OctetString("authentication"))));
            }

            if (AESPrivacyProviderBase.IsSupported)
            {
                users.Add(new OctetString("aes"), new AESPrivacyProvider(new OctetString("privacyphrase"), new MD5AuthenticationProvider(new OctetString("authentication"))));
                users.Add(new OctetString("aes192"), new AES192PrivacyProvider(new OctetString("privacyphrase"), new MD5AuthenticationProvider(new OctetString("authentication"))));
                users.Add(new OctetString("aes256"), new AES256PrivacyProvider(new OctetString("privacyphrase"), new MD5AuthenticationProvider(new OctetString("authentication"))));
            }

            GetV1MessageHandler getv1 = new GetV1MessageHandler();
            HandlerMapping getv1Mapping = new HandlerMapping("v1", "GET", getv1);

            GetMessageHandler getv23 = new GetMessageHandler();
            HandlerMapping getv23Mapping = new HandlerMapping("v2,v3", "GET", getv23);

            SetV1MessageHandler setv1 = new SetV1MessageHandler();
            HandlerMapping setv1Mapping = new HandlerMapping("v1", "SET", setv1);

            SetMessageHandler setv23 = new SetMessageHandler();
            HandlerMapping setv23Mapping = new HandlerMapping("v2,v3", "SET", setv23);

            GetNextV1MessageHandler getnextv1 = new GetNextV1MessageHandler();
            HandlerMapping getnextv1Mapping = new HandlerMapping("v1", "GETNEXT", getnextv1);

            GetNextMessageHandler getnextv23 = new GetNextMessageHandler();
            HandlerMapping getnextv23Mapping = new HandlerMapping("v2,v3", "GETNEXT", getnextv23);

            GetBulkMessageHandler getbulk = new GetBulkMessageHandler();
            HandlerMapping getbulkMapping = new HandlerMapping("v2,v3", "GETBULK", getbulk);

            Version1MembershipProvider v1 = new Version1MembershipProvider(new OctetString("public"), new OctetString("public"));
            Version2MembershipProvider v2 = new Version2MembershipProvider(new OctetString("public"), new OctetString("public"));
            Version3MembershipProvider v3 = new Version3MembershipProvider();
            ComposedMembershipProvider membership = new ComposedMembershipProvider(new IMembershipProvider[] { v1, v2, v3 });
            MessageHandlerFactory handlerFactory = new MessageHandlerFactory(new[]
            {
                getv1Mapping,
                getv23Mapping,
                setv1Mapping,
                setv23Mapping,
                getnextv1Mapping,
                getnextv23Mapping,
                getbulkMapping
            });

            SnmpApplicationFactory pipelineFactory = new SnmpApplicationFactory(new RollingLogger(), store, membership, handlerFactory);
            _engine = new SnmpEngine(pipelineFactory, new Listener { Users = users }, new EngineGroup());
            _engine.ExceptionRaised += (sender, e) => DisplayAlert("Exception Raised",e.Exception.ToString(),"OK");

            InitializeComponent();

            IpPicker.Items.Add("All Unassigned");
            foreach (IPAddress address in Dns.GetHostEntry(string.Empty).AddressList.Where(address => !address.IsIPv6LinkLocal))
                IpPicker.Items.Add(address.ToString());
            IpPicker.SelectedItem = IpPicker.Items.FirstOrDefault();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void StartListeners()
        {
            _engine.Listener.ClearBindings();
            int port = int.Parse(PortEntry.Text, CultureInfo.InvariantCulture);
            if (IpPicker.SelectedItem.ToString() == IpPicker.Items.FirstOrDefault())
            {
                if (Socket.OSSupportsIPv4)
                    _engine.Listener.AddBinding(new IPEndPoint(IPAddress.Any, port));

                if (Socket.OSSupportsIPv6)
                    _engine.Listener.AddBinding(new IPEndPoint(IPAddress.IPv6Any, port));

                _engine.Start();
                return;
            }

            IPAddress address = IPAddress.Parse(IpPicker.SelectedItem.ToString());
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                if (!Socket.OSSupportsIPv4)
                {
                    DisplayAlert("Error IPv4 Not Supported", Listener.ErrorIPv4NotSupported, "OK");
                    return;
                }

                _engine.Listener.AddBinding(new IPEndPoint(address, port));
                _engine.Start();
                return;
            }

            if (!Socket.OSSupportsIPv6)
            {
                DisplayAlert("Error IPv6 Not Supported", Listener.ErrorIPv6NotSupported, "OK");
                return;
            }

            _engine.Listener.AddBinding(new IPEndPoint(address, port));
            _engine.Start();
        }

        private void StopListeners() => _engine.Stop();


        private void ActEnabledExecute(object sender, EventArgs e)
        {
            //ActEnabledExecuteButton.Text = (ActEnabledExecuteButton.Text == "Start listening") ? "Stop listening" : "Start listening";
            //ActEnabledExecuteButton.BackgroundColor = (ActEnabledExecuteButton.BackgroundColor == Color.Green) ? Color.Red : Color.Green;

            if (_engine.Active)
            {
                StopListeners();
                ActEnabledExecuteButton.Text = "Start listening";
                ActEnabledExecuteButton.BackgroundColor = Color.Green;
                PortEntry.IsEnabled = true;
                IpPicker.IsEnabled = true;
                return;
            }

            //if (SnmpMessageExtension.IsRunningOnMono && PlatformSupport.Platform != PlatformType.Windows &&
            //    Mono.Unix.Native.Syscall.getuid() != 0 && int.Parse(PortEntry.Text, CultureInfo.InvariantCulture) < 1024)
            //{
            //    DisplayAlert("Port permissions", @"On Linux this application must be run as root for port < 1024.", "OK");
            //    return;
            //}
            if (SnmpMessageExtension.IsRunningOnMono && PlatformSupport.Platform != PlatformType.Windows &&
                Environment.OSVersion.ToString() == "Unix" && int.Parse(PortEntry.Text, CultureInfo.InvariantCulture) < 1024)
            {
                DisplayAlert("Port permissions", @"On Linux/Unix this application must be run as root for port < 1024.", "OK");
                return;
            }

            try
            {
                StartListeners();
                ActEnabledExecuteButton.Text = "Stop listening";
                ActEnabledExecuteButton.BackgroundColor = Color.Red;
                PortEntry.IsEnabled = false;
                IpPicker.IsEnabled = false;
            }
            catch (PortInUseException ex)
            {
                DisplayAlert("Port in use", $@"Port is already in use: {ex.Endpoint}", @"Error","OK");
            }
        }
    }
}
