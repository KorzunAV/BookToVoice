using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BookToVoice.Core.Reader;

namespace BookToVoice.Core.Extensions
{
    /// <summary>
    /// Методы расширения для работы с текстовыми файлами и файловой системой
    /// </summary>
    public class FileExtensions
    {
        private const string BackupFile = ".bak";
        private const string All = "*";

        public static IEnumerable<FileInfo> EnumerateFindFiles(DirectoryInfo dir, IEnumerable<string> filePatterns, SearchOption searchOption)
        {

            if (filePatterns != null)
            {
                return dir.EnumerateFiles(All, searchOption)
                    .Where(s => filePatterns.Any(p => s.Extension.EndsWith(p)));
            }
            return dir.EnumerateFiles(All, searchOption);
        }

        #region кодировка
        public static Encoding GetEncodingType(string path)
        {
            const string gram = "йцукенгшщзхъфывапролджэячсмитьбюЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮQWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789\n\r ";

            var instr = new BinaryReader(File.OpenRead(path));
            var readCount = (int)instr.BaseStream.Length;
            readCount = readCount > 255 ? 255 : readCount;
            byte[] data = instr.ReadBytes(readCount);
            instr.Close();

            EncodingInfo[] allEnc = Encoding.GetEncodings();

            double bestPrc = 0.1;
            int bestPrcId = -1;

            for (int i = 0; i < allEnc.Length; i++)
            {
                double uCharNum = 0;

                Encoding e = allEnc[i].GetEncoding();
                string bf = e.GetString(data);

                for (int j = 0; j < bf.Length; j++)
                {
                    if (gram.Contains(bf[j]))
                    {
                        uCharNum++;
                    }
                }

                if (uCharNum / bf.Length > bestPrc)
                {
                    bestPrc = uCharNum / bf.Length;
                    bestPrcId = i;
                }
            }

            if (bestPrcId != -1)
            {
                return allEnc[bestPrcId].GetEncoding();
            }
            return null;
        }

        public static void ChangeEncoding(string directory, IEnumerable<string> filePatterns, SearchOption searchOption)
        {
            var directoryInfo = new DirectoryInfo(directory);
            foreach (var fileInfo in EnumerateFindFiles(directoryInfo, filePatterns, searchOption))
            {
                ChangeEncoding(fileInfo.FullName);
            }
        }

        public static void ChangeEncoding(string fileFullName)
        {
            var fileFullNameWithoutExtension = Path.GetFileNameWithoutExtension(fileFullName) ?? string.Empty;
            var enc = GetEncodingType(fileFullName);
            if (!Equals(Encoding.Default, enc))
            {
                using (var sr = new StreamReader(fileFullName, enc))
                {
                    using (var sw = new StreamWriter(fileFullNameWithoutExtension, true, Encoding.Default))
                    {
                        while (!sr.EndOfStream)
                        {
                            sw.WriteLine(sr.ReadLine());
                        }
                    }
                }
                File.Replace(fileFullNameWithoutExtension, fileFullName, fileFullNameWithoutExtension + BackupFile, false);
            }
        }
        
        #endregion кодировка

        #region Поиск текста
        public static IEnumerable<KeyValuePair<string, int>> FindValueInFiles(DirectoryInfo dir, string value)
        {
            var readerFactory = new ReaderFactory
            {
                Readers = new List<BaseReader>
                                                      {
                                                          new DocReader(),
                                                          new TxtReader()
                                                      }
            };

            foreach (FileInfo fileInfo in EnumerateFindFiles(dir, readerFactory.SupportedExtensions, SearchOption.AllDirectories)
                .Where(s => !s.FullName.Contains("$")))
            {
                int position = FindValueInFile(readerFactory, fileInfo, value);
                if (position > 0)
                {
                    yield return new KeyValuePair<string, int>(fileInfo.FullName, position);
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, int>> FindPatternInFiles(DirectoryInfo dir, string pattern)
        {
            var readerFactory = new ReaderFactory
            {
                Readers = new List<BaseReader>
                                                      {
                                                          new DocReader(),
                                                          new TxtReader()
                                                      }
            };

            foreach (FileInfo fileInfo in EnumerateFindFiles(dir, readerFactory.SupportedExtensions, SearchOption.AllDirectories)
                .Where(s => !s.FullName.Contains("$")))
            {
                int position = FindPatternInFile(readerFactory, fileInfo, pattern);
                if (position > 0)
                {
                    yield return new KeyValuePair<string, int>(fileInfo.FullName, position);
                }
            }
        }

        public static int FindValueInFile(ReaderFactory readerFactory, FileInfo fileInfo, string value)
        {
            var source = readerFactory.Read(fileInfo);
            int position = source.IndexOf(value, StringComparison.OrdinalIgnoreCase);
            return position;
        }

        public static int FindPatternInFile(ReaderFactory readerFactory, FileInfo fileInfo, string pattern)
        {
            var source = readerFactory.Read(fileInfo);
            var match = Regex.Match(source, pattern);
            return match.Index;
        }
        #endregion Поиск текста

        public static void DeleteBackupFiles(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            foreach (var fileInfo in directoryInfo.EnumerateFiles(BackupFile, SearchOption.AllDirectories))
            {
                File.Delete(fileInfo.FullName);
            }
        }

        public static void DeleteTextByPatterns(string directory, IEnumerable<string> filePatterns, string[] separator)
        {
            var directoryInfo = new DirectoryInfo(directory);
            foreach (var fileInfo in EnumerateFindFiles(directoryInfo, filePatterns, SearchOption.AllDirectories))
            {
                var fileFullName = fileInfo.FullName;
                var fileFullNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName) ?? string.Empty;
                Encoding enc = GetEncodingType(fileFullName);
                if (enc != null)
                {
                    using (var sr = new StreamReader(fileFullName, enc))
                    {
                        using (var sw = new StreamWriter(fileFullNameWithoutExtension, true, Encoding.Default))
                        {
                            bool flag = false;
                            string textLine = string.Empty;
                            while (!sr.EndOfStream)
                            {
                                if (string.IsNullOrEmpty(textLine))
                                {
                                    flag = true;
                                }

                                textLine += sr.ReadLine();
                                string[] formatComponents = textLine.Split(separator, StringSplitOptions.None);

                                if (formatComponents.Count() == separator.Count() + 1)
                                {
                                    sw.WriteLine(formatComponents[0] + formatComponents[separator.Count()]);
                                    textLine = string.Empty;
                                }
                                else
                                {
                                    if (formatComponents.Count() == 1 || flag)
                                    {
                                        sw.WriteLine(textLine);
                                        textLine = string.Empty;
                                        flag = false;
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(textLine))
                            {
                                sw.WriteLine(textLine);
                            }
                        }
                    }
                    File.Replace(fileFullNameWithoutExtension, fileFullName, fileFullNameWithoutExtension + ".bak", false);
                }
            }
        }

    }
}