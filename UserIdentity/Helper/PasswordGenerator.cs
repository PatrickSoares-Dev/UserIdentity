using System.Text;

namespace UserIdentity.Helper
{
    public static class PasswordGenerator
    {
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder result = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(validCharacters.Length);
                result.Append(validCharacters[index]);
            }

            return result.ToString();
        }
    }
}
