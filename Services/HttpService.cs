using DotaHackLoader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotaHackLoader.Services
{
    public static class HttpService
    {
        public static string dotaPath;
        private static string URL = "http://dotameat.com/api?action=";
        private static HttpClient httpClient;
        public static string Token;

        private static async Task<string> GetAsync(string data)
        {
            try
            {
                httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                var response = await httpClient.GetAsync($"{URL}{data}");
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                httpClient.Dispose();
                return content;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Tuple<bool, string>> GetAuthAsync(string username, SecureString password)
        {
            try
            {
                var lang = Properties.Settings.Default.language;
                IntPtr passwordBSTR = default(IntPtr);
                string insecurePassword = "";
                passwordBSTR = Marshal.SecureStringToBSTR(password);
                insecurePassword = Marshal.PtrToStringBSTR(passwordBSTR);
                var result = await GetAsync($"authenticate&username={username}&password={insecurePassword}");
                if (result == null)
                {
                    var errorMesage = lang == "ru-RU" ? "Неизвестная ошибка. Пожалуйста, попробуйте позже." : "Unknown error. Please try again later";
                    return new Tuple<bool, string>(true, errorMesage);
                }
                return new Tuple<bool, string>(false, result);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(true, ex.Message);
            }
        }

        public static async Task<Tuple<bool, string>> GetUserDataAsync()
        {
            try
            {
                var lang = Properties.Settings.Default.language;
                var userData = await GetAsync($"getUser&token={Token}");
                if (userData == null)
                {
                    var errorMesage = lang == "ru-RU" ? "Не удалось получить данные о пользователе. Пожалуйста, попробуйте позже." : "Couldn't get the userdata. Please try again later";
                    return new Tuple<bool, string>(true, errorMesage);
                }                    
                return new Tuple<bool, string>(false, userData);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(true, ex.Message);
            }
        }

        public static async Task<Tuple<bool, string>> GetPricesAsync()
        {
            try
            {
                var lang = Properties.Settings.Default.language;
                var priceData = await GetAsync("getPrice");
                if (priceData == null)
                {
                    var errorMesage = lang == "ru-RU" ? "Не удалось получить данные о ценах. Пожалуйста, попробуйте позже." : "Couldn't get the prices. Please try again later";
                    return new Tuple<bool, string>(true, errorMesage);
                }                    
                return new Tuple<bool, string>(false, priceData);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(true, ex.Message);
            }
        }

        public static async Task<Tuple<bool, string>> GetHwidAsync(string hwid)
        {
            try
            {
                var lang = Properties.Settings.Default.language;
                var responseResult = await GetAsync($"setHwid&token={Token}&hwid={hwid}");
                if (responseResult == null)
                {
                    var errorMesage = lang == "ru-RU" ? "Не удалось привязать аккаунт к базе. Пожалуйста, попробуйте позже." : "Couldn't bind ur profile to database. Please try again later";
                    return new Tuple<bool, string>(true, errorMesage);
                }
                return new Tuple<bool, string>(false, responseResult);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(true, ex.Message);
            }
        }
    }
}
