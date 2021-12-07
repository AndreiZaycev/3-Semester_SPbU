using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MD5
{
    /// <summary>
    /// Implementation of hash calculator of directory and files
    /// </summary>
    public class Hasher
    {
        private readonly HashAlgorithm _hashAlgorithm;

        public Hasher(HashAlgorithm hashAlgorithm)
        {
            _hashAlgorithm = hashAlgorithm;
        }

        /// <summary>
        /// The main function that calculates the hash function of input path
        /// </summary>
        /// <param name="path">Path to file or directory</param>
        /// <param name="isParallel">if true then calculates the task in parallel otherwise sequentially</param>
        /// <returns>Hash of directory or file</returns>
        public byte[] CalculatePath(string path, bool isParallel)
        {
            if (File.Exists(path))
            {
                return CalculateHashFile(path);
            }
            if (Directory.Exists(path))
            {
                return CalculateHashDirectory(path, isParallel);
            }
            throw new DirectoryNotFoundException();
        }
        
        /// <summary>
        /// Calculates hash of file 
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Hash of file</returns>
        private byte[] CalculateHashFile(string path)
        {
            using var streamReader = new StreamReader(path);
            var file = streamReader.ReadToEnd();
            return _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(file));
        }
        
        /// <summary>
        /// Calculates hash of directory 
        /// </summary>
        /// <param name="path">Path to directory</param>
        /// <param name="isParallel">if true then calculates the task in parallel otherwise sequentially</param>
        /// <returns>Hash of directory</returns>
        private byte[] CalculateHashDirectory(string path, bool isParallel)
        {
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            // Это должно дать независимость
            Array.Sort(files);
            Array.Sort(directories);
            var directoryName = Path.GetDirectoryName(path);
            var result = new StringBuilder(directoryName);
            if (isParallel)
            {
                Parallel.ForEach(files, file =>
                {
                    var buffer = CalculateHashFile(file);
                    result.Append(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                });

                Parallel.ForEach(directories, directory =>
                {
                    var buffer = CalculateHashDirectory(directory, isParallel);
                    result.Append(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                });
            }
            else
            {
                foreach (var file in files)
                {
                    var buffer = CalculateHashFile(file);
                    result.Append(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                }

                foreach (var directory in directories)
                {
                    var buffer = CalculateHashDirectory(directory, isParallel);
                    result.Append(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                }
            }

            return _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(result.ToString()));
        }
    }
}