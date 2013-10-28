using System;
using Graphite;
using System.IO;
using System.Reflection;
using Metricus.Plugin;
using ServiceStack.Text;

namespace Metricus.Plugins
{
	public class GraphiteOut : OutputPlugin, IOutputPlugin
	{
		class GraphiteOutConfig
		{
			public String Hostname { get; set; }
			public int Port { get; set; }
		}


		private PluginManager pm;
		private string graphiteHostname;
		private int graphitePort;

		public GraphiteOut(PluginManager pm) : base(pm) 
		{ 
			var path = Path.GetDirectoryName (Assembly.GetExecutingAssembly().Location);
			var config = JsonSerializer.DeserializeFromString<GraphiteOutConfig> (File.ReadAllText (path + "/config.json"));
			Console.WriteLine ("Loaded config : {0}", config.Dump ());
			this.pm = pm;
			graphiteHostname = config.Hostname;
			graphitePort = config.Port; 
		}

		public override void Work(metric theMetric)
		{
			Console.WriteLine ("Sending graphite metric.");
			using (var client = new GraphiteUdpClient (graphiteHostname, graphitePort, pm.Hostname )) {
				var path = theMetric.category;
				if ( theMetric.instance != "" ) path += "." + theMetric.instance;
				path += "." + theMetric.type;
				path = path.ToLower ();
				client.Send(path, (int)theMetric.value);
			}

		}
	}
}
