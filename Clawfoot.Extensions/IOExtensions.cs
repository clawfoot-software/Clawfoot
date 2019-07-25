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


        // From: https://stackoverflow.com/a/9277503
        /// <summary>
        /// Determines if a file is already in use
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsInUse(this FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
