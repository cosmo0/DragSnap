namespace DragSnap.Events
{
    using System;

    /// <summary>
    /// Represents an item released (dropped) event
    /// </summary>
    public class ItemDroppedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDroppedEvent"/> class
        /// </summary>
        /// <param name="x">The item's X</param>
        /// <param name="y">The item's Y</param>
        /// <param name="width">The item's width</param>
        /// <param name="height">The item's height</param>
        public ItemDroppedEvent(double x, double y, double width, double height, Guid id)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;

            this.ID = id;
        }

        /// <summary>
        /// Gets the bottom side coordinates
        /// </summary>
        public double Bottom
        {
            get
            {
                return this.Y + this.Height;
            }
        }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the item's ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets the right side coordinates
        /// </summary>
        public double Right
        {
            get
            {
                return this.X + this.Width;
            }
        }

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