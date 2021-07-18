using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Examples.Echo
{
    public static class Program
    {

        private static decimal Debt = 0;

        private static TelegramBotClient Bot;

        public static async Task Main()
        {
            Bot = new TelegramBotClient(Configuration.BotToken);

            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;


            await Bot.SendTextMessageAsync(-542968508, "Я начал следить");
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            await Task.Delay(int.MaxValue);
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                var message = messageEventArgs.Message;
                if (message == null || message.Type != MessageType.Text)
                    return;

                if ("Harron".Equals(message.From.Username))
                {
                    Debt += decimal.Parse(message.Text);

                    Console.WriteLine(Debt);
                }

                if ("AlexanderMelashchenko".Equals(message.From.Username))
                {
                    Debt -= decimal.Parse(message.Text);

                    Console.WriteLine(Debt);
                }


                await SendAnswer(message);

            }
            catch (System.FormatException) { }
        }

        static async Task SendAnswer(Message message)
        {
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            Console.WriteLine(message.Chat.Id);
            await Task.Delay(500);

            if (Debt > 0)
            {
                await Bot.SendTextMessageAsync(
               chatId: message.Chat.Id,
                text: $"Саня ты торчишь {Debt} долларов");

                Console.WriteLine($"Саня ты торчишь {Debt} долларов");
            }
            if (Debt < 0)
            {
                await Bot.SendTextMessageAsync(
               chatId: message.Chat.Id,
                text: $" Уважаемый Антон требуеться выплата {-Debt} гривен");

                Console.WriteLine($" Уважаемый Антон требуеться выплата {-Debt} гривен");
            }
            if (Debt == 0)
            {
                await Bot.SendTextMessageAsync(
               chatId: message.Chat.Id,
                text: "Уважаемые, вы в расчёте");

                Console.WriteLine("Уважаемые, вы в расчёте");
            }
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }
    }
}