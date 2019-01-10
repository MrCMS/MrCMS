using System;

namespace MrCMS.Services.Auth
{
    public class Generate2FACode : IGenerate2FACode
    {
        private static readonly char[] Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        public string GenerateCode(int length = 6)
        {
            var rand = new Random();
            var charArray = new char[length];

            for (int i = 0; i < length; i++)
            {
                charArray[i] = Chars[rand.Next(0, Chars.Length)];
            }

            return new string(charArray);
        }
    }
}