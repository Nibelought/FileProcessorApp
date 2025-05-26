using System.Text;

namespace FileProcessorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // For proper console display

            // Create some sample files
            CreateSampleFiles();

            // --- Test Scenarios ---
            ProcessFile("sample.html");
            ProcessFile("sample.txt");
            ProcessFile("sample.bin"); // This will contain UTF-8 text
            ProcessFile("sample_actual.bin"); // This will contain some binary data

            Console.WriteLine("\n--- All files processed. Check the 'edited_...' files. ---");
        }

        static void ProcessFile(string filePath)
        {
            Console.WriteLine($"\n--- Processing: {filePath} ---");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File '{filePath}' not found.");
                return;
            }

            IFileFormatFactory factory = null;
            string originalContentForDisplay = null;
            FileType detectedType;

            // 1. Server assigns file (as a stream), library tries to determine type
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                detectedType = FileTypeDetector.DetectType(fs);
                Console.WriteLine($"Detected type: {detectedType}");

                factory = FileTypeDetector.GetFactory(detectedType);
                ILoader loader = factory.CreateLoader();

                // Reset stream position for loader AFTER detection
                fs.Position = 0;
                originalContentForDisplay = loader.Load(fs);
            }

            Console.WriteLine("--- Content Loaded for Display ---");
            Console.WriteLine(originalContentForDisplay);
            Console.WriteLine("--- End of Loaded Content ---");

            // 2. Simulate editing
            string editedContent = originalContentForDisplay + "\n\n[--- Appended by Editor ---]";
            Console.WriteLine("\n--- Content After Simulated Edit ---");
            Console.WriteLine(editedContent);
            Console.WriteLine("--- End of Edited Content ---");

            // 3. Save the edited file
            if (factory != null)
            {
                ISaver saver = factory.CreateSaver();
                string editedFilePath = Path.Combine(
                    Path.GetDirectoryName(filePath),
                    "edited_" + Path.GetFileName(filePath)
                );

                try
                {
                    using (FileStream fsWrite = new FileStream(editedFilePath, FileMode.Create, FileAccess.Write))
                    {
                        saver.Save(fsWrite, editedContent);
                    }
                    Console.WriteLine($"Successfully saved edited content to: {editedFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving file {editedFilePath}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Could not determine factory, skipping save.");
            }
        }

        static void CreateSampleFiles()
        {
            // Sample HTML
            File.WriteAllText("sample.html",
                "<html><head><title>Test HTML</title></head><body>" +
                "<h1>Main Heading</h1>" +
                "<P>This is the <b>first</b> paragraph.</p>" +
                "Some text without paragraph tag.<br/>Another line after break." +
                "<p class='myclass'>Second paragraph with class.</p>" +
                "<div><p>Nested paragraph.</p></div>" +
                "<This is an encoded tag & an ampersand>" +
                "</body></html>");

            // Sample TXT
            File.WriteAllText("sample.txt",
                "This is a simple text file.\n" +
                "It has multiple lines.\n" +
                "And some special characters like: áéíóú ñ €." +
                "\tTabs and   spaces too.");

            // Sample BIN (containing UTF-8 text)
            File.WriteAllBytes("sample.bin",
                Encoding.UTF8.GetBytes("This is text content stored in a binary file.\n" +
                "It should be treated as text.\n" +
                "Line three for the binary-stored text."));

            // Sample actual BIN (with some non-text data)
            byte[] actualBinData = new byte[] {
                0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, // "Hello "
                0x00, 0x01, 0x02, 0x03, 0x04,       // Some non-printable bytes
                0x57, 0x6F, 0x72, 0x6C, 0x64,       // "World"
                0xFF, 0xFE, 0xFD                  // More non-printable
            };
            File.WriteAllBytes("sample_actual.bin", actualBinData);

            Console.WriteLine("Sample files created.");
        }
    }
}