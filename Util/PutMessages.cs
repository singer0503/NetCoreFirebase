
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Microsoft.AspNetCore.Http;
using static Gambling.Controllers.FirebaseController;

namespace Gambling.Util
{
    public class PutMessagesUtil{
        public void PutMessages(HttpContext context, WebSocket webSocket)
        {
            var auth = "1N2wjXel9HxzoJB75R9SqE8MkiGZuoFrnZuiIRmF"; 
            var firebaseClient = new FirebaseClient(
              "https://netcore-b6bd7.firebaseio.com",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(auth)
            });

            //Note: 這是一個Observable物件，執行註冊（Subscribe）後
            // var observable = firebaseClient
            //   .Child("MyMoneys")
            //   .AsObservable<MyMoney>()
            //   .Subscribe(d => 
            //     webSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(d.Key)), WebSocketMessageType.Text, true, CancellationToken.None)
            //   );
            
            var IObservable = firebaseClient.Child("MyMoneys").AsObservable<MyMoney>();
            IObservable.Subscribe(x => Console.WriteLine("==="+x.Key));
            IObservable.Subscribe(x => SendMessage( x.Key + " " + ( x.Object ==null? "": x.Object.Title + " " +  x.Object.Cost ) , webSocket));
            //Note: 關閉WebSocket連接 
            //await webSocket.SendAsync(new ArraySegment<byte>(null), WebSocketMessageType.Binary, false, CancellationToken.None);
        }

        public void SendMessage(string message, WebSocket webSocket){
            var bytes = Encoding.UTF8.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes);
            webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

        }

    }
}