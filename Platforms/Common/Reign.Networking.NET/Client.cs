using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Reign.Networking.PC
{
	public class Client : IClient
	{
		#region Properties
		private TcpClient client;
		private IPAddress ipAddress;
		private string ipAddressURL;
		private int port;

		public byte[] Data
		{
			get
			{
				if (client.Available <= 0) return null;

				var networkStream = client.GetStream();
			    var data = new byte[client.ReceiveBufferSize];
			    networkStream.Read(data, 0, client.ReceiveBufferSize);
				return data;
			}

			set
			{
				if (value == null) return;

				var networkStream = client.GetStream();
				networkStream.Write(value, 0, value.Length);
			}
		}

		public bool Connected
		{
			get {return client.Connected;}
		}
		#endregion

		#region Constructors
		public Client(string ipAddress, int port)
		{
			ipAddressURL = ipAddress;
			this.port = port;
			client = new TcpClient();
		}

		public Client(IPAddress ipAddress, int port)
		{
			this.ipAddress = ipAddress;
			this.port = port;
			client = new TcpClient();
			//client.SendBufferSize = ;
			//client.ReceiveBufferSize = ;
		}

		public Client(TcpClient client)
		{
			this.client = client;
		}
		#endregion

		#region Methods
		public void Connect()
		{
			if (!client.Connected)
			{
				if (ipAddressURL != null) client.Connect(ipAddressURL, port);
				else client.Connect(ipAddress, port);
			}
		}

		public void Close()
		{
			if (client.Connected) client.Close();
		}
		#endregion
	}
}
