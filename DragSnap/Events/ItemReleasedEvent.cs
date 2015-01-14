namespace DragSnap.Events
{
    /// <summary>
    /// Represents an item released (dropped) event
    /// </summary>
    public class ItemReleasedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemReleasedEvent"/> class
        /// </summary>
        /// <param name="x">The item's X</param>
        /// <param name="y">The item's Y</param>
        /// <param name="width">The item's width</param>
        /// <param name="height">The item's height</param>
        public ItemReleasedEvent(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate
        /// </summary>
        public double Y { get; set; }
    }
}