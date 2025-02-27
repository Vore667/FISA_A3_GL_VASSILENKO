using interface_projet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Other
{
    public class ClientCommunicationStrategy : ICommunicationStrategy
    {
        public event Action<string>? MessageReceived;
        private ClientWebSocket? _clientWebSocket;

        public Task StartAsync(CancellationToken token)
        {
            _clientWebSocket = new ClientWebSocket();
            return Task.CompletedTask;
        }


        public async Task ConnectAsync(Uri serverUri, CancellationToken token)
        {
            _clientWebSocket = new ClientWebSocket();
            await _clientWebSocket.ConnectAsync(serverUri, token);
            _ = Task.Run(() => ReceiveMessagesAsync(token));
        }

        private async Task ReceiveMessagesAsync(CancellationToken token)
        {
            if (_clientWebSocket == null)
                return; 

            byte[] buffer = new byte[1024];
            while (_clientWebSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    MessageReceived?.Invoke(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fermeture demandée", token);
                }
            }
        }


        public async Task SendAsync(string message)
        {
            if (_clientWebSocket?.State == WebSocketState.Open)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await _clientWebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
