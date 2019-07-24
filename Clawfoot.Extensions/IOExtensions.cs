using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Clawfoot.Extensions
{
    public static class IOExtensions
    {
        /// <summary>
        /// Generates an MD5 hash for the file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetMd5Hash(this FileInfo file)
        {
            if (!file.Exists)
            {
                throw new InvalidOperationException("Cannot calculate hash for a file that does not exist");
            }

            // Thanks Jon Skeet https://stackoverflow.com/a/10520086
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = file.OpenRead())
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// Compares the MD5 hashes for both files
        /// </summary>
        /// <param name="file"></param>
        /// <param name="otherFile"></param>
        /// <returns></returns>
        public static bool EqualTo(this FileInfo file, FileInfo otherFile)
        {
            return file.GetMd5Hash() == otherFile.GetMd5Hash();
        }
    }
}
