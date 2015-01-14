namespace DragSnap.Models
{
    /// <summary>
    /// Defines an interface for the drop handler
    /// </summary>
    public interface IDropHandler
    {
        /// <summary>
        /// Runs when the item is dropped
        /// </summary>
        void Dropped();
    }
}