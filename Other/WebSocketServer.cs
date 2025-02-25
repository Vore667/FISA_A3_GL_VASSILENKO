using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace interface_projet.Other
{
    public class WebSocketServer
    {
        private readonly HttpListener _httpListener;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ConcurrentBag<WebSocket> _clients = new ConcurrentBag<WebSocket>();

        public WebSocketServer(string uriPrefix)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(uriPrefix);
        }

        // Méthode pour démarrer le serveur sur un thread séparé.
        public async Task RunAsync()
        {
            _httpListener.Start();
            Console.WriteLine("WebSocket Server started...");

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var httpContext = await _httpListener.GetContextAsync();

                    if (httpContext.Request.IsWebSocketRequest)
                    {
                        ProcessWebSocketRequest(httpContext);
                    }
                    else
                    {
                        // Si ce n'est pas une requête WebSocket, on renvoie une erreur 400.
                        httpContext.Response.StatusCode = 400;
                        httpContext.Response.Close();
                    }
                }
            }
            catch (HttpListenerException) { /* Listener arrêté */ }
            finally
            {
                _httpListener.Stop();
            }
        }

        private async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            try
            {
                var wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                WebSocket webSocket = wsContext.WebSocket;
                _clients.Add(webSocket);
                Console.WriteLine("Client connecté.");

                await ReceiveMessages(webSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'acceptation d'un client: " + ex.Message);
            }
        }

        private async Task ReceiveMessages(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fermeture demandée", CancellationToken.None);
                        Console.WriteLine("Client déconnecté.");
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine("Message reçu: " + message);
                        // Traitement du message reçu
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la réception: " + ex.Message);
                    break;
                }
            }
        }

        // Méthode pour envoyer un message à un client spécifique
        public async Task SendMessageToClient(WebSocket client, string message)
        {
            if (client.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
            }
        }

        // Méthode pour diffuser un message à tous les clients connectés
        public async Task BroadcastMessage(string message)
        {
            foreach (var client in _clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    await SendMessageToClient(client, message);
                }
            }
        }

        // Méthode pour arrêter proprement le serveur
        public void Stop()
        {
            _cts.Cancel();
            foreach (var client in _clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down", CancellationToken.None).Wait();
                }
            }
            _httpListener.Stop();
        }
    }
}
