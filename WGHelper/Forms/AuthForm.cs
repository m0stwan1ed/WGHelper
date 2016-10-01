using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WGHelper.Forms
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        XDocument settings;

        string access_token;
        string nickname;
        string account_id;
        string experies_at;

        private void button_Back_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            settings = XDocument.Load("settings.xml");
            string url = webBrowser1.Url.ToString();
            if (url.Contains("status=ok") == true)
            {
                url = url.Replace("https://api.worldoftanks.ru/wot/blank/?&status=ok&", "");
                for (int i = 0; i < url.Length; i++)
                {
                    if (url[i] == '&')
                    {
                        access_token = url.Substring(0, i+1);
                        url = url.Replace(access_token, "");
                        access_token = access_token.Replace("&", "");
                        access_token = access_token.Replace("access_token=", "");
                        settings.Element("settings").Element("wg_open_id").Element("access_token").Value = access_token;
                        break;
                    }
                }
                for (int i=0; i<url.Length; i++)
                {
                    if (url[i] == '&')
                    {
                        nickname = url.Substring(0, i+1);
                        url = url.Replace(nickname, "");
                        nickname = nickname.Replace("&", "");
                        nickname = nickname.Replace("nickname=", "");
                        settings.Element("settings").Element("wg_open_id").Element("nickname").Value = nickname;
                        break;
                    }
                }
                for (int i=0; i<url.Length; i++)
                {
                    if (url[i] == '&')
                    {
                        account_id = url.Substring(0, i+1);
                        url = url.Replace(account_id, "");
                        account_id = account_id.Replace("&", "");
                        account_id = account_id.Replace("account_id=", "");
                        experies_at = url.Replace("expires_at=", "");
                        settings.Element("settings").Element("wg_open_id").Element("account_id").Value = account_id;
                        settings.Element("settings").Element("wg_open_id").Element("experies_at").Value = experies_at;
                        settings.Element("settings").Element("wg_open_id").Element("authorized").Value = "yes";
                        break;
                    }
                }
                this.Dispose();
                settings.Save("settings.xml");
            }
        }
    }
}
