using System.Security.Cryptography;
using System.Text;

namespace MermaidFlow.Application.Common.Helpers;

public static class HashHelper
{
    public static string ComputeSha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
