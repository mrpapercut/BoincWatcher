using BoincWatcher.Agent;
using BoincWatcher.Agent.Models;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BoincManager.Watcher;
public class BoincActions {
    private AppConfig AppConfig;

    private string AuthKey;

    public BoincActions(AppConfig appConfig) {
        this.AppConfig = appConfig;
    }

    public async Task<string> CallSocket(string command) {
        string cleanResponse = "";

        IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 31416);

        using (Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)) {
            await client.ConnectAsync(ipEndPoint);

            string commandToSend = $"<boinc_gui_rpc_request>\n{command}</boinc_gui_rpc_request>\n\u0003";
            byte[] cmdBytes = Encoding.UTF8.GetBytes(commandToSend);

            _ = await client.SendAsync(cmdBytes, SocketFlags.None);

            byte[] buff = new byte[client.ReceiveBufferSize * 2];
            // Console.WriteLine($"ReceiveBufferSize: {client.ReceiveBufferSize}");
            // Console.WriteLine($"Bytes available before read: {client.Available}");

            string response = "";
            int received = await client.ReceiveAsync(buff, SocketFlags.None);
            response += Encoding.UTF8.GetString(buff, 0, received);

            while (client.Available > 0) {
                // Console.WriteLine("Reading more");
                buff = new byte[client.ReceiveBufferSize * 2];
                received = await client.ReceiveAsync(buff, SocketFlags.None);
                response += Encoding.UTF8.GetString(buff, 0, received);
            }
            // Console.WriteLine($"Bytes available after read: {client.Available}");

            // Clean response
            Regex re = new Regex(@"/boinc_gui_rpc_reply>\n.*$", RegexOptions.Multiline);
            cleanResponse = re.Replace(response, "/boinc_gui_rpc_reply>");
        }

        return cleanResponse;
    }

    public async Task<ClientState> GetState() {
        string response = await CallSocket("<get_state/>\n");

        try {
            XElement resXml = XElement.Parse(response);

            XElement clientStateXml = resXml?.Element("client_state");

            if (clientStateXml == null) {
                throw new Exception("Error: client_state is null");
            }

            ClientState parsedState = BoincParser.ParseClientState(clientStateXml);

            return parsedState;
        } catch (Exception ex) {
            throw;
        }
    }

    public void HandleGenefer(object sender, FileSystemEventArgs e, string filecontents) {
        string createdOrChanged = (e.ChangeType == WatcherChangeTypes.Created) ? "created" : "changed";

        AppUtils.LogToFile($"Genefer file {e.Name} {createdOrChanged}\nContents:\n{filecontents}", "genefer.txt");

        // Find corresponding BoincTask
        // Submit updated task to central server
        // Remove from list
    }

    public void PostToServer(string endpoint, string message) {
        string url = $"http://localhost:8080{endpoint}";

        using var client = new HttpClient();
        var result = client.PostAsync(url, new StringContent(message, Encoding.UTF8, "application/json")).Result;

        // Console.WriteLine($"Posted to server: {result}");
    }
}
