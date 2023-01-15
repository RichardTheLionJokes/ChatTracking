using ChatTracking.Models;
using ChatTracking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using static ChatTracking.Pages.JabberMessages.MessagesModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatTracking.Pages.JabberMessages
{
    public class IndexModel : PageModel
    {
        private readonly JabberContext context;
        public string? User1 { get; set; }
        public string? User2 { get; set; }
        public string? SearchUser { get; set; }
        public List<UserInfo> userList = new List<UserInfo>();
        public PaginatedList<OfVcard>? userPList { get; set; }

        public IndexModel(JabberContext db)
        {
            context = db;
        }

        public async Task OnGetAsync(string user1, string user2, string searchUser, int? pageIndex)
        {
            User1 = user1;
            User2 = user2 ?? "";
            SearchUser = searchUser ?? "";

            IQueryable<OfVcard> users;

            users = from u in context.OfVcards
                     select u;

            if (!System.String.IsNullOrEmpty(searchUser))
            {
                users = users.Where(u => u.Vcard.Contains(searchUser));
            }

            try
            {
                userPList = await PaginatedList<OfVcard>.CreateAsync(users, pageIndex ?? 1, 10);
            }
            catch (Exception ex)
            {
                throw;
            }

            foreach (OfVcard user in userPList)
            {
                UserInfo userInfo = new UserInfo();
                userInfo.username = user.Username.Trim();
                string s = user.Vcard;
                Regex regex = new Regex(@"<NICKNAME>(.*?)</NICKNAME>");
                Group group = regex.Match(s).Groups[1];
                userInfo.name = group.ToString();
                userList.Add(userInfo);
            }
        }

        private void FillUserList(OfVcard user)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.username = user.Username.Trim();
            string s = user.Vcard;
            Regex regex = new Regex(@"<NICKNAME>(.*?)</NICKNAME>");
            Group group = regex.Match(s).Groups[1];
            userInfo.name = group.ToString();
            userList.Add(userInfo);
        }
    }

    public class UserInfo
    {
        public string username;
        public string name;
    }
}
