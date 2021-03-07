using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace BotTelegramGame
{
    class Program
    {
        static void Main(string[] args)
        {
            //string token = File.ReadAllText(@"c:\token");
            string token = "1461628324:AAHUfuGdmuSvyw0x0EiMbqAsEEL26lNBKG0";
            TelegramBotClient bot = new TelegramBotClient(token);
            Console.WriteLine($"@{bot.GetMeAsync().Result.Username} start");


            int max = 5;
            string initialMessage = "Я загадываю число. Потом мы по очереди отнимаем от него " +
                "число от 1 до 5. Побеждает тот, кто смог сделать последний ход\n" +
                "restart если число не нравится\n";
            Random rand = new Random();
            Dictionary<long, int> db = new Dictionary<long, int>();


            bot.OnMessage += (s, arg) =>
             {
                 #region var 

                 string msgText = arg.Message.Text;
                 string firstName = arg.Message.Chat.FirstName;
                 string replyMsg = String.Empty;
                 int msgId = arg.Message.MessageId;
                 long chatId = arg.Message.Chat.Id;

                 int user = 0;
                 string path = $"id_{chatId.ToString().Substring(0, 5).Substring(0, 5)}";
                 bool skip = false;

                 Console.WriteLine($"{firstName}: {msgText}");

                 #endregion

                  if (!db.ContainsKey(chatId) 
                 || msgText == "/restart" 
                 || msgText.StartsWith("start")
                 || msgText.ToLower().IndexOf("start")!= -1
                 
                 )
                  {
                      int startGame = rand.Next(20, 30);
                      db[chatId] = startGame;
                      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                      skip = true;
                      replyMsg = initialMessage + $"Загадано число: {db[chatId]}";
                  }
                else
                {
                    if (db[chatId] <= 0) return;

                    int.TryParse(msgText, out user);
                    if (!(user >= 1 && user <= max))
                    {
                        skip = true;
                        replyMsg = $"Обнаружено читерство. Число: {db[chatId]}";
                    }
                    if (!skip)
                    {
                        db[chatId] -= user;

                        replyMsg = $"Ход {firstName}: {user}. Число: {db[chatId]}";
                        if (db[chatId] <= 0)
                        {
                            replyMsg = $"Ура! Победа, {firstName}!"+"\nstart для новой игры";
                            skip = true;
                        }
                    }
                }

                if (!skip)
                {
                    int temp = db[chatId] % (max + 1);
                    if (temp == 0) temp = rand.Next(max) + 1; // 1 2 3 4 5

                    db[chatId] -= temp;
                    replyMsg += $"\nХод БОТА: {temp} Число: {db[chatId]}";
                    if (db[chatId] <= 0) replyMsg = $"Ура! Победа БОТА!";
                }

                //Bitmap image = new Bitmap(400, 400);
                //Graphics graphics = Graphics.FromImage(image);


                 /*graphics.DrawImage(
                     Image.FromFile("img.bmp"),
                     x: 100,
                     y: 10);*/

               /* graphics.DrawString(
                    s: replyMsg,
                    font: new Font("Consolas", 16),
                    brush: Brushes.Blue,
                    x: 10,
                    y: 200);

                path += $@"\file_{DateTime.Now.Ticks}.bmp";
                image.Save(path);*/

                 Console.WriteLine($" >>> {replyMsg}");
                /*bot.SendPhotoAsync(
                    chatId: chatId,
                    //caption: "https://t.me/joinchat/UrUycVAA3BCDlfTWTh2x2w",

                    photo: new InputOnlineFile(new FileStream(path, FileMode.Open)),

                    replyToMessageId: msgId
                    );*/



                 bot.SendTextMessageAsync(
                     chatId: chatId,
                     text: replyMsg,
                     replyToMessageId: msgId
                     );

             };

            bot.StartReceiving();

            Console.ReadLine();









        }
    }
}
