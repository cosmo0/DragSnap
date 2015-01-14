namespace DragSnap.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Media;
    using Caliburn.Micro;
    using PropertyChanged;

    /// <summary>
    /// Represents an item to drag and drop
    /// </summary>
    [ImplementPropertyChanged]
    public class ItemViewModel
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
            this.X = 10;
            this.Y = 10;
        }

        /// <summary>
        /// Gets or sets the item background color
        /// </summary>
        public Brush BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the item height
        /// </summary>
        public int Height { get; set; }

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