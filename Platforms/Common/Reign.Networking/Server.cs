namespace Reign.Networking
{
	public interface IServer
	{
		bool Listening {get;}

		void Start();
		void Stop();
		void Update();
	}
}
