using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace TEST_API
{
    internal class check
    {
        private static string MacAddressHash = mac.GetMacAddressHash();
        public static int strt(string path)
        {
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                if (lines[0].Length > 3)
                {                       //Ключ
                    var requestParameters = new Dictionary<string, object>() {
                        {"key", lines[0]},
                        {"email", lines[1]},
                        {"machine", MacAddressHash},
                        {"program", "reself"},
                    };
                    string json;
                    try
                    {
                        json = FormUpload.MultipartFormDataPost("https://Neurotuners.ru/api/keys/check/", requestParameters);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); return 404; } //интернет ошибка

                    string search;
                    search = "ok";
                    if (json[json.IndexOf(search) + search.Length + 2] == 't')
                    {
                        search = "status";
                        switch (json[json.IndexOf(search) + search.Length + 2])
                        {
                            case '0':
                            case '1':
                                return -1;  //удачная активация из файла
                            default:
                                return 1000;//не удачная активация из файла
                        }
                    }
                }
                else
                {
                    int file = Int32.Parse(lines[0]) + 1;
                    File.WriteAllText(path, file.ToString());
                    return file;//демо режим
                }
            }
            else
            {
                File.WriteAllText(path, "1");
                return 1;//первый запуск, файл создан
            }
            return 0;
        }
        public static char act(string email, string key, string program)
        {
            var requestParameters = new Dictionary<string, object>() {
                {"key", key},
                {"email", email},
                {"machine", MacAddressHash},
                {"program", program},
            };

            string json = "";

            try
            {
                json = FormUpload.MultipartFormDataPost("https://Neurotuners.ru/api/keys/check/", requestParameters);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return 'e'; }

            string search;
            search = "ok";
            if (json[json.IndexOf(search) + search.Length + 2] == 't')
            {
                search = "status";
                return json[json.IndexOf(search) + search.Length + 2];
            }
            return 'e';
        }
    }
}
