using Bogus;

namespace OAuthServer.Models
{
    public class Helper
    {
        public string GenerateSingaporeNRIC()
        {
            Random rnd = new Random();

            // Choose prefix: S or T (you can include F/G if you want)
            char prefix = rnd.Next(0, 2) == 0 ? 'S' : 'T';

            // Generate 7-digit number with leading zeros allowed
            int digits = rnd.Next(0, 10_000_000);

            string digitsStr = digits.ToString("D7");

            string partialNric = prefix + digitsStr;

            char checksum = CalculateChecksum(partialNric);

            return partialNric + checksum;
        }

       public char CalculateChecksum(string nric)
        {
            // Weight factors for the first 7 digits
            int[] weights = { 2, 7, 6, 5, 4, 3, 2 };

            // Prefix letter values: S=0, T=4 (offset 0 for S, 4 for T)
            int prefixValue = nric[0] == 'T' ? 4 : 0;

            int sum = prefixValue;

            for (int i = 0; i < 7; i++)
            {
                sum += (nric[i + 1] - '0') * weights[i];
            }

            int remainder = sum % 11;

            // Checksum letters for S/T series
            char[] stChecksumLetters = { 'J', 'Z', 'I', 'H', 'G', 'F', 'E', 'D', 'C', 'B', 'A' };

            return stChecksumLetters[remainder];
        }


       public string GenerateSingaporePhoneNumber(Faker faker)
        {
            // First digit: 6, 8, or 9
            char firstDigit = faker.PickRandom(new[] { '6', '8', '9' });

            // Generate 7 random digits
            string restDigits = faker.Random.Replace("#######"); // 7 digits

            return firstDigit + restDigits;
        }

        public string GenerateRMName(Faker faker)
        {
            var randomName = faker.Name.FullName();
            string randomEmptyOrName = faker.PickRandom(new[] { "", randomName });

            return randomEmptyOrName;
        }
    }
}
