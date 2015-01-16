namespace DragSnap.Models
{
    /// <summary>
    /// Defines an interface for the drag and drop handler
    /// </summary>
    public interface IDraggable
    {
        /// <summary>
        /// Gets or sets the X coordinate
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate
        /// </summary>
        double Y { get; set; }

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