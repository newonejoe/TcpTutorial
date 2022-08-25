using PacketDotNet;
using SharpPcap;
using System.Text.RegularExpressions;


namespace Monitor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            UDPMonitor _instance = new UDPMonitor();
            _instance.Initialize(Properties.Settings.Default.IPAddress, Properties.Settings.Default.Port);

            _instance.Start();
            Console.WriteLine("CT45 Monitor Started");
            */

            #region Get the Device List

            string ver = SharpPcap.Pcap.Version;
            Console.WriteLine("SharpPcap {0}", ver);

            CaptureDeviceList devices = CaptureDeviceList.Instance;

            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine("\nThe following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------\n");

            // Print out the available network devices
            foreach (ICaptureDevice dev in devices)
                Console.WriteLine("{0}\n", dev.ToString());



            #endregion

            #region Extract a device from List
            //ICaptureDevice device = devices.Single(d => d.Description == "CT35");
            // ICaptureDevice device = devices[Properties.Settings.Default.DeviceId];
            // ICaptureDevice device = devices[4];
            // ICaptureDevice? device = devices.FirstOrDefault(d => d.MacAddress != null && d.MacAddress.ToString() == "4851C5058CEB"); ;
            ICaptureDevice? device = devices.FirstOrDefault(d => d.Description == "Adapter for loopback traffic capture");
            if (devices == null) {
                return;
            }

            // string filter = "ip and udp";
            string filter = "ip and tcp";

            // device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device.OnPacketArrival += Device_OnPacketArrival;

            int readTimeOutMilliseconds = 1000;

            device.Open( DeviceModes.Promiscuous, readTimeOutMilliseconds);
            device.Filter = filter;

            Console.WriteLine("-- Listening on {0}, hit 'Enter' to stop...", device.Description);

            device.StartCapture();


            #endregion

            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();

            device.StopCapture();

            device.Close();

            Console.WriteLine("Press Any Key to quit!");
            Console.ReadKey();

        }

        private static void Device_OnPacketArrival(object sender, PacketCapture e)
        {

            var rawPacket = e.GetPacket();

            var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

            var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
            if (tcpPacket == null) {
                return;
            }

            if (tcpPacket.DestinationPort == 56677)
            {
                DateTime time = e.Header.Timeval.Date;
                int len = e.Data.Length;
                var data = tcpPacket.PayloadData;
                Console.WriteLine("[Req]: {0}:{1}:{2},{3} Len={4}\n{5}", time.Hour, time.Minute, time.Second, time.Millisecond, len, System.Text.Encoding.UTF8.GetString(data));
                Console.WriteLine(e.Data.ToString());
            }

            if (tcpPacket.SourcePort == 56677)
            {

                DateTime time = e.Header.Timeval.Date;
                int len = e.Data.Length;
                var data = tcpPacket.PayloadData;
                Console.WriteLine("[Req]: {0}:{1}:{2},{3} Len={4}\n{5}", time.Hour, time.Minute, time.Second, time.Millisecond, len, System.Text.Encoding.UTF8.GetString(data));
                Console.WriteLine(e.Data.ToString());
            }
        }

        /*
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            DateTime time = e.Packet.Timeval.Date;
            int len = e.Packet.Data.Length;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
         time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(e.Packet.ToString());
        }
        */
    }
}