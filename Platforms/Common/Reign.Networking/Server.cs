namespace Reign.Networking
{
	public interface ServerI
	{
		bool Listening {get;}

		void Start();
		void Stop();
		void Update();
	}
}
