using System;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramDateBot
{
    class Program
    {
        private static ITelegramBotClient botClient;

        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("YOUR_BOT_TOKEN_HERE");

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

            Console.WriteLine("Bot is running...");
            Console.ReadLine();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var messageText = e.Message.Text.ToLower();
                var chatId = e.Message.Chat.Id;

                if (messageText.StartsWith("/start"))
                {
                    await botClient.SendTextMessageAsync(chatId, "سلام! من ربات تاریخ هستم. می‌توانم تاریخ امروز را بگویم و تبدیل تاریخ بین شمسی و میلادی انجام دهم.\n\nدستورات:\n/today - نمایش تاریخ امروز\n/to_miladi [تاریخ شمسی] - تبدیل به میلادی\n/to_shamsi [تاریخ میلادی] - تبدیل به شمسی");
                }
                else if (messageText.StartsWith("/today"))
                {
                    var persianCalendar = new PersianCalendar();
                    var today = DateTime.Now;
                    var persianDate = persianCalendar.GetYear(today) + "/" +
                                      persianCalendar.GetMonth(today).ToString("00") + "/" +
                                      persianCalendar.GetDayOfMonth(today).ToString("00");
                    await botClient.SendTextMessageAsync(chatId, $"تاریخ امروز: {persianDate} (شمسی) / {today.ToString("yyyy/MM/dd")} (میلادی)");
                }
                else if (messageText.StartsWith("/to_miladi"))
                {
                    try
                    {
                        var parts = messageText.Split(' ');
                        if (parts.Length == 2)
                        {
                            var persianDate = parts[1];
                            var dateParts = persianDate.Split('/');
                            var persianCalendar = new PersianCalendar();
                            var miladiDate = persianCalendar.ToDateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]), 0, 0, 0, 0);
                            await botClient.SendTextMessageAsync(chatId, $"تاریخ میلادی: {miladiDate.ToString("yyyy/MM/dd")}");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, "لطفا تاریخ شمسی را به فرمت صحیح وارد کنید. مثال: /to_miladi 1403/01/01");
                        }
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(chatId, "خطا در تبدیل تاریخ. لطفا تاریخ را به فرمت صحیح وارد کنید.");
                    }
                }
                else if (messageText.StartsWith("/to_shamsi"))
                {
                    try
                    {
                        var parts = messageText.Split(' ');
                        if (parts.Length == 2)
                        {
                            var miladiDate = DateTime.Parse(parts[1]);
                            var persianCalendar = new PersianCalendar();
                            var persianDate = persianCalendar.GetYear(miladiDate) + "/" +
                                              persianCalendar.GetMonth(miladiDate).ToString("00") + "/" +
                                              persianCalendar.GetDayOfMonth(miladiDate).ToString("00");
                            await botClient.SendTextMessageAsync(chatId, $"تاریخ شمسی: {persianDate}");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, "لطفا تاریخ میلادی را به فرمت صحیح وارد کنید. مثال: /to_shamsi 2024/01/01");
                        }
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(chatId, "خطا در تبدیل تاریخ. لطفا تاریخ را به فرمت صحیح وارد کنید.");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "دستور ناشناخته. لطفا از /start برای مشاهده دستورات استفاده کنید.");
                }
            }
        }
    }
}
