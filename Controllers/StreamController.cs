using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gambling.Models;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Gambling.Controllers
{
    //Note: 這是一個Web API 的 WebSocket範例，實作Get方法，提供前端網頁請求upgraded升級為WebSocket
    //由AcceptWebSocketAsync來實作這件事情，升級成功後在調用GetMessages這個方法
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        //Note: 這是一個簡單的WebSocket範例，寫一個Get方法，讓前端可以使用Get請求升級為WebSocket
        // GET api/Stream
        [HttpGet]
        public async Task Get()
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest) //Note: 檢查是否為WebSocket的Get請求，若是一般的Get請求則回傳400錯誤碼
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await GetMessages(context, webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task GetMessages(HttpContext context, WebSocket webSocket)
        {
            var messages = new[]
            {
            "Message1",
            "Message2",
            "Message3",
            "Message4",
            "Message5"
            };

            foreach (var message in messages)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                var arraySegment = new ArraySegment<byte>(bytes);
                await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                Thread.Sleep(2000); //sleeping so that we can see several messages are sent
            }

            //Note: 關閉WebSocket連接
            //await webSocket.SendAsync(new ArraySegment<byte>(null), WebSocketMessageType.Binary, false, CancellationToken.None);
        }
    }
}
