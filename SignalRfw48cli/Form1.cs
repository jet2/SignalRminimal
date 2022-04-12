using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        HubConnection connection;
        public Form1()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/endpoint")
            .WithAutomaticReconnect()
            .Build();
        }


        private void UpdateLabel(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateLabel), new object[] { message });
                return;
            }
            messagesList.Items.Add(message);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            messagesList.Items.Add("Clicked");
            connection.On<string>("Broadcast", (message) => UpdateLabel(message));
            try
            {
                await connection.StartAsync();
                messagesList.Items.Add("Connection started");
                connectButton.Enabled = false;
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }    
    }
}
