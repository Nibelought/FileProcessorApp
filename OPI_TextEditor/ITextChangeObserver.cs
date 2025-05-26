namespace OPI_TextEditor
{
    public interface ITextChangeObserver
    {
        void Update(TextChangeData changeData);
    }
}