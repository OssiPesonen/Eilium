using System;
using System.Collections.Generic;
using System.Text;

namespace Svt.Caspar
{
	public class CasparDevice
	{
        internal Svt.Network.ServerConnection Connection { get; private set; }
        private Svt.Network.ReconnectionHelper ReconnectionHelper { get; set; }

        public CasparDeviceSettings Settings { get; private set; }
        public List<Channel> Channels { get; private set; }
        public TemplatesCollection Templates { get; private set; }
        public List<MediaInfo> Mediafiles { get; private set; }
        public List<string> Datafiles { get; private set; }

        public string Version { get; private set; }

        public bool IsConnected { get { return (Connection == null) ? false : Connection.IsConnected; } }

        [Obsolete("This event is obsolete. Use the new ConnectionStatusChanged instead")]
		public event EventHandler<Svt.Network.NetworkEventArgs> Connected;
        [Obsolete("This event is obsolete. Use the new ConnectionStatusChanged instead")]
        public event EventHandler<Svt.Network.NetworkEventArgs> Disconnected;
        [Obsolete("This event is obsolete. Use the new ConnectionStatusChanged instead")]
        public event EventHandler<Svt.Network.NetworkEventArgs> FailedConnect;
        [Obsolete("This event is obsolete.")]
        public event EventHandler<Svt.Network.ExceptionEventArgs> OnAsyncException;

        public event EventHandler<Svt.Network.ConnectionEventArgs> ConnectionStatusChanged;

		public event EventHandler<DataEventArgs> DataRetrieved;
		public event EventHandler<EventArgs> UpdatedChannels;
		public event EventHandler<EventArgs> UpdatedTemplates;
		public event EventHandler<EventArgs> UpdatedMediafiles;
		public event EventHandler<EventArgs> UpdatedDatafiles;

        volatile bool bIsDisconnecting = false;

		public CasparDevice()
		{
            Settings = new CasparDeviceSettings();
            Connection = new Network.ServerConnection();
            Channels = new List<Channel>();
		    Templates = new TemplatesCollection();
		    Mediafiles = new List<MediaInfo>();
		    Datafiles = new List<string>();

            Version = "unknown";

            Connection.ProtocolStrategy = new AMCP.AMCPProtocolStrategy(this);
            Connection.ConnectionStateChanged += server__ConnectionStateChanged;
		}

		#region Server notifications
        void server__ConnectionStateChanged(object sender, Network.ConnectionEventArgs e)
        {
            try
            {
                if (ConnectionStatusChanged != null)
                    ConnectionStatusChanged(this, e);
            }
            catch { }

            if (e.Connected)
            {
                Connection.SendString("VERSION");

                //Ask server for channels
                Connection.SendString("INFO");

                //For compability with legacy users
                try
                {
                    if (Connected != null)
                    {
                        Connection.SendString("TLS");
                        Connected(this, new Svt.Network.NetworkEventArgs(e.Hostname, e.Port));
                    }
                }
                catch { }
            }
            else
            {
                lock (this)
                {
                    try
                    {
                        if (!bIsDisconnecting && Settings.AutoConnect)
                        {
                            Connection.ConnectionStateChanged -= server__ConnectionStateChanged;
                            ReconnectionHelper = new Svt.Network.ReconnectionHelper(Connection, Settings.ReconnectInterval);
                            ReconnectionHelper.Reconnected += ReconnectionHelper_Reconnected;
                            ReconnectionHelper.Start();
                        }
                    }
                    catch { }
                    bIsDisconnecting = false;
                }

                //For compability with legacy users
                try
                {
                    if (Disconnected != null)
                        Disconnected(this, new Svt.Network.NetworkEventArgs(e.Hostname, e.Port));
                }
                catch { }
            }
        }

        void ReconnectionHelper_Reconnected(object sender, Network.ConnectionEventArgs e)
        {
            lock (this)
            {
                ReconnectionHelper.Close();
                ReconnectionHelper = null;
                Connection.ConnectionStateChanged += server__ConnectionStateChanged;
            }
            server__ConnectionStateChanged(Connection, e);
        }
		#endregion

