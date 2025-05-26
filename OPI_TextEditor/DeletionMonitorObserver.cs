using System.Windows.Forms; // Required for MessageBox

namespace OPI_TextEditor
{
    public class DeletionMonitorObserver : ITextChangeObserver
    {
        public void Update(TextChangeData changeData)
        {
            if (!string.IsNullOrEmpty(changeData.DeletedText))
            {
                string[] words = changeData.DeletedText.Split(new[] { ' ', '\t', '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (words.Length > 1)
                {
                    MessageBox.Show($"More than one word deleted. Number of words: {words.Length}",
                                    "Deletion Detected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}