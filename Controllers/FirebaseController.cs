using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gambling.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Gambling.Models.Firebase;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading;
using Gambling.Util;

namespace Gambling.Controllers
{
    public class FirebaseController : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            var auth = "1N2wjXel9HxzoJB75R9SqE8MkiGZuoFrnZuiIRmF"; // your app secret
            var firebaseClient = new FirebaseClient(
              "https://netcore-b6bd7.firebaseio.com",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(auth)
              });

            var child = firebaseClient.Child("MyMoneys");

            Console.WriteLine("刪除掉所有的資料");
            await child.DeleteAsync();

            Console.WriteLine("產生 10 筆購物紀錄");
            for (int i = 1; i < 10; i++)
            {
                await child.PostAsync<MyMoney>(new MyMoney() //Note: 這邊把 await 拿掉就會沒辦法塞資料
                {
                    Id = Guid.NewGuid(),
                    Title = $"冷泡茶 {i} 瓶",
                    InvoiceNo = $"0000 {i}",
                    Cost = 20 * i,
                });
            }

            Console.WriteLine("列出 Firebase 中所有的紀錄");
            var fooPosts = await child.OnceAsync<MyMoney>();
            foreach (var item in fooPosts)
            {
                Console.WriteLine($"購買商品:{item.Object.Title} 價格:{item.Object.Cost}");
            }

            Console.WriteLine("查詢購物價格小於 90 的紀錄");
            var fooRec = fooPosts.Where(x => x.Object.Cost <= 90);
            foreach (var item in fooRec)
            {
                Console.WriteLine($"購買商品:{item.Object.Title} 價格:{item.Object.Cost}");
            }

            Console.WriteLine("刪除購物價格小於 90 的紀錄");
            var fooRecDeleted = fooPosts.Where(x => x.Object.Cost <= 90);
            foreach (var item in fooRecDeleted)
            {
                await child.Child(item.Key).DeleteAsync();
                Console.WriteLine($"購買商品:{item.Object.Title} 價格:{item.Object.Cost} 已經被刪除");
            }


            Console.WriteLine("列出 Firebase 中所有的紀錄");
            fooPosts = await child.OnceAsync<MyMoney>();
            foreach (var item in fooPosts)
            {
                Console.WriteLine($"購買商品:{item.Object.Title} 價格:{item.Object.Cost}");
            }

            Console.WriteLine("查詢購物價格等於 140 的紀錄");
            var foo140Rec = fooPosts.FirstOrDefault(x => x.Object.Cost == 140);
            foo140Rec.Object.Cost = 666;
            await child.Child(foo140Rec.Key).PutAsync(foo140Rec.Object);
            Console.WriteLine($"購買商品:{foo140Rec.Object.Title} 的價格已經修正為 價格:{foo140Rec.Object.Cost}");

            Console.WriteLine("列出 Firebase 中所有的紀錄");
            fooPosts = await child.OnceAsync<MyMoney>();
            foreach (var item in fooPosts)
            {
                Console.WriteLine($"購買商品:{item.Object.Title} 價格:{item.Object.Cost}");
            }

            Console.WriteLine("Press any key for continuing...");

            IndexAsyncViewModel model= new IndexAsyncViewModel();
            //List<IndexAsyncViewModel> listData = fooPosts.Select(x => new IndexAsyncViewModel{data1 =x.Key}).ToList();
            List<string> listString = fooPosts.Select(x => x.Key + " " + x.Object.Title + " " +x.Object.Cost).ToList();
            model.data1 = String.Join("<br> ", listString.ToArray());
            return View(model);
        }

        public class MyMoney
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string InvoiceNo { get; set; }
            public int Cost { get; set; }
        }

        public async Task<IActionResult> RealtimeFirebaseView()
        {

            return View();
        }

        //[HttpGet]
        //[MiddlewareFilter(typeof(Middleware.RealTimeWSMiddleware))]
        public async Task TestGet()
        {
            Console.WriteLine("in to testGet");
            // ...
        }


        [HttpGet]
        public async Task Get()
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest) //Note: 檢查是否為WebSocket的Get請求，若是一般的Get請求則回傳400錯誤碼
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                //去註冊如果發生事件的Class
                PutMessagesUtil putUtil = new PutMessagesUtil();
                putUtil.PutMessages(context, webSocket);

                var buffer = new byte[1024]; //new byte[1024 * 8];
                var receiveArraySegment = new ArraySegment<byte>(buffer);
                //Note: 只要離開這個Get方法, webSocket就斷線了, 所以先這樣嘗試！
                while (true)
                {
                    //Thread.Sleep(60000);
                    if (context.RequestAborted.IsCancellationRequested) break; //Note: 這邊一定需要，因為前端關閉webSocket會觸發ReceiveAsync，跑完while迴圈再跑到這裡檢查後跳出這個迴圈，關閉執行緒
                    if (webSocket.State != WebSocketState.Open) break;
                    //Note: 到這邊如果沒收到資料會自己沈默，等待 似乎外層改為do..while迴圈就好，反正在這會一直等待收到資料，關鍵點好像在這！！！！！
                    var response = await webSocket.ReceiveAsync(receiveArraySegment, CancellationToken.None); 
                    if (response == null) break;
                    string receiveData = Encoding.UTF8.GetString(buffer,0,response.Count);
                    putUtil.SendMessage(receiveData,webSocket);
                    //if (string.IsNullOrEmpty(response) && webSocket.State != WebSocketState.Open) break;

                    // foreach (var item in _wSConnectionManager.GetAll())
                    // {
                    //     if (item.Value.State == WebSocketState.Open)
                    //     {
                    //         await _wsHanlder.SendMessageAsync(item.Value, response, cancellationToken);
                    //     }
                    //     continue;
                    // }
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private void PutMessages(HttpContext context, WebSocket webSocket)
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
            IObservable.Subscribe(x => Message( x.Key, webSocket));
            //Note: 關閉WebSocket連接
            //await webSocket.SendAsync(new ArraySegment<byte>(null), WebSocketMessageType.Binary, false, CancellationToken.None);
        }

        private void Message(string message, WebSocket webSocket){
            var bytes = Encoding.ASCII.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes);
            webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

        }

        public async Task<IActionResult> RealtimeConsoleTest()
        {
            var auth = "1N2wjXel9HxzoJB75R9SqE8MkiGZuoFrnZuiIRmF"; 
            var firebaseClient = new FirebaseClient(
              "https://netcore-b6bd7.firebaseio.com",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(auth)
              });

            //Note: 這是一個Observable物件，執行註冊（Subscribe）後，只要發生異動會就寫log
            var observable = firebaseClient
              .Child("MyMoneys")
              .AsObservable<MyMoney>()
              .Subscribe(d => Console.WriteLine(d.Key));

            return View();
        }
    }
}