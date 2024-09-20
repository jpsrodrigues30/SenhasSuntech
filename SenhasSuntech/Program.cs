using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Generate4GPassword
{
    public class GeneratePassword
    {
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            Console.WriteLine("Application Started!");

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            var private_key = _configuration["KEYS:PRIVATE_KEY"]?.ToString();

            string[] imei = ["867747070258569", "864454071220002", "867747070218365", "867747070241334", "867747070197973", "867747070256084"];

            Console.WriteLine($"Gerando senha para o imei {imei}...");

            if (private_key != null)
            {
                foreach(var item in imei)
                {
                    string password = GenerateEncriptedPassword(item, private_key);
                    Console.WriteLine($"SENHA PARA O IOT {item}: {password}");
                }
                Console.ReadLine();
            }
            else
                throw new Exception("Private key é nula");
        }

        public static string GenerateEncriptedPassword(string imei, string private_key)
        {
            byte[] imeiBytes = Encoding.UTF8.GetBytes(imei);
            byte[] keyBytes = Encoding.UTF8.GetBytes(private_key);

            byte[] combinedBytes = new byte[imeiBytes.Length + keyBytes.Length];
            Buffer.BlockCopy(imeiBytes, 0, combinedBytes, 0, imeiBytes.Length);
            Buffer.BlockCopy(keyBytes, 0, combinedBytes, imeiBytes.Length, keyBytes.Length);

            int iterations = 10000;
            int numBytesResponse = 18;

            var pbkdf2 = new Rfc2898DeriveBytes(combinedBytes, keyBytes, iterations, HashAlgorithmName.SHA256);
            using (pbkdf2)
            {
                byte[] hashValues = pbkdf2.GetBytes(numBytesResponse);

                StringBuilder hexString = new StringBuilder();
                foreach(byte b in hashValues)
                {
                    hexString.Append(b % 10);
                }

                return hexString.ToString();
            }
        }
    }
}


