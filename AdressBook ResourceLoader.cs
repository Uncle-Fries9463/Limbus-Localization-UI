namespace AdressBook
{
    using System;
    using System.Text;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// .AdressBook archive files manager
    /// </summary>
    internal class AdressBookTools
    {
        public static byte[] ByteArrayExtend(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        private static string ToHex(string s)
        {
            return string.Concat
            (
                Encoding.UTF8.GetBytes(s)
                    .Select(x => x.ToString("x2"))
                        .ToArray()
            );
        }
        private static string FromHex(string h)
        {
            string[] hexBytes = new string[h.Length / 2];
            for (int i = 0; i < hexBytes.Length; i++) hexBytes[i] = h.Substring(i * 2, 2);

            return Encoding.UTF8.GetString
            (
                hexBytes
                    .Select(value => Convert.ToByte(value, 16))
                        .ToArray()
            );
        }

        private static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
        private static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        private static string ReadLastLine(string filePath, char UntilChar = '\n')
        {
            const int bufferSize = 16384; //https://stackoverflow.com/a/56091135/22964624
            byte[] buffer = new byte[bufferSize];

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan))
            {
                long fileLength = fileStream.Length;
                if (fileLength == 0)
                {
                    return string.Empty;
                }

                long position = fileLength - 1;
                int bytesRead;
                StringBuilder lastLine = new StringBuilder();

                for (; position >= 0; position--)
                {
                    fileStream.Seek(position, SeekOrigin.Begin);
                    bytesRead = fileStream.Read(buffer, 0, 1);

                    if (bytesRead > 0)
                    {
                        if (buffer[0] == UntilChar)
                        {
                            if (lastLine.Length > 0)
                            {
                                break;
                            }
                        }
                        else if (buffer[0] != '\r')
                        {
                            lastLine.Insert(0, (char)buffer[0]);
                        }
                    }
                }
                return lastLine.ToString();
            }
        }
        
        private static byte[] B(string text) => Encoding.UTF8.GetBytes(text);
        //private static string UTF8(byte[] data) => Encoding.UTF8.GetString(data);

        
        private static Dictionary<string, string> PositionalDictions = new()
        {
            ["MainHeader"] = "[ Adress Book archive-type ]\n\0\0hd\u0014d\0\0»\u0002",
            ["HeaderPoint"] = "\n\0\0hd\u0014p\0\0/\u0002",
            ["AdressBook_ContentTable"] = "\n⇲\0afd\u0014\u0013\0\0\u0002",
            ["FileHeader"] = "\u001c\0f\u0014h\u0002",
            ["FileStart"] = "_\0fd\u0014sp\u0002\u0006",
            ["FileEnds"] = "_\0fd\u0014ep\u0006\u0002",

            ["FileExtension"] = "AdressBook",
        };

        private static bool CheckAdressBookCompression(string AdressBookFile)
        {
            bool IsCompressed = false;
            using (FileStream fileStream = new(AdressBookFile, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new(fileStream, Encoding.UTF8))
            {
                char[] buffer = new char[13];
                if (new string(buffer, 0, reader.Read(buffer, 0, buffer.Length)) == "[Compressed] ") IsCompressed = true;
            }
            return IsCompressed;
        }

        /// <summary>
        /// Get "Path\Filename: 0x01, 0x10" content table with adresses of all files inside
        /// </summary>
        private static Dictionary<string, List<long>> RecognizeAdressBook(string AdressBookFile)
        {
            string AdressBookData = FromHex(ReadLastLine(AdressBookFile).Split("_")[0]);
            Dictionary<string, List<long>> AdressBook_ContentTable = [];

            string AdressBook_ContentTable_Page_Name;

            string AdressBookData_Page_IS;
            string AdressBookData_Page_IE;

            List<string> AdressBook_ContentTable_Pages = AdressBookData.Split("_N</>").ToList();
            AdressBook_ContentTable_Pages.RemoveAt(0);

            foreach (var AdressBook_ContentTable_Page in AdressBook_ContentTable_Pages)
            {
                AdressBook_ContentTable_Page_Name = AdressBook_ContentTable_Page.Split("_N<\\>")[0];
                AdressBookData_Page_IS = AdressBook_ContentTable_Page.Split("_IS<\\>")[0].Split("_IS</>")[1];
                AdressBookData_Page_IE = AdressBook_ContentTable_Page.Split("_IE<\\>")[0].Split("_IE</>")[1];

                AdressBook_ContentTable[AdressBook_ContentTable_Page_Name] = [
                    Convert.ToInt64(AdressBookData_Page_IS[2..^0], 16),
                                Convert.ToInt64(AdressBookData_Page_IE[2..^0], 16)
                ];

            }

            return AdressBook_ContentTable;
        }



        /// <summary>
        /// Create AdressBook from folder
        /// </summary>
        public static void AdressBook_Create(string Folder, string InternalName, string OutputFilename, string OutputPath, bool DoCompression = false, int RoundToNearestNBytes = 1000000)
        {
            bool RoundSizeToNearestNBytes = true;
            if (RoundToNearestNBytes == 0) // 14 569 355 output bytesize -> 14 570 000  for value 10000  // Just for perfection
            {
                RoundSizeToNearestNBytes = false;
            }

            Dictionary<string, List<long>> AdressBook_ContentTable = [];
            long AdressTCV1;
            long AdressTCV2;

            int ContentTable_Page_StartLength = Folder.Length + 1;
            byte[] AdressBook_Header = B($"{(DoCompression ? "[Compressed] " : "")}{PositionalDictions["MainHeader"]}{InternalName}{PositionalDictions["HeaderPoint"]}");

            string AdressBook_OutputFilepath = $"{OutputPath}\\{OutputFilename}.{PositionalDictions["FileExtension"]}";
            long AdressBook_PageCounter = 0;

            File.WriteAllBytes(AdressBook_OutputFilepath, AdressBook_Header);

            Dictionary<string, string> AdressBook_Blueprint = [];

            string Enum_FileAbsPath;
            string Enum_FileHeader;
            foreach (string file in Directory.EnumerateFiles(Folder, "*.*", SearchOption.AllDirectories))
            {
                Enum_FileAbsPath = file;
                Enum_FileHeader  = file[ContentTable_Page_StartLength..];
                if (!Enum_FileAbsPath.EndsWith(PositionalDictions["FileExtension"]))
                {
                    AdressBook_Blueprint[Enum_FileAbsPath] = Enum_FileHeader;
                }
            }

            string AdressBook_Page_Br1;
            string AdressBook_Page_Br2;
            byte[] AdressBook_Page_Data;
            long   AdressBook_CurrentSize;
            string AdressBook_ContentTable_Output = "";

            foreach (var AdressBook_Item in AdressBook_Blueprint)
            {
                //Console.WriteLine($"  - Packing {AdressBook_Item.Value}");

                AdressBook_Page_Br1 = $"\n{PositionalDictions["FileHeader"]}{AdressBook_Item.Value}{PositionalDictions["FileStart"]}";
                AdressBook_Page_Br2 = $"{PositionalDictions["FileEnds"]}";
                if (DoCompression)
                {
                    AdressBook_Page_Data = Compress(File.ReadAllBytes(AdressBook_Item.Key));
                }
                else
                {
                    AdressBook_Page_Data = File.ReadAllBytes(AdressBook_Item.Key);
                }
                AdressBook_CurrentSize = new FileInfo(AdressBook_OutputFilepath).Length;

                AdressTCV1 = AdressBook_CurrentSize + B(AdressBook_Page_Br1).Length; // item start index
                AdressTCV2 = AdressTCV1 + AdressBook_Page_Data.Length;               // item end   index

                AdressBook_ContentTable[AdressBook_Item.Value] = [AdressTCV1, AdressTCV2];
                File.AppendAllBytes(
                    AdressBook_OutputFilepath,
                    ByteArrayExtend
                    (
                        [],
                        B(AdressBook_Page_Br1),
                        AdressBook_Page_Data,
                        B(AdressBook_Page_Br2)
                    )
                );
                AdressBook_PageCounter++;
            }

            foreach (var Adress in AdressBook_ContentTable)
            {
                Console.WriteLine($"  - {Adress.Key}: [0x{string.Format("{0:X}", Adress.Value[0])} : 0x{string.Format("{0:X}", Adress.Value[1])}]");
                AdressBook_ContentTable_Output += ToHex($"_N</>{Adress.Key}_N<\\>_IS</>0x{string.Format("{0:X}", Adress.Value[0])}_IS<\\>_IE</>0x{string.Format("{0:X}", Adress.Value[1])}_IE<\\>");
            }

            long OutputSize = 0;
            long RoundNulls = 0;
            if (RoundSizeToNearestNBytes)
            {
                OutputSize = new FileInfo(AdressBook_OutputFilepath).Length + B($"\n{AdressBook_ContentTable_Output}_{AdressBook_PageCounter}").Count();
                RoundNulls = OutputSize;
                while (RoundNulls % RoundToNearestNBytes != 0)
                {
                    RoundNulls++;
                }
                RoundNulls -= OutputSize;

                File.AppendAllBytes(AdressBook_OutputFilepath, B($"{new string('\0', Convert.ToInt32(RoundNulls))}"));
            }
            File.AppendAllBytes(AdressBook_OutputFilepath, B($"\n{AdressBook_ContentTable_Output}_{AdressBook_PageCounter}"));
            //File.SetAttributes(AdressBook_OutputFilepath, FileAttributes.ReadOnly);

            //Console.WriteLine($"\n ¤ [DONE] Adress Book ({AdressBook_PageCounter} Files, {new FileInfo(AdressBook_OutputFilepath).Length} bytes [{OutputSize}o + {RoundNulls}r]) writed as: {AdressBook_OutputFilepath}\n");
        }

        /// <summary>
        /// Unpack AdressBook file to folder
        /// </summary>
        public static void AdressBook_Unleash(string AdressBookFile, string Unpack_Path)
        {
            AdressBookFile = Path.GetFullPath(AdressBookFile);

            bool IsCompressed = CheckAdressBookCompression(AdressBookFile);

            //string AdressBookData = FromHex(ReadLastLine(AdressBookFile).Replace("\0", "")); //Useless
            Dictionary<string, List<long>> AdressBook_ContentTable = RecognizeAdressBook(AdressBookFile);
            long long__Load_Page_StartPos;
            long long__Load_Page_EndPos;


            //Console.WriteLine($"\n ¤ {(ColoredHeaders ? VT100["2"] : "")}Unleashing{VT100["def"]}");
            long Unleash_ItemsTotal = 0;

            string Unleash_ADB_Page_Name;
            string Unleash_ADB_Page_Name_Filename;

            string Unleash_ADB_Object_AppendPath;
            List<string> Unleash_ADB_Object_AppendPath_List;

            long Unleash_FileStream_BytesToRead;
            byte[] Unleash_FileStream_Buffer;
            int Unleash_FileStream_BytesRead;

            foreach (var AdressBook_ContentTable_Page in AdressBook_ContentTable)
            {
                Unleash_ItemsTotal += 1;
                long__Load_Page_StartPos = AdressBook_ContentTable_Page.Value[0];
                long__Load_Page_EndPos = AdressBook_ContentTable_Page.Value[1];
                Unleash_FileStream_BytesToRead = long__Load_Page_EndPos - long__Load_Page_StartPos;
                using (FileStream Unleash_FileStream = new(AdressBookFile, FileMode.Open, FileAccess.Read))
                {
                    Unleash_FileStream.Seek(long__Load_Page_StartPos, SeekOrigin.Begin);

                    Unleash_FileStream_Buffer = new byte[Unleash_FileStream_BytesToRead];
                    Unleash_FileStream_BytesRead = Unleash_FileStream.Read(Unleash_FileStream_Buffer, 0, (int)Unleash_FileStream_BytesToRead);

                    Unleash_ADB_Page_Name = AdressBook_ContentTable_Page.Key;
                    //Console.WriteLine($"  - Processing \"{Unleash_ADB_Page_Name}\"");

                    // Create folders
                    if (Unleash_ADB_Page_Name.Contains('\\'))
                    {
                        Unleash_ADB_Page_Name_Filename = Unleash_ADB_Page_Name.Split("\\")[^1];

                        Unleash_ADB_Object_AppendPath_List = Unleash_ADB_Page_Name.Split("\\").ToList();
                        Unleash_ADB_Object_AppendPath_List.RemoveAt(Unleash_ADB_Object_AppendPath_List.Count - 1);
                        Unleash_ADB_Object_AppendPath = String.Join('\\', Unleash_ADB_Object_AppendPath_List);

                        if (!Unleash_ADB_Object_AppendPath.StartsWith('\\')) Unleash_ADB_Object_AppendPath = $"\\{Unleash_ADB_Object_AppendPath}";
                        Unleash_ADB_Object_AppendPath = $"{Unpack_Path}{Unleash_ADB_Object_AppendPath}";

                        Directory.CreateDirectory(Unleash_ADB_Object_AppendPath);
                        File.WriteAllBytes($"{Unleash_ADB_Object_AppendPath}\\{Unleash_ADB_Page_Name_Filename}", Unleash_FileStream_Buffer);
                    }
                    else
                    {
                        File.WriteAllBytes($"{Unpack_Path}\\{Unleash_ADB_Page_Name}", IsCompressed ? Decompress(Unleash_FileStream_Buffer) : Unleash_FileStream_Buffer);
                    }
                }
            }
            ///Console.WriteLine($"\n ¤ Unpacked {Unleash_ItemsTotal} files at {Unpack_Path}\n");

        }

        /// <returns>
        /// Dictionary with all byte[] files from .AdressBook file
        /// </returns>
        public static Dictionary<string, byte[]> AdressBook_LoadPages(string AdressBook_Path)
        {
            //Console.WriteLine($"Loading interface assets from {Path.GetFullPath(AdressBook_Path)}\n");

            Dictionary<string, byte[]> AdressBook_Pages = [];

            bool IsCompressed = CheckAdressBookCompression(AdressBook_Path);
            Dictionary<string, List<long>> AdressBook_ContentTable = RecognizeAdressBook(AdressBook_Path);

            long long__Load_Page_StartPos;
            long long__Load_Page_EndPos;

            long   Load_PagesTotal = 0;
            string Load_Page_Name;

            long   Load_FileStream_BytesToRead;
            int    Load_FileStream_BytesRead;
            byte[] Load_FileStream_Buffer;

            string AdressBook_PagesCount = ReadLastLine(AdressBook_Path, '_');

            foreach (var AdressBook_ContentTable_Page in AdressBook_ContentTable)
            {
                Load_PagesTotal += 1;
                Load_Page_Name = AdressBook_ContentTable_Page.Key;
                //Console.WriteLine($"({Load_PagesTotal}/{AdressBook_PagesCount}) Loading item \"{Load_Page_Name}\"\n");

                long__Load_Page_StartPos = AdressBook_ContentTable_Page.Value[0];
                long__Load_Page_EndPos = AdressBook_ContentTable_Page.Value[1];

                Load_FileStream_BytesToRead = long__Load_Page_EndPos - long__Load_Page_StartPos;
                using (FileStream Load_FileStream = new(AdressBook_Path, FileMode.Open, FileAccess.Read))
                {
                    Load_FileStream.Seek(long__Load_Page_StartPos, SeekOrigin.Begin);

                    Load_FileStream_Buffer = new byte[Load_FileStream_BytesToRead];///////////////////////////////////////////////
                    Load_FileStream_BytesRead = Load_FileStream.Read(Load_FileStream_Buffer, 0, (int)Load_FileStream_BytesToRead);
                    
                    AdressBook_Pages[Load_Page_Name] = IsCompressed ? Decompress(Load_FileStream_Buffer) : Load_FileStream_Buffer;
                }
            }

            return AdressBook_Pages;
        }
    }
}