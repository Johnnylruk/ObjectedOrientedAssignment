using System.Security.Cryptography;
using System.Text;

namespace EVotingSystem_SBMM.Helper;

public static class Cryptography
{
    // Need to say why is static!!!!
    // Reference this one as I did not do bymyself
    public static string GenerateHash(this string value)
    {
        var hash = SHA256.Create();
        var encode = new ASCIIEncoding();
        var array = encode.GetBytes(value);
        array = hash.ComputeHash(array);

        var strHexa = new StringBuilder();

        foreach (var item in array)
        {
            strHexa.Append(item.ToString("x2"));
        }

        return strHexa.ToString();
    }
    
    public static int HashVoterId(int voterId)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(voterId.ToString()));
            return BitConverter.ToInt32(hashedBytes, 0);
        }
    }
  

}