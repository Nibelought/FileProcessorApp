namespace OPI_TextEditor
{
    public class TextChangeData
    {
        public string OldText { get; }
        public string NewText { get; }
        public string InsertedText { get; }
        public string DeletedText { get; }
        public Form1 EditorForm { get; } // To allow observers to interact with the form

        public TextChangeData(string oldText, string newText, string insertedText, string deletedText, Form1 editorForm)
        {
            OldText = oldText;
            NewText = newText;
            InsertedText = insertedText;
            DeletedText = deletedText;
            EditorForm = editorForm;
        }
    }
}