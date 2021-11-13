
using System.Security.Cryptography;
using System.Text;

var password = "qwerty123456";
var salt = Encoding.ASCII.GetBytes("Lorem ipsum sdgfs gerwsv  gfdsg sefrs");
var passwordHash = GeneratePasswordHashUsingSalt(password, salt);
Console.WriteLine($"Pass - {password}, password hash - {passwordHash}");


static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
{
    var iterate = 10000;
    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
    byte[] hash = pbkdf2.GetBytes(20);
    byte[] hashBytes = new byte[36];
    Buffer.BlockCopy(salt, 0, hashBytes, 0, 16);
    Buffer.BlockCopy(hash, 0, hashBytes, 16, 20);
    var passwordHash = Convert.ToBase64String(hashBytes);
    return passwordHash;
}
