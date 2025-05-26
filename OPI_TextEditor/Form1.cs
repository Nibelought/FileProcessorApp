using System;
using System.Collections.Generic; // For List
using System.IO;
using System.Linq; // For Where, Select, Count on collections
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic; // For Interaction.InputBox

// Make sure you have using directives for your new observer classes if they are in different namespaces,
// though here they are in the same OPI_TextEditor namespace.

namespace OPI_TextEditor
{
    public partial class Form1 : Form
    {
        private string currentFilePath = null;
        private System.Windows.Forms.Timer dropdownTimer;
        private string previousText = string.Empty;
        private bool _isLogging = false; // Prevents re-entrancy for TextChanged and related processing

        private readonly List<ITextChangeObserver> _observers = new List<ITextChangeObserver>();

        public Form1()
        {
            InitializeComponent();

            SetupDropDownHover(FileMenu);
            SetupDropDownHover(EditMenu);
            SetupDropDownHover(AboutMenu);

            dropdownTimer = new System.Windows.Forms.Timer();
            dropdownTimer.Interval = 500;
            dropdownTimer.Tick += DropdownTimer_Tick;

            this.richTextBox1.TextChanged += new System.EventHandler(this.RichTextBox1_TextChanged);
            this.previousText = this.richTextBox1.Text; // Initialize previousText

            // Attach observers
            AttachObserver(new DeletionMonitorObserver());
            AttachObserver(new AutoSaveParagraphObserver());
        }

        // Observer pattern methods
        public void AttachObserver(ITextChangeObserver observer)
        {
            _observers.Add(observer);
        }

        public void DetachObserver(ITextChangeObserver observer)
        {
            _observers.Remove(observer);
        }

        private void NotifyObservers(TextChangeData changeData)
        {
            foreach (var observer in _observers)
            {
                observer.Update(changeData);
            }
        }

        private void ProcessTextChange(string oldText, string newText)
        {
            // Calculate diffs (insertedText, deletedText)
            int prefixLength = 0;
            while (prefixLength < oldText.Length && prefixLength < newText.Length && oldText[prefixLength] == newText[prefixLength])
            {
                prefixLength++;
            }

            int suffixLength = 0;
            while (suffixLength < oldText.Length - prefixLength &&
                   suffixLength < newText.Length - prefixLength &&
                   oldText[oldText.Length - 1 - suffixLength] == newText[newText.Length - 1 - suffixLength])
            {
                suffixLength++;
            }

            string currentDeletedText = "";
            if (oldText.Length > prefixLength + suffixLength)
            {
                currentDeletedText = oldText.Substring(prefixLength, oldText.Length - prefixLength - suffixLength);
            }

            string currentInsertedText = "";
            if (newText.Length > prefixLength + suffixLength)
            {
                currentInsertedText = newText.Substring(prefixLength, newText.Length - prefixLength - suffixLength);
            }

            TextChangeData changeData = new TextChangeData(oldText, newText, currentInsertedText, currentDeletedText, this);
            NotifyObservers(changeData);
        }


        private void SetupDropDownHover(ToolStripDropDownButton menu)
        {
            bool isDropDownOpenLocal = false;

            menu.DropDownOpened += (s, e) => isDropDownOpenLocal = true;
            menu.DropDownClosed += (s, e) =>
            {
                isDropDownOpenLocal = false;
            };

            menu.MouseEnter += (s, e) =>
            {
                dropdownTimer.Stop();
                if (!isDropDownOpenLocal)
                {
                    menu.ShowDropDown();
                }
            };

            menu.MouseLeave += (s, e) =>
            {
                Point clientCursorPos = menu.Owner.PointToClient(Cursor.Position);
                if (!menu.Bounds.Contains(clientCursorPos) &&
                    (!menu.HasDropDownItems || !menu.DropDown.Visible || !menu.DropDown.Bounds.Contains(clientCursorPos)))
                {
                    dropdownTimer.Start();
                }
            };

            if (menu.HasDropDownItems)
            {
                menu.DropDown.MouseEnter += (s, e) =>
                {
                    dropdownTimer.Stop();
                };

                menu.DropDown.MouseLeave += (s, e) =>
                {
                    Point clientCursorPos = menu.Owner.PointToClient(Cursor.Position);
                    if (!menu.Bounds.Contains(clientCursorPos) &&
                        (!menu.HasDropDownItems || !menu.DropDown.Visible || !menu.DropDown.Bounds.Contains(clientCursorPos)))
                    {
                        dropdownTimer.Start();
                    }
                };
            }
        }


