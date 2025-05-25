using System.Text;

namespace FileProcessorApp
{
    public static class FileTypeDetector
    {
        // A simple heuristic-based detector
        public static FileType DetectType(Stream stream)
        {
            if (!stream.CanRead) throw new ArgumentException("Stream is not readable.");
            if (!stream.CanSeek) throw new ArgumentException("Stream is not seekable. For detection, a seekable stream or a copy is needed.");

            long originalPosition = stream.Position;
            stream.Position = 0; // Go to the beginning

            FileType detectedType = FileType.Unknown;
            byte[] buffer = new byte[1024]; // Read up to 1KB for detection
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string contentStart = Encoding.ASCII.GetString(buffer, 0, Math.Min(bytesRead, 200)).ToLowerInvariant(); // Look at first 200 chars

                // HTML check: very basic, looks for <html> or <!doctype html
                if (contentStart.Contains("<html") || contentStart.Contains("<!doctype html"))
                {
                    detectedType = FileType.HTML;
                }
                else
                {
                    int nonPrintableChars = 0;
                    int printableChars = 0;
                    for (int i = 0; i < Math.Min(bytesRead, 512); i++) // Check first 512 bytes
                    {
                        byte b = buffer[i];
                        if (b == 0) // Null bytes are a strong indicator of binary
                        {
                            nonPrintableChars += 5; // Heavily weight null bytes
                        }
                        else if (b < 32 && b != '\n' && b != '\r' && b != '\t') // Control characters
                        {
                            nonPrintableChars++;
                        }
                        else if (b < 127 || b >=160) // Printable ASCII or extended (UTF-8 can have higher bytes)
                        {
                            printableChars++;
                        }
                    }

                    // Heuristic: if non-printable characters are more than 10% of printable, or many nulls, consider it BIN
                    if (nonPrintableChars > 0 && (printableChars == 0 || (double)nonPrintableChars / printableChars > 0.1))
                    {
                        detectedType = FileType.BIN;
                    }
                    else
                    {
                        detectedType = FileType.TXT; // Default to TXT if not obviously HTML or BIN
                    }
                }
            }

            stream.Position = originalPosition; // Reset stream position
            return detectedType;
        }

        // Helper to get factory based on detected type
        public static IFileFormatFactory GetFactory(FileType type)
        {
            switch (type)
            {
                case FileType.HTML:
                    return new HtmlFileFactory();
                case FileType.TXT:
                    return new TxtFileFactory();
                case FileType.BIN:
                    return new BinFileFactory();
                default:
                    Console.WriteLine($"Warning: Unknown file type or unhandled type '{type}'. Defaulting to TXT factory.");
                    return new TxtFileFactory();
            }
        }
    }
}