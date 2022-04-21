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
            .Build();

            connection.Closed += Connection_Closed;
            connection.On<string>("readerreport", (message) => UpdateReaderReport(message));
            connection.On<string>("readerstate", (message) => UpdateReaderState(message));
            _ = makeConnect();
        }

        private async Task Connection_Closed(Exception arg)
        {
            //throw new NotImplementedException();
            UpdateReaderReport("Disconnected");
            await Task.Delay(1000);
            await makeConnect();
        }

        private async Task makeConnect()
        {
            UpdateReaderReport("makeConnect");
            try
            {
                await connection.StartAsync();
                UpdateReaderReport("Connected");
                //messagesList.Items.Add("Connection started");
                connectButton.Enabled = false;
            }
            catch (Exception ex)
            {
                UpdateReaderReport("Disconnected");
                //messagesList.Items.Add(ex.Message);
                await Task.Delay(1000);
                await makeConnect();
            }
        }

        private void UpdateReaderState(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateReaderState), new object[] { message });
                return;
            }
            if (message == "1")
            {
                Status3.Text = "Подключен";
            }
            if (message == "0")
            {
                Status3.Text = "Отключен";
            }
        }

        private void UpdateReaderReport(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateReaderReport), new object[] { message });
                return;
            }
            if (message== "makeConnect")
            {
                messagesList.Items.Clear();
            }
            messagesList.Items.Add(message);
            if (message == "Connected")
            {
                Status2.Text = "Доступна";
            }
            if (message == "Disconnected")
            {
                Status2.Text = "НеДоступна";
                Status3.Text = "Отключен";
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
           // messagesList.Items.Add("Clicked");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