        private void DropdownTimer_Tick(object sender, EventArgs e)
        {
            dropdownTimer.Stop();
            bool cursorOverAnyMenu = false;

            foreach (var menu in new[] { FileMenu, EditMenu, AboutMenu })
            {
                Point clientCursorPos = menu.Owner.PointToClient(Cursor.Position);
                if (menu.Bounds.Contains(clientCursorPos) ||
                    (menu.HasDropDownItems && menu.DropDown.Visible && menu.DropDown.Bounds.Contains(clientCursorPos)))
                {
                    cursorOverAnyMenu = true;
                    break;
                }
            }

            if (!cursorOverAnyMenu)
            {
                foreach (var menu in new[] { FileMenu, EditMenu, AboutMenu })
                {
                    if (menu.HasDropDownItems && menu.DropDown.Visible)
                    {
                        menu.HideDropDown();
                    }
                }
            }
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (_isLogging) return;

            _isLogging = true; // Main guard for this event handler chain
            string currentText = richTextBox1.Text;
            string textBeforeChange = this.previousText;

            try
            {
                // Original logging to file
                FindDiffAndLog(textBeforeChange, currentText);

                // Notify new observers
                ProcessTextChange(textBeforeChange, currentText);

                this.previousText = currentText; // Update baseline for the next change
            }
            finally
            {
                _isLogging = false;
            }
        }

