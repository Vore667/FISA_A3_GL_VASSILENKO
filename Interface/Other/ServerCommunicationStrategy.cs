using interface_projet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Other
{
    public class ServerCommunicationStrategy : ICommunicationStrategy
    {
        public event Action<string>? MessageReceived;
        private readonly string _uriPrefix;
        private HttpListener _listener;
        private readonly List<WebSocket> _clients = new List<WebSocket>();

        public ServerCommunicationStrategy(string uriPrefix)
        {
            _uriPrefix = uriPrefix;
            _listener = new HttpListener();
        }

        public async Task StartAsync(CancellationToken token)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_uriPrefix);
            _listener.Start();
            while (!token.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    _ = ProcessClientAsync(context, token);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
            _listener.Stop();
        }

        private async Task ProcessClientAsync(HttpListenerContext context, CancellationToken token)
        {
            try
            {
                var wsContext = await context.AcceptWebSocketAsync(null);
                WebSocket ws = wsContext.WebSocket;
                lock (_clients) { _clients.Add(ws); }
                await ReceiveMessagesAsync(ws, token);
            }
            catch (Exception ex)
            {
                // Gérer l'erreur
                MessageReceived?.Invoke($"Erreur serveur : {ex.Message}");
            }
        }

        private async Task ReceiveMessagesAsync(WebSocket ws, CancellationToken token)
        {
            byte[] buffer = new byte[1024 * 4];
            while (ws.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fermeture demandée", token);
                    lock (_clients) { _clients.Remove(ws); }
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    MessageReceived?.Invoke(message);
                    // Exemple de broadcast
                    await BroadcastMessageAsync(message, token);
                }
            }
        }

        public async Task BroadcastMessageAsync(string message, CancellationToken token)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            List<WebSocket> clientsCopy;
            lock (_clients)
            {
                clientsCopy = new List<WebSocket>(_clients);
            }
            foreach (var client in clientsCopy)
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, token);
                }
            }
        }

        // Pour simplifier, on redirige SendAsync vers un broadcast
        public async Task SendAsync(string message)
        {
            await BroadcastMessageAsync(message, CancellationToken.None);
        }
    }
}
