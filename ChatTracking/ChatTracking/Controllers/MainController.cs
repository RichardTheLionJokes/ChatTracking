using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace ChatTracking.Controllers
{
    static class MainController
    {
        public static List<string> GetUsers() //получить список имён пользователей
        {
            List<string> nameList = new List<string>();

            using (jabberEntities db = new jabberEntities())
            {
                var VCards = db.ofVCards;
                foreach (ofVCard user in VCards)
                {
                    string s = user.vcard;
                    Regex regex = new Regex(@"<NICKNAME>(.*?)</NICKNAME>");
                    Group group = regex.Match(s).Groups[1];
                    nameList.Add(group.ToString());
                }                
            }

            nameList.Sort();
            return nameList;
        }

        public static List<string> GetMessages(string name1, string name2, ushort msgCount, ushort msgSkip) //получить список сообщений
        {
            List<string> messageList = new List<string>();

            using (jabberEntities db = new jabberEntities())
            {
                string user1 = db.ofVCards.FirstOrDefault(u => u.vcard.IndexOf(name1) != -1).username;
                string user2 = db.ofVCards.FirstOrDefault(u => u.vcard.IndexOf(name2) != -1).username;
                var messages = db.ofMessageArchives.Where(m => (m.fromJID.StartsWith(user1) && m.toJID.StartsWith(user2))
                || (m.fromJID.StartsWith(user2) && m.toJID.StartsWith(user1))).OrderByDescending(m => m.sentDate).Skip(msgSkip).Take(msgCount);
                foreach (ofMessageArchive message in messages)
                {
                    string fromJID = message.fromJID.StartsWith(user1) ? name1 : name2;
                    messageList.Add(fromJID + " (" + ConvertFromUnixTime(message.sentDate) + "):\n" + message.body);
                }
            }

            return messageList;
        }

        static DateTime ConvertFromUnixTime(double unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return epoch.AddSeconds((double)Math.Floor(unixTime / 1000));
        }
    }
}