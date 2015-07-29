using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Reign.Networking
{
	public interface IClient
	{
		byte[] Data {get; set;}
		bool Connected {get;}

		void Connect();
		void Close();
	}
}
