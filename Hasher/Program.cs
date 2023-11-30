using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Hasher version 1.0.0\n");
var builder = new StringBuilder();

var path = args[0];
EnumerateAndHashFiles(path);
Console.WriteLine(builder.ToString());
Console.ReadKey();

void EnumerateAndHashFiles(string path) {
    var isDirectory = Directory.Exists(path);

    if (isDirectory)
    {
        foreach (var filePath in Directory.EnumerateFileSystemEntries(path))
        {
            EnumerateAndHashFiles(filePath);
        }
    }
    else
    {
        using FileStream file = File.Open(path, FileMode.Open, FileAccess.Read);

        builder.AppendLine($"Path: {file.Name}\n");

        SHA256Hash(file);
        SHA1Hash(file);
        MD5Hash(file);
    }
}

void SHA256Hash(FileStream file)
{
    file.Position = 0;
    var hash = SHA256.HashData(file);
    var sha256 = Convert.ToHexString(hash);
    var base64 = Convert.ToBase64String(hash);

    builder.AppendLine($"SHA256: {sha256}");
    builder.AppendLine($"SHA256 Base64: {base64}\n");
}

void MD5Hash(FileStream file)
{
    file.Position = 0;
    var hash = MD5.HashData(file);
    var md5 = Convert.ToHexString(hash);

    builder.AppendLine($"MD5: {md5}\n");
}

void SHA1Hash(FileStream file)
{
    file.Position = 0;
    var hash = SHA1.HashData(file);
    var sha1 = Convert.ToHexString(hash);
    var base64 = Convert.ToBase64String(hash);

    builder.AppendLine($"SHA1: {sha1}");
    builder.AppendLine($"SHA1 Base64: {base64}\n");
}




