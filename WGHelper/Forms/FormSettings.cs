using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq; //Библиотека, что отвечает за работу с XML
using System.Windows.Forms;

namespace WGHelper
{
    public partial class FormSettings : Form
    {
        XDocument settings;//Объект для хранения XML-файла

        public FormSettings()
        {
            settings = XDocument.Load("settings.xml");//Загрузка файла настроек
            InitializeComponent();
            WoTPath_textBox.Text = settings.Element("settings").Element("worldoftanks").Element("client_path").Value;//Вывод текущего пути к клиенту игры
        }

        private void buttonSetWoTFolder_Click(object sender, EventArgs e)//Нажатие на кнопку"Указать путь к клиенту"
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)//Если путь указан
            {
                string path = folderBrowserDialog1.SelectedPath;
                if ((File.Exists(path + "\\WorldOfTanks.exe")) && (File.Exists(path + "\\WoTLauncher.exe")) && (File.Exists(path + "\\version.xml")))//Проверяем, есть ли клиент по заданному пути
                {
                    settings.Element("settings").Element("worldoftanks").Element("client_path").Value = path;//Сохраняем путь в файле настроек
                    settings.Element("settings").Element("worldoftanks").Element("installed").Value = "yes";//Указываем, что путь к клиенту игры есть
                    XDocument ClientProp = XDocument.Load(path + "\\version.xml");//Загружаем файл версии клиента игры
                    settings.Element("settings").Element("worldoftanks").Element("client_version").Value = ClientProp.Element("version.xml").Element("version").Value;//Сохраняем версию клиента в настройках
                    WoTPath_textBox.Text = path;//Выводим путь к клиенту игры на экран
                    settings.Save("settings.xml");//Сохраняем файл настроек на диске
                }
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)//Нажатие на кнопку "Назад"
        {
            this.Dispose();//Закрыть текущее окно и уничтожить все ресурсы в памяти, что использовались в данном окне
        }
    }
}
