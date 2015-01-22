namespace DragSnap.Events
{
    using System;

    /// <summary>
    /// Represents an item selected event
    /// </summary>
    public class ItemSelectedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSelectedEvent"/> class
        /// </summary>
        /// <param name="id"></param>
        public ItemSelectedEvent(Guid id)
        {
            this.ID = id;
        }

        /// <summary>
        /// Gets or sets the item ID
        /// </summary>
        public Guid ID { get; set; }
    }
}