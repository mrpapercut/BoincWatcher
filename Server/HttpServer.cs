using System;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Runtime.Serialization.Json;
using BoincManager.Server.Objects;

namespace BoincManager.Server;

public class HttpServer {
    public int Port = 8000;

    private DBConnection dbConn;

    private HttpListener _listener;

    public HttpServer(DBConnection dbConn) {
        this.dbConn = dbConn;
    }

    public void Start() {
        this._listener = new HttpListener();
        this._listener.Prefixes.Add($"http://localhost:{Port}/");
        this._listener.Start();

        Console.WriteLine($"Is listening? {this._listener.IsListening}");

        Receive();
    }

    public void Stop() {
        this._listener.Stop();
    }

    private void Receive() {
        this._listener.BeginGetContext(new AsyncCallback(ListenerCallback), this._listener);
    }

    private void ListenerCallback(IAsyncResult result) {
        if (this._listener.IsListening) {
            var context = this._listener.EndGetContext(result);
            var request = context.Request;

            Receive();

            switch (request.RawUrl) {
                case "/task":
                    if (request.HasEntityBody) {
                        BoincTask requestData = this.ParseTaskJson(request);
                        this.HandlePostTask(requestData);
                    }
                    break;
            }

            // Console.WriteLine($"{request.HttpMethod} {request.Url} {request.RawUrl} HasBody: {request.HasEntityBody}");

            var response = context.Response;
            response.StatusCode = (int) HttpStatusCode.OK;
            if (request.HttpMethod == "OPTIONS") {
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            } else {
                response.ContentType = "text/plain";
            }
            response.AppendHeader("Access-Control-Allow-Origin", "*");
            response.OutputStream.Write(new byte[] { }, 0, 0);
            response.OutputStream.Close();
        }
    }

    private BoincTask ParseTaskJson(HttpListenerRequest request) {
        var body = request.InputStream;
        var encoding = request.ContentEncoding;
        var reader = new StreamReader(body, encoding);

        var settings = new DataContractJsonSerializerSettings {
            DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("yyyy-MM-ddTHH:mm:ss")
        };
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BoincTask), settings);

        BoincTask ParsedRequestObject = (BoincTask)serializer.ReadObject(reader.BaseStream);

        reader.Close();
        body.Close();

        return ParsedRequestObject;
    }

    private void HandlePostTask(BoincTask task) {
        // Console.WriteLine(requestData.GetType());
        // MySQLObjects.Task task = JsonSerializer.Deserialize<MySQLObjects.Task>(requestData, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Console.WriteLine(task.ToJSON());

        int affectedRows = this.dbConn.UpsertTask(task);

        Console.WriteLine($"Affected rows: {affectedRows}");
    }
}
