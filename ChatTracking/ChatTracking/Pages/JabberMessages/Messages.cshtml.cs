using ChatTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.Intrinsics.X86;
//using Microsoft.Extensions.Configuration;
using ChatTracking.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using NuGet.Protocol.Plugins;

namespace ChatTracking.Pages.JabberMessages
{
    public class MessagesModel : PageModel
    {
        private readonly JabberContext context;
        //private readonly IConfiguration Configuration;
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string UserName1 { get; set; }
        public string UserName2 { get; set; }
        public List<MessageInfo> messageList = new List<MessageInfo>();
        public PaginatedList<OfMessageArchive> messagePList { get; set; }

        public MessagesModel(JabberContext db)
        {
            context = db;
        }

        public async Task OnGetAsync(string user1, string user2, int? pageIndex)
        {
            User1 = user1;
            User2 = user2 ?? "";
            UserName1 = "";
            UserName2 = "";
            IQueryable<OfMessageArchive> messages;
            List<OfVcard> users;

            Regex regexUser = new Regex(@"<NICKNAME>(.*?)</NICKNAME>"); //поиск имени пользователя в таблице пользователей
            Regex regexMsg = new Regex(@"(.*?)@fileserv"); //поиск имени пользователя в таблице сообщений

            if (!System.String.IsNullOrEmpty(user2))
            {
                try
                {
                    users = context.OfVcards.Where(u => u.Username.StartsWith(user1) || u.Username.StartsWith(user2)).ToList();
                    messages = context.OfMessageArchives.Where(m => (m.FromJid.StartsWith(user1)
                    && m.ToJid.StartsWith(user2)) || (m.FromJid.StartsWith(user2)
                    && m.ToJid.StartsWith(user1))).OrderByDescending(m => m.SentDate);
                    OfVcard? u2 = users.FirstOrDefault(u => u.Username.StartsWith(user2));
                    UserName2 = u2 == null ? "" : regexUser.Match(u2.Vcard).Groups[1].ToString();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                try
                {
                    users = context.OfVcards.ToList();
                    messages = context.OfMessageArchives.Where(m => (m.FromJid.StartsWith(user1)
                    || m.ToJid.StartsWith(user1))).OrderByDescending(m => m.SentDate);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            OfVcard? u1 = users.FirstOrDefault(u => u.Username.StartsWith(user1));
            UserName1 = u1 == null ? "" : regexUser.Match(u1.Vcard).Groups[1].ToString();

            try
            {
                messagePList = await PaginatedList<OfMessageArchive>.CreateAsync(messages, pageIndex ?? 1, 10);
            }
            catch (Exception ex)
            {
                throw;
            }

            foreach (OfMessageArchive message in messagePList)
            {
                MessageInfo messageInfo = new MessageInfo();
                string? FromJid = "";
                if (!System.String.IsNullOrEmpty(user2))
                {
                    FromJid = message.FromJid.StartsWith(user1) ? UserName1 : UserName2;
                }
                else
                {
                    if (message.FromJid.StartsWith(user1))
                    {
                        FromJid = UserName1;
                    }
                    else
                    {
                        OfVcard? sender = users.FirstOrDefault(u => message.FromJid.StartsWith(u.Username));
                        FromJid = sender == null ? regexMsg.Match(message.FromJid).Groups[1].ToString() : regexUser.Match(sender.Vcard).Groups[1].ToString();
                    }
                }
                messageInfo.sender = FromJid;
                messageInfo.date = ConvertFromUnixTime(message.SentDate).ToString();
                messageInfo.body = message.Body;
                messageList.Add(messageInfo);
            }
        }

        private DateTime ConvertFromUnixTime(double unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return epoch.AddSeconds((double)Math.Floor(unixTime / 1000));
        }

        public class MessageInfo
        {
            public string sender;
            public string date;
            public string? body;
        }
    }
}