        public void SendString(string command)
        {
            if (IsConnected)
                Connection.SendString(command);
        }
		public void RefreshMediafiles()
		{
			if (IsConnected)
                Connection.SendString("CLS");
		}
		public void RefreshTemplates()
		{
			if (IsConnected)
                Connection.SendString("TLS");
		}
		public void RefreshDatalist()
		{
			if (IsConnected)
                Connection.SendString("DATA LIST");
		}
		public void StoreData(string name, ICGDataContainer data)
		{
            if (IsConnected)
                Connection.SendString(string.Format("DATA STORE \"{0}\" \"{1}\"", name, data.ToAMCPEscapedXml())); 
		}
		public void RetrieveData(string name)
		{
			if (IsConnected)
                Connection.SendString(string.Format("DATA RETRIEVE \"{0}\"", name));
		}

		#region Connection
        public bool Connect(string host, int port)
        {
            return Connect(host, port, false);
        }

        public bool Connect(string host, int port, bool reconnect)
        {
            if (!IsConnected)
            {
                Settings.Hostname = host;
                Settings.Port = port;
                Settings.AutoConnect = reconnect;
                return Connect();
            }
            return false;
        }
        public bool Connect()
		{
			if (!IsConnected)
			{
                Connection.InitiateConnection(Settings.Hostname, Settings.Port);
				return true;
			}
			return false;
		}

		public void Disconnect()
		{
            lock (this)
            {
                bIsDisconnecting = true;
                if (ReconnectionHelper != null)
                {
                    ReconnectionHelper.Close();
                    ReconnectionHelper = null;
                    Connection.ConnectionStateChanged += server__ConnectionStateChanged;
                }
            }

            Connection.CloseConnection();
		}
		#endregion

		#region AMCP-protocol callbacks
		internal void OnUpdatedChannelInfo(List<ChannelInfo> channels)
		{
            List<Channel> newChannels = new List<Channel>();
			
            foreach (ChannelInfo info in channels)
			{
                if (info.ID <= Channels.Count)
                {
                    Channels[info.ID - 1].VideoMode = info.VideoMode;
                    newChannels.Add(Channels[info.ID - 1]);
                }
                else
                    newChannels.Add(new Channel(Connection, info.ID, info.VideoMode));
			}

            Channels = newChannels;

			if (UpdatedChannels != null)
				UpdatedChannels(this, EventArgs.Empty);
		}

		internal void OnUpdatedTemplatesList(List<TemplateInfo> templates)
		{
            TemplatesCollection newTemplates = new TemplatesCollection();
            newTemplates.Populate(templates);
            Templates = newTemplates;

			if (UpdatedTemplates != null)
				UpdatedTemplates(this, EventArgs.Empty);
		}

		internal void OnUpdatedMediafiles(List<MediaInfo> mediafiles)
		{
            Mediafiles = mediafiles;

			if (UpdatedMediafiles != null)
				UpdatedMediafiles(this, EventArgs.Empty);
		}

		internal void OnVersion(string version)
		{
			Version = version;
		}

		internal void OnLoad(string clipname)
		{
		}

		internal void OnLoadBG(string clipname)
		{
		}

		internal void OnUpdatedDataList(List<string> datafiles)
		{
            Datafiles = datafiles;

			if (UpdatedDatafiles != null)
				UpdatedDatafiles(this, EventArgs.Empty);
		}

		internal void OnDataRetrieved(string data)
		{
			if(DataRetrieved != null)
				DataRetrieved(this, new DataEventArgs(data));
		}
		#endregion
	}

	public class DataEventArgs : EventArgs
	{
		public DataEventArgs(string data)
		{
			Data = data;
		}

		public string Data { get; set; }
	}

	public class CasparDeviceSettings
	{
        public const int DefaultReconnectInterval = 5000;

        public CasparDeviceSettings()
        {
            ReconnectInterval = DefaultReconnectInterval;
        }

        public string Hostname { get; set; }
        public int Port { get; set; }
        public bool AutoConnect { get; set; }
        public int ReconnectInterval { get; set; }
    }
}
