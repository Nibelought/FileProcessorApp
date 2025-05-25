using System.Text;
using System.Text.RegularExpressions;

namespace FileProcessorApp
{
    // --- HTML Specific ---
    public class HtmlLoader : ILoader
    {
        public string Load(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true))
            {
                string htmlContent = reader.ReadToEnd();
                // 1. Replace <p> and <br> with newlines
                // Regex to match <p...>, <P...>, <br...>, <BR...> tags, accommodating attributes
                string processedContent = Regex.Replace(htmlContent, @"<[pP][^>]*>", "\n\n", RegexOptions.IgnoreCase); // Paragraphs
                processedContent = Regex.Replace(processedContent, @"<[bB][rR][^>]*?/?>", "\n", RegexOptions.IgnoreCase);  // Line breaks

                // 2. Remove all other HTML tags
                processedContent = Regex.Replace(processedContent, @"<[^>]+>", string.Empty);

                // 3. Decode HTML entities (like  , <, etc.) - optional but good
                processedContent = System.Net.WebUtility.HtmlDecode(processedContent);

                // 4. Trim leading/trailing whitespace from the whole text and from each line
                var lines = processedContent.Split(new[] { '\n' }, StringSplitOptions.None)
                                            .Select(line => line.Trim());
                return string.Join("\n", lines).Trim();
            }
        }
    }

    public class HtmlSaver : ISaver
    {
        public void Save(Stream stream, string content)
        {
            var paragraphs = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(line => $"<p>{System.Net.WebUtility.HtmlEncode(line.Trim())}</p>");
            string htmlContent = $"<html>\n<head>\n<title>Edited Document</title>\n</head>\n<body>\n{string.Join("\n", paragraphs)}\n</body>\n</html>";

            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
            {
                writer.Write(htmlContent);
                writer.Flush(); // Ensure content is written to stream
            }
        }
    }

    // --- TXT Specific ---
    public class TxtLoader : ILoader
    {
        public string Load(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public class TxtSaver : ISaver
    {
        public void Save(Stream stream, string content)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
            {
                writer.Write(content);
                writer.Flush();
            }
        }
    }
    
    public class BinLoader : ILoader
    {
        public string Load(Stream stream)
        {
            // Read all bytes
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                byte[] bytes = ms.ToArray();
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }

    public class BinSaver : ISaver
    {
        public void Save(Stream stream, string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
            {
                writer.Write(bytes);
                writer.Flush();
            }
        }
    }
}