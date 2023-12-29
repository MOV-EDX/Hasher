using LogEngine;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Hasher version 1.1.0\n");
var builder = new StringBuilder();

// CBS log parser from SFG courtesy of Tekno Venus
var parser = new LogParser();
var matches = new List<LogMatch>();
var corruptedFiles = 0;

var sIndex = Array.FindIndex(args, arg => arg.Equals("-source"));
var lIndex = Array.FindIndex(args, arg => arg.Equals("-log"));

var lPath = string.Empty;
var sPath = sIndex > -1 ? args[sIndex + 1] : args[0];

if (lIndex > -1)
{
    lPath = args[lIndex + 1];
    matches = parser.ParseLog(await File.ReadAllTextAsync(lPath));
}

EnumerateAndHashFiles(sPath, matches);

if (corruptedFiles <= 0)
{
    Console.WriteLine("No corrupt files were found.");
}
else
{
    Console.WriteLine(builder.ToString());
}

Console.ReadKey();

void EnumerateAndHashFiles(string path, IList<LogMatch> matches) {
    var isDirectory = Directory.Exists(path);

    if (isDirectory)
    {
        foreach (var filePath in Directory.EnumerateFileSystemEntries(path))
        {
            EnumerateAndHashFiles(filePath, matches);
        }
    }
    else
    {
        using FileStream file = File.Open(path, FileMode.Open, FileAccess.Read);

        // Check if a CBS log has been provided
        if (lIndex < 0)
        {
            builder.AppendLine($"Path: {file.Name}\n");

            SHA256Hash(file);
            SHA1Hash(file);
            MD5Hash(file);
            SHA512Hash(file);
        }
        else
        {
            SHA256HashAndVerify(file, matches);
        }
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

void SHA512Hash(FileStream file)
{
    file.Position = 0;
    var hash = SHA512.HashData(file);
    var sha512 = Convert.ToHexString(hash);
    var base64 = Convert.ToBase64String(hash);

    builder.AppendLine($"SHA512: {sha512}");
    builder.AppendLine($"SHA512 Base64: {base64}\n");
}

void SHA256HashAndVerify(FileStream file, IList<LogMatch> matches)
{
    file.Position = 0;
    var filePath = file.Name.Split(@"\");
    var payloadFile = $@"{filePath[filePath.Length - 2]}\{filePath[filePath.Length - 1]}";

    var hash = SHA256.HashData(file);
    var sha256 = Convert.ToHexString(hash);
    var base64 = Convert.ToBase64String(hash);

    var hashesToVerify = matches.Where(lm => lm.File.Filepath.Equals(payloadFile, StringComparison.InvariantCultureIgnoreCase));

    foreach (var verification in hashesToVerify)
    {
        if (verification.File.ExpectedHash != base64 && verification.File.ExpectedHash != sha256)
        {
            var actualHash = IsBase64String(verification.File.ExpectedHash) ? base64 : sha256;

            builder.AppendLine($"Component: {verification.File.Filepath}");
            builder.AppendLine($"Expected Hash: {verification.File.ExpectedHash}");
            builder.AppendLine($"Actual Hash: {actualHash}\n");

            corruptedFiles++;
        }
    }
}

//SO Post: https://stackoverflow.com/questions/6309379/how-to-check-for-a-valid-base64-encoded-string/54143400#54143400
bool IsBase64String(string base64)
{
    Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
    return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
}