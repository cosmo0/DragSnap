namespace DragSnap.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Media;
    using Caliburn.Micro;
    using DragSnap.Events;
    using DragSnap.Models;
    using PropertyChanged;

    /// <summary>
    /// Represents an item to drag and drop
    /// </summary>
    [ImplementPropertyChanged]
    public class ItemViewModel : IDragDropHandler
    {
        /// <summary>
        /// The events aggregator
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The random generator
        /// </summary>
        private readonly Random r = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemViewModel"/> class
        /// </summary>
        /// <param name="events">The event aggregator to use</param>
        public ItemViewModel(IEventAggregator events)
        {
            this.events = events;
            this.events.Subscribe(this);

            this.BackgroundColor = this.RandomColor();

            this.Width = 100;
            this.Height = 100;
            this.X = 0;
            this.Y = 0;

            this.ID = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the item background color
        /// </summary>
        public Brush BackgroundColor { get; set; }

        /// <summary>
        /// Gets the bottom side coordinates
        /// </summary>
        public double Bottom
        {
            get
            {
                return this.Y + (double)this.Height;
            }
        }

        /// <summary>
        /// Gets or sets the item height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets the right side coordinates
        /// </summary>
        public double Right
        {
            get
            {
                return this.X + (double)this.Width;
            }
        }

        /// <summary>
        /// Gets or sets the item width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the item X position
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the item Y position
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the item ID (to differenciate between them in the main viewmodel)
        /// </summary>
        internal Guid ID { get; set; }

        /// <summary>
        /// Runs when the item is dropped
        /// </summary>
        public void Dropped()
        {
            this.events.PublishOnUIThread(new ItemDroppedEvent(this.X, this.Y, this.Width, this.Height, this.ID));
        }

        /// <summary>
        /// Runs when the item is moved
        /// </summary>
        public void Moved(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// http://stackoverflow.com/a/11282427/6776
        /// </summary>
        private SolidColorBrush RandomColor()
        {
            var properties = typeof(Brushes).GetProperties();
            var count = properties.Count();
            var color = properties.Select(x => new { Property = x, Index = this.r.Next(count) })
                                  .OrderBy(x => x.Index)
                                  .First();

            return (SolidColorBrush)color.Property.GetValue(color, null);
        }
    }
}