using System.IO;
using CryptoSoft;

namespace Projet_Easy_Save_grp_4.Models
{
    public class FileModel
    {
        private readonly SemaphoreSlim largeFileSemaphore = new(1, 1);
        private static readonly SemaphoreSlim cryptoSemaphore = new(1, 1);

        public async Task<long> CopyFile(string sourceFile, string destFile, CancellationToken cancellationToken)
        {
            const int BufferSize = 1000000; // 1Mo
            long totalBytesCopied = 0;

            try
            {
                using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None);
                byte[] buffer = new byte[BufferSize];
                int bytesRead;
                while ((bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await destStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                    totalBytesCopied += bytesRead;
                }
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(destFile))
                    File.Delete(destFile);

                return totalBytesCopied;
            }
            catch (Exception ex)
            {
                throw new IOException($"Erreur lors de la copie du fichier '{sourceFile}' vers '{destFile}'.", ex);
            }

            return totalBytesCopied;
        }

        public async Task EncryptFile(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                await cryptoSemaphore.WaitAsync(cancellationToken);
                await Task.Run(() => CryptoService.Transformer(filePath, "CESI_EST_MA_CLE_DE_CHIFFREMENT"), cancellationToken);
            }
            finally
            {
                cryptoSemaphore.Release();
            }
        }
    }
}
