using System.IO; // Required for Path
using System.Linq; // Required for LINQ methods like Any
using System.Windows.Forms; // Required for MessageBox

namespace OPI_TextEditor
{
    public class AutoSaveParagraphObserver : ITextChangeObserver
    {
        private static readonly string[] ParagraphSeparators = { "\r\n\r\n", "\n\n" };

        public void Update(TextChangeData changeData)
        {
            if (string.IsNullOrEmpty(changeData.InsertedText)) // Only consider additions
            {
                return;
            }

            int oldParagraphCount = CountParagraphs(changeData.OldText);
            int newParagraphCount = CountParagraphs(changeData.NewText);

            if (newParagraphCount > oldParagraphCount)
            {
                Form1 editor = changeData.EditorForm;
                if (editor != null)
                {
                    editor.AutoSaveFile("New paragraph added");
                }
            }
        }

        private int CountParagraphs(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;

            string normalizedText = text.Replace("\r\n", "\n");
            string[] paragraphs = normalizedText.Split(new[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            return paragraphs.Length > 0 ? paragraphs.Length : 1;
        }
    }
}