using Microsoft.AspNetCore.Routing.Constraints;

namespace GSMAO.Server
{
    public class Utils
    {        
        public static Exception GetInnermostException(Exception original)
        {
            var innermost = original;

            while (innermost.InnerException != null)
            {
                innermost = innermost.InnerException;
            }

            return innermost;
        }

        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        public static string GetMimeType(string extension)
        {
            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream", // Default
            };
        }
    }
}
