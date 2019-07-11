using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gambling.Models;

namespace Gambling.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Stream(){

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult Poke(){
            PokeViewModel viewModel = new PokeViewModel();

            Poker[] poker = new Poker[52];
            //Note: 洗牌
            shuffle(poker);

            //Note: 建立出四個人，空的持有陣列
            Person[] person =new Person[4];
            for (int i =0; i<4; i++){
                person[i] = new Person();
                person[i].perPoker =new Poker[13];
            }
            
            //Note: 發牌
            for(int i =0; i<52; i++ ){
                if(i % 4 ==0)
                    person[0].perPoker[person[0].count++] = poker[i];
                else if (i % 4 == 1)
                    person[1].perPoker[person[1].count++] = poker[i];
                else if (i % 4 == 2)
                    person[2].perPoker[person[2].count++] = poker[i];
                else if (i % 4 == 3)
                    person[3].perPoker[person[3].count++] = poker[i];

            }

            viewModel.person = person;
            viewModel.data = new string[4];

            for(int i = 0; i< 4; i++){
                string temp = "";
                for(int j =0; j<13 ;j++){
                    temp += person[i].perPoker[j].suit +" " + person[i].perPoker[j].value +" / ";
                }
                viewModel.data[i] = temp;
            }

            viewModel.person1 = viewModel.data[0];
            viewModel.person2 = viewModel.data[1];
            viewModel.person3 = viewModel.data[2];
            viewModel.person4 = viewModel.data[3];
            return View(viewModel);
        }

        //洗牌邏輯
        private static void shuffle(Poker[] poker){
            for (int i =0; i<4; i++){
                for (int j =0; j <13; j++){
                    poker[i * 13 + j] =new Poker((Suit)i, (Value)j+1);
                }
            }


            for(int i =1; i<=52; i++){
                Random random =new Random();
                int num = random.Next(1, 53);
                Poker temp = poker[i -1];
                poker[i -1] = poker[num -1];
                poker[num -1] =temp;
            }
        }

        public IActionResult Recursively(){
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
