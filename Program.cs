using System.Net.Sockets;
using System.Net;
using System.Text;

namespace _21._10_UDP_Multiplayer_game_Server
{
    internal class Program
    {
     
        private const int multicastPort = 11000;
        private const string multicastAddress = "235.5.5.11";
        private UdpClient udpClient;
        private IPEndPoint multicastEndPoint;
        public HashSet<int> Ports = new HashSet<int>();
        static void Main(string[] args)
        {
            Program server = new Program();
            server.Start();
        }

        public Program()
        {
            udpClient = new UdpClient(multicastPort);
            multicastEndPoint = new IPEndPoint(IPAddress.Parse(multicastAddress), multicastPort);
            udpClient.JoinMulticastGroup(IPAddress.Parse(multicastAddress));
        }

        public void Start()
        {
            Console.WriteLine("Сервер готов отправлять и получать уведомления.");

            Task.Run(() => ReceiveMessages());

            Console.ReadKey();
        }

       

        private async Task ReceiveMessages()
        {
            try
            {
                HashSet<string> receivedMessages = new HashSet<string>();
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);


                while (true)
                {
                    var data = udpClient.Receive(ref remoteEndPoint);
                      string str = Encoding.UTF8.GetString(data);
                    if (!Ports.Contains(remoteEndPoint.Port))
                        Ports.Add(remoteEndPoint.Port);
                    if (!receivedMessages.Contains(str))
                    {
                        var parts = str.Split(";");
                        var msg = $"Игрок {remoteEndPoint.Address}:{remoteEndPoint.Port} находится на координатах X = {parts[0]}, Y = {parts[1]}";
                        Console.WriteLine(msg);
                        receivedMessages.Add(str);
                        foreach (var p in Ports)
                        {
                            if (p != remoteEndPoint.Port)
                            {
                                IPEndPoint remotePort = new IPEndPoint(IPAddress.Parse("235.5.5.11"), p);
                                udpClient.Send(data, remotePort);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении сообщения: {ex.Message}");
            }
        }
    }
}
    

