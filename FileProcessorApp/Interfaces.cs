using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FileProcessorApp
{
    // ----- Abstract Products: Loaders -----
    public interface ILoader
    {
        string Load(Stream stream); // Returns content ready for display
    }

    // ----- Abstract Products: Savers -----
    public interface ISaver
    {
        void Save(Stream stream, string content); // Content is the edited text from screen
    }

    // ----- Abstract Factory -----
    public interface IFileFormatFactory
    {
        ILoader CreateLoader();
        ISaver CreateSaver();
    }

    // ----- File Type Enum -----
    public enum FileType
    {
        HTML,
        TXT,
        BIN,
        Unknown
    }
}