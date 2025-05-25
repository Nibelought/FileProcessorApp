namespace FileProcessorApp
{
    public class HtmlFileFactory : IFileFormatFactory
    {
        public ILoader CreateLoader() => new HtmlLoader();
        public ISaver CreateSaver() => new HtmlSaver();
    }

    public class TxtFileFactory : IFileFormatFactory
    {
        public ILoader CreateLoader() => new TxtLoader();
        public ISaver CreateSaver() => new TxtSaver();
    }

    public class BinFileFactory : IFileFormatFactory
    {
        public ILoader CreateLoader() => new BinLoader();
        public ISaver CreateSaver() => new BinSaver();
    }
}