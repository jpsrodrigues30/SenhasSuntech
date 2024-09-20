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

            Console.WriteLine("Digite o número do IMEI (Completo): ");
            string imei = Console.ReadLine();

            if (imei == null)
                throw new Exception("Imei é nulo");
            if (imei.Length != 15)
                throw new Exception("Digite o imei completo...");

            Console.WriteLine($"Gerando senha para o imei {imei}...");

            if (private_key != null)
            {
                string password = GenerateEncriptedPassword(imei, private_key);
                Console.WriteLine($"SENHA PARA O IOT: {password}");
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

            var pbkdf2 = new Rfc2898DeriveBytes(combinedBytes, combinedBytes, iterations, HashAlgorithmName.SHA256);
            using (pbkdf2)
            {
                byte[] hashValues = pbkdf2.GetBytes(numBytesResponse);

                StringBuilder hexString = new StringBuilder();
                foreach(byte b in hashValues)
                {
                    hexString.Append(b.ToString("X2"));
                }

                var password = hexString.ToString().Substring(6, 18);

                return password;
            }
        }
    }
}


