namespace MyWebServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using MyWebServer.Http;
    using MyWebServer.Routing;

    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener listener;

        private readonly RoutingTable routingTable;

        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            listener = new TcpListener(this.ipAddress, port);

            routingTableConfiguration(this.routingTable = new RoutingTable());
        }

        public HttpServer(int port, Action<IRoutingTable> routingTable)
            : this("127.0.0.1", port, routingTable)
        {
        }

        public HttpServer(Action<IRoutingTable> routingTable)
            : this(5000, routingTable)
        {
        }

        public async Task Start()
        {
            this.listener.Start();

            Console.WriteLine($"Server started on port {port}...");
            Console.WriteLine("Listening for requests...");

            while (true)
            {
                var connection = await this.listener.AcceptTcpClientAsync();

                var networkStream = connection.GetStream();

                var requestText = await this.ReadRequest(networkStream);
                try
                {
                    var request = HttpRequest.Parse(requestText);

                    var response = this.routingTable.ExecuteRequest(request);

                    PrepareSession(request, response);

                    LogPipeline(request, response);

                    await WriteResponse(networkStream, response);
                }
                catch (Exception exception)
                {
                    await HandleError(networkStream, exception);
                }

                connection.Close();
            }
        }

        private void LogPipeline(HttpRequest request, HttpResponse response)
        {
            var separator = new string('-', 50);

            var log = new StringBuilder();
            log.AppendLine();
            log.AppendLine(separator);

            log.AppendLine("REQUEST:");
            log.AppendLine(request.ToString());

            log.AppendLine();

            log.AppendLine("RESPONSE");
            log.AppendLine(response.ToString());

            Console.WriteLine(log.ToString());
        }

        private void PrepareSession(HttpRequest request, HttpResponse response)
        {
            response.AddCokie(HttpSession.SeesionCookieName, request.Session.Id);
        }

        private async Task<string> ReadRequest(NetworkStream networkStream)
        {
            var bufferLength = 1024;
            var buffer = new byte[bufferLength];

            var totalBytes = 0;

            var requestBuilder = new StringBuilder();

            do
            {
                var bytesRead = await networkStream.ReadAsync(buffer, 0, bufferLength);

                totalBytes += bytesRead;

                if (totalBytes > 10 * 1024)
                {
                    throw new InvalidOperationException("Request is too large.");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
            while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }

        private async Task HandleError(
            NetworkStream networkStream,
            Exception exception)
        {
            var errorMessage = $"{exception.Message} {Environment.NewLine} {exception.StackTrace}";

            var errorResponse = HttpResponse.ForError(errorMessage);

            await WriteResponse(networkStream, errorResponse);
        }

        private async Task WriteResponse(
            NetworkStream networkStream,
            HttpResponse response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

            await networkStream.WriteAsync(responseBytes);
        }
    }
}
