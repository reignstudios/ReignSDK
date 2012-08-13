using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Reign.Networking.PC
{
	public class Server : ServerI
	{
		#region Properties
		public LinkedList<Client> Clients {get; private set;}
		private TcpListener listenerTCP;

		public bool Listening
		{
			get {return listenerTCP.Server.IsBound;}
		}
		#endregion

		#region Constructors
		public Server(IPAddress ipAddress, int port)
		{
			listenerTCP = new TcpListener(ipAddress, port);
			Clients = new LinkedList<Client>();
		}
		#endregion

		#region Methods
		public void Start()
		{
			if (!listenerTCP.Server.Connected) listenerTCP.Start();
		}

		public void Stop()
		{
			if (listenerTCP.Server.Connected) return;

			foreach (var client in Clients)
			{
				if (client.Connected) client.Close();
			}
			Clients.Clear();

			listenerTCP.Stop();
		}

		public void Update()
		{
			// Remove closed connections
			var closedClients = new LinkedList<Client>();
			foreach (var client in Clients)
			{
				if (!client.Connected)
				{
					client.Close();
					closedClients.AddLast(client);
				}
			}

			foreach (var client in closedClients)
			{
				Clients.Remove(client);
			}

			// Add pending connections
			if (listenerTCP.Pending())
			{
				var newClient = listenerTCP.AcceptTcpClient();
				Clients.AddLast(new Client(newClient));
			}
		}
		#endregion
	}
}
