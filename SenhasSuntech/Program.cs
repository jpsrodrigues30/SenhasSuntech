using System.Security.Cryptography;
using System.Text;

namespace Generate4GPassword
{
    public class GeneratePassword
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Application Started!");

            Console.WriteLine("Digite o número do IMEI (Completo): ");
            string imei = Console.ReadLine();

            if (imei == null)
                throw new Exception("Imei é nulo");
            if (imei.Length != 15)
                throw new Exception("Digite o imei completo...");

            Console.WriteLine($"Gerando senha para o imei {imei}...");

            string password = GenerateEncriptedPassword(imei);

            Console.WriteLine(password);
            Console.ReadLine();
        }

        public static string GenerateEncriptedPassword(string imei)
        {
            SHA256 sha = SHA256.Create();
            byte[] hashValue;

            UTF8Encoding utf8Encoding = new UTF8Encoding();
            hashValue = sha.ComputeHash(utf8Encoding.GetBytes(imei));


            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in hashValue)
            {
                stringBuilder.Append(b.ToString("X2"));
            }

            string hashHex = stringBuilder.ToString();

            var hashNumerico = Math.Abs(Convert.ToInt64(hashHex.Substring(0, 15), 16));

            return hashNumerico.ToString();
        }
    }
}


