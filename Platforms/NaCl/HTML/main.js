onload = function()
{
	var plugin = document.getElementById('plugin');
	
	handleMessage = function(message)
	{
		if (message.data == 'Mono Initialized')
		{
			document.getElementById('status').innerHTML = 'Mono Initialized';
			plugin.postMessage('LoadAssemblyDlls');
		}
		else if (message.data == 'Mono Loaded Dlls')
		{
			document.getElementById('status').innerHTML = 'Mono Loaded Dlls';
			plugin.postMessage('LoadAssemblyExe');
		}
		else if (message.data == 'Mono Loaded Exe')
		{
			document.getElementById('status').innerHTML = 'Mono Loaded Exe';
			plugin.postMessage('ExecuteAssembly');
		}
		else if (message.data == 'Mono Executed Exe')
		{
			document.getElementById('status').innerHTML = 'Mono Executed Exe';
		}
		else
		{
			var reignMessage = message.data.split('^');
			if (reignMessage.length >= 2 && reignMessage[0] == 'Reign.Core.Message') alert(reignMessage[1]);
			else document.getElementById('output').innerHTML = message.data + '<br>';
		}
	}
	plugin.addEventListener('message', handleMessage, true);	
};

