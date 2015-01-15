namespace DragSnap.Models
{
    /// <summary>
    /// Defines an interface for the drag and drop handler
    /// </summary>
    public interface IDragDropHandler
    {
        /// <summary>
        /// Runs when the item is dropped
        /// </summary>
        void Dropped();

        /// <summary>
        /// Runs when the item is moved
        /// </summary>
        void Moved(double x, double y);
    }
}