using System;
using System.IO;
using System.Text;

namespace NMSResign
{
    class Program
    {
        static string BanksPath = ".";
        static string GetFilePath(string fileName)
        {
            return Path.Combine(BanksPath, fileName);
        }

        static void Main(string[] args)
        {
            bool decrypt = args.Length > 0 && args[0].ToLower() == "-decrypt";
            bool encrypt = args.Length > 0 && args[0].ToLower() == "-encrypt";
            bool createbin = args.Length > 0 && args[0].ToLower() == "-createbin";
            bool createdec = args.Length > 0 && args[0].ToLower() == "-createdec";
            bool help = args.Length == 0 || args[0].ToLower() == "-help";

            string fileName = "BankSignatures.bin";
            if (args.Length > 1)
            {
                fileName = args[1];
            }

            var bankSigsPath = GetFilePath(fileName);
            var bankSigsKey = Encoding.ASCII.GetBytes("ds8r5fV80hbtbDCXXHXPT0dAKMgndVl1");
            Array.Resize(ref bankSigsKey, 0x10); // some reason they use a key larger than 0x10 bytes, even though only first 0x10 is used...

            if (help || (!decrypt && !encrypt && !createbin && !createdec))
            {
                if (args.Length > 0 && !help)
                {
                    Console.WriteLine("Invalid argument");
                }
                Console.WriteLine("Usage:");
                Console.WriteLine("\t-decrypt");
                Console.WriteLine("\t\tDecrypts a .bin file to a .dec file.");
                Console.WriteLine("\t-encrypt");
                Console.WriteLine("\t\tEncrypts a .dec file to a .bin file.");
                Console.WriteLine("\t-createbin");
                Console.WriteLine("\t\tCreates a new encrypted bin file from the folder of pak files.");
                Console.WriteLine("\t-createdec");
                Console.WriteLine("\t\tCreates a new decrypted bin.dec file from the folder of pak files.");
                Console.WriteLine("\t-help");
                Console.WriteLine("\t\tDisplays this help message");
                Console.WriteLine("\n\tAll commands also accept a filename as an additional argument, although this is mostly for testing purposes.");
                return;
            }

            if (decrypt)
            {
                if (!File.Exists(bankSigsPath))
                {
                    Console.WriteLine($"Failed to find {bankSigsPath} file...");
                    return;
                }

                var data = File.ReadAllBytes(bankSigsPath);
                data = Xxtea.XXTEA.Decrypt(data, bankSigsKey);
                File.WriteAllBytes(bankSigsPath + ".dec", data);
                Console.WriteLine($"Wrote decrypted {fileName} to {bankSigsPath}.dec");
                return;
            }

            if (encrypt)
            {
                var decFilePath = bankSigsPath + ".dec";
                if (!File.Exists(decFilePath))
                {
                    Console.WriteLine($"Failed to find {decFilePath} file...");
                    return;
                }

                var data = File.ReadAllBytes(decFilePath);
                data = Xxtea.XXTEA.Encrypt(data, bankSigsKey);
                File.WriteAllBytes(bankSigsPath, data);
                Console.WriteLine($"Wrote encrypted {fileName}.dec to {bankSigsPath}");
                return;
            }

            if (createbin || createdec)
            {
                // Create Bank"Signature" file
                var bankSigs = "";
                foreach (var pak in Directory.GetFiles(BanksPath, "*.pak"))
                {
                    var info = new FileInfo(pak);
                    bankSigs += Path.GetFileName(pak).ToUpper() + "," + info.Length.ToString() + "\r\n";
                }
                var bankSigsData = Encoding.ASCII.GetBytes(bankSigs);

                var stream = new MemoryStream();
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(bankSigsData);
                    while (stream.Length % 4 != 0) // If the stream length is not a multiple of 4
                    {
                        writer.Write((byte)0); // Add a zero byte for 4-byte alignment
                    }
                    writer.Write(bankSigsData.Length);
                    bankSigsData = stream.ToArray();
                }

                if (createbin)
                {
                    bankSigsData = Xxtea.XXTEA.Encrypt(bankSigsData, bankSigsKey);
                    File.WriteAllBytes(bankSigsPath, bankSigsData);
                    Console.WriteLine($"Wrote updated {fileName} to {bankSigsPath}");
                }

                if (createdec)
                {
                    File.WriteAllBytes(bankSigsPath + ".dec", bankSigsData);
                    Console.WriteLine($"Wrote updated {fileName} to {bankSigsPath}.dec");
                }
            }
        }
    }
}