        private void FindDiffAndLog(string oldText, string newText)
        {
            int prefixLength = 0;
            while (prefixLength < oldText.Length && prefixLength < newText.Length && oldText[prefixLength] == newText[prefixLength])
            {
                prefixLength++;
            }

            int suffixLength = 0;
            while (suffixLength < oldText.Length - prefixLength &&
                   suffixLength < newText.Length - prefixLength &&
                   oldText[oldText.Length - 1 - suffixLength] == newText[newText.Length - 1 - suffixLength])
            {
                suffixLength++;
            }

            int changeStartLine = 1;
            int changeStartCol = 1;
            for (int i = 0; i < prefixLength; i++)
            {
                if (oldText[i] == '\n')
                {
                    changeStartLine++;
                    changeStartCol = 1;
                }
                else
                {
                    changeStartCol++;
                }
            }

            string deletedText = "";
            if (oldText.Length > prefixLength + suffixLength)
            {
                deletedText = oldText.Substring(prefixLength, oldText.Length - prefixLength - suffixLength);
                if (!string.IsNullOrEmpty(deletedText))
                {
                    EventLogger.Instance.Log(changeStartLine, changeStartCol, EventType.Deletion, deletedText);
                }
            }

            string insertedText = "";
            if (newText.Length > prefixLength + suffixLength)
            {
                insertedText = newText.Substring(prefixLength, newText.Length - prefixLength - suffixLength);
                if (!string.IsNullOrEmpty(insertedText))
                {
                    EventLogger.Instance.Log(changeStartLine, changeStartCol, EventType.Addition, insertedText);
                }
            }
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fullPathname = openFileDialog.FileName;
                    FileInfo src = new FileInfo(fullPathname);
                    currentFilePath = fullPathname;
                    FilenameDisplay.Text = src.Name;

                    string textBeforeLoad = this.previousText; // Content before clearing/loading
                    string fileContent = File.ReadAllText(fullPathname, Encoding.UTF8);

                    _isLogging = true;
                    richTextBox1.Text = fileContent;
                    _isLogging = false;

                    // Manually process changes as TextChanged was suppressed
                    ProcessTextChange(textBeforeLoad, fileContent);
                    FindDiffAndLog(textBeforeLoad, fileContent);
                    this.previousText = fileContent; // Set new baseline
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- SAVING LOGIC ---
        private bool PerformSave()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                return PerformSaveAs();
            }
            else
            {
                try
                {
                    File.WriteAllText(currentFilePath, richTextBox1.Text, Encoding.UTF8);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private bool PerformSaveAs()
        {
            saveFileDialog1.FileName = string.IsNullOrEmpty(currentFilePath) ? "Untitled.txt" : Path.GetFileName(currentFilePath);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filename = saveFileDialog1.FileName;
                    File.WriteAllText(filename, richTextBox1.Text, Encoding.UTF8);
                    currentFilePath = filename;
                    FilenameDisplay.Text = Path.GetFileName(filename);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file as: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false; // User cancelled
        }

        public void AutoSaveFile(string reason)
        {
            bool savedSuccessfully;
            string filePathForMessage = currentFilePath; // Capture before potential change by PerformSaveAs

            if (string.IsNullOrEmpty(currentFilePath))
            {
                savedSuccessfully = PerformSaveAs();
                if (savedSuccessfully) filePathForMessage = currentFilePath; // Update if SaveAs gave a new path
            }
            else
            {
                savedSuccessfully = PerformSave();
            }

            if (savedSuccessfully)
            {
                MessageBox.Show($"File auto-saved: {Path.GetFileName(filePathForMessage)}.\nReason: {reason}.\nData in the file has been updated.",
                                "Auto-Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PerformSave())
            {
                MessageBox.Show("File saved!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PerformSaveAs())
            {
                MessageBox.Show("File " + Path.GetFileName(currentFilePath) + " was saved successfully.", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cleanWhitespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBox1.Text)) return;

            string textBeforeChange = richTextBox1.Text;
            string text = textBeforeChange;

            text = text.Replace("\t", " ");
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var cleanedLines = lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => string.Join(" ", line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
            string result = string.Join(Environment.NewLine, cleanedLines);

            if (result != textBeforeChange)
            {
                _isLogging = true;
                richTextBox1.Text = result;
                _isLogging = false;

                ProcessTextChange(textBeforeChange, result);
                FindDiffAndLog(textBeforeChange, result);
                this.previousText = result;
            }
        }

        private void changeCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                string textBeforeFullChange = richTextBox1.Text;
                string selectedText = richTextBox1.SelectedText;

                DialogResult dialogResult = MessageBox.Show("Change to UPPERCASE?\n(Yes = Uppercase, No = Lowercase)",
                    "Change Case", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                string modifiedSelectedText = dialogResult == DialogResult.Yes ? selectedText.ToUpper() : selectedText.ToLower();

                if (selectedText != modifiedSelectedText)
                {
                    int selectionStart = richTextBox1.SelectionStart;
                    int selectionLength = richTextBox1.SelectionLength;

                    _isLogging = true;
                    richTextBox1.Text = richTextBox1.Text.Remove(selectionStart, selectionLength).Insert(selectionStart, modifiedSelectedText);
                    richTextBox1.Select(selectionStart, modifiedSelectedText.Length);
                    _isLogging = false;

                    string textAfterChange = richTextBox1.Text;
                    ProcessTextChange(textBeforeFullChange, textAfterChange);
                    FindDiffAndLog(textBeforeFullChange, textAfterChange);
                    this.previousText = textAfterChange;
                }
            }
            else
            {
                MessageBox.Show("Please select some text to change its case.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void loadNewsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox( // Ensure Microsoft.VisualBasic is referenced
                "Enter the number of news items to load (1-50):",
                "Load News",
                "10", // Default value
                -1, -1); // Default position

            if (!int.TryParse(input, out int newsCount) || newsCount < 1 || newsCount > 50)
            {
                MessageBox.Show("Please enter a valid number between 1 and 50.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Fetch news content (LoadNewsAsync needs to return the content or be refactored)
                string newsContent = await FetchNewsContentAsync(newsCount); // New helper method

                string textBeforeLoad = this.previousText;

                _isLogging = true;
                richTextBox1.Text = newsContent;
                _isLogging = false;

                ProcessTextChange(textBeforeLoad, newsContent);
                FindDiffAndLog(textBeforeLoad, newsContent);
                this.previousText = newsContent;

                currentFilePath = null; // News is not a saved file initially
                FilenameDisplay.Text = "News from ZNU.edu.ua";

                MessageBox.Show($"Successfully loaded {newsCount} news items.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading news: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Refactored from original LoadNewsAsync to separate fetching from UI update
        private async Task<string> FetchNewsContentAsync(int count)
        {
            using (var client = new HttpClient())
            {
                string url = "https://www.znu.edu.ua/cms/index.php?action=news/view&start=0&site_id=27&lang=ukr";
                string html = await client.GetStringAsync(url);

                string titlePattern = @"<h4><a[^>]*>(.+?)</a></h4>";
                string annotationPattern = @"<div class=""text""><p>([\s\S]*?)</p></div>";

                var titleMatches = Regex.Matches(html, titlePattern);
                var annotationMatches = Regex.Matches(html, annotationPattern);

                StringBuilder newsTextBuilder = new StringBuilder();
                int itemsToProcess = Math.Min(count, Math.Min(titleMatches.Count, annotationMatches.Count));

                for (int i = 0; i < itemsToProcess; i++)
                {
                    string title = Regex.Replace(titleMatches[i].Groups[1].Value, "<.*?>", "").Trim();
                    string annotation = Regex.Replace(annotationMatches[i].Groups[1].Value, "<.*?>", "").Trim();

                    newsTextBuilder.AppendLine($"Title: {title}");
                    newsTextBuilder.AppendLine($"Annotation: {annotation}");
                    newsTextBuilder.AppendLine(new string('-', 50));
                    newsTextBuilder.AppendLine();
                }
                return newsTextBuilder.ToString();
            }
        }


        // --- Other existing methods (About, Statistics, Regex Test) ---
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string statsMessage =
                "Author: Maksym Dziuman\n" +
                "App for laboratory work with Observer Pattern."; // Updated
            MessageBox.Show(statsMessage, "About Text Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void statisticsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                MessageBox.Show("No text to analyze!", "Statistics", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string text = richTextBox1.Text;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            double fileSizeKB = textBytes.Length / 1024.0;
            int totalChars = text.Length;
            string[] paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            int paragraphCount = paragraphs.Length;
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            int emptyLines = lines.Count(line => string.IsNullOrWhiteSpace(line));
            double authorPages = totalChars / 1800.0;
            string vowelsLatin = "aeiouAEIOU";
            string vowelsCyrillic = "àå¸èîóûýþÿÀÅ¨ÈÎÓÛÝÞß"; // Cyrillic vowels
            string allVowels = vowelsLatin + vowelsCyrillic;
            int vowelCount = text.Count(c => allVowels.Contains(c));
            int consonantCount = text.Count(c => char.IsLetter(c) && !allVowels.Contains(c));
            int digitCount = text.Count(char.IsDigit);
            int specialCharCount = text.Count(c => !char.IsLetterOrDigit(c) && !char.IsPunctuation(c) && !char.IsWhiteSpace(c));
            int punctuationCount = text.Count(char.IsPunctuation);
            int cyrillicCount = text.Count(c => (c >= 'À' && c <= 'ÿ') || c == '¨' || c == '¸');
            int latinCount = text.Count(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));

            string statsMessage =
                $"File Statistics:\n" +
                $"Size: {fileSizeKB:F2} KB\n" +
                $"Total Characters: {totalChars}\n" +
                $"Paragraphs (split by double newlines): {paragraphCount}\n" + // Clarified
                $"Empty Lines: {emptyLines}\n" +
                $"Author Pages (1800 chars): {authorPages:F2}\n" +
                $"Vowels: {vowelCount}\n" +
                $"Consonants: {consonantCount}\n" +
                $"Digits: {digitCount}\n" +
                $"Special Characters: {specialCharCount}\n" +
                $"Punctuation Marks: {punctuationCount}\n" +
                $"Cyrillic Letters: {cyrillicCount}\n" +
                $"Latin Letters: {latinCount}";
            MessageBox.Show(statsMessage, "Text Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void regexTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pattern = @"(['""])(\d+\.\d+)\1";
            MatchCollection matches = Regex.Matches(richTextBox1.Text, pattern);

            if (matches.Count > 0)
            {
                string result = "Real constants found in the text:\n";
                foreach (Match match in matches)
                {
                    result += $"{match.Value}\n";
                }
                MessageBox.Show(result, "Real Constants Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No real constants found in the text.", "Real Constants Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // The EventLogger and LogEntry classes remain unchanged as per the provided code.
    }

    // EventLogger and LogEntry classes (from provided code, assumed to be in the same file or accessible)
    public enum EventType
    {
        Addition,
        Deletion
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public EventType Type { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            string formattedText = Text.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t"); // Handle \r too
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} - Line: {LineNumber}, Col: {ColumnNumber} - Type: {Type} - Text: \"{formattedText}\"";
        }
    }

    public sealed class EventLogger
    {
        private static readonly Lazy<EventLogger> lazyInstance =
            new Lazy<EventLogger>(() => new EventLogger());

        public static EventLogger Instance => lazyInstance.Value;

        private readonly List<LogEntry> _logEntries;
        private const string LogFileName = "editor_events.log";

        private EventLogger()
        {
            _logEntries = new List<LogEntry>();
        }

        public void Log(int lineNumber, int columnNumber, EventType eventType, string text)
        {
            if (string.IsNullOrEmpty(text) && eventType == EventType.Addition)
            {
                // Allow logging of empty additions if that's ever intended, but often it's not.
                // However, for deletions, empty text might mean something was deleted to become empty.
                // The original code had this, keeping it.
                // return; 
            }

            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                LineNumber = lineNumber,
                ColumnNumber = columnNumber,
                Type = eventType,
                Text = text
            };
            _logEntries.Add(entry);
            System.Diagnostics.Debug.WriteLine(entry.ToString());

            try
            {
                // Log if text is present OR it's a deletion (even if deleted text is empty, the act of deletion is logged)
                if (!string.IsNullOrEmpty(entry.Text) || entry.Type == EventType.Deletion)
                {
                    File.AppendAllText(LogFileName, entry.ToString() + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        public IReadOnlyList<LogEntry> GetLogEntries()
        {
            return _logEntries.AsReadOnly();
        }
    }
}