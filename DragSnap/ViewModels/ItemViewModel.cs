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
        private readonly IEventAggregator events;

        Random r = new Random();

        public ItemViewModel(IEventAggregator events)
        {
            this.events = events;
            this.events.Subscribe(this);

            this.BackgroundColor = this.RandomColor();

            this.Width = 100;
            this.Height = 100;
            this.X = 10;
            this.Y = 10;

            this.Content = "test";
        }

        public Brush BackgroundColor { get; set; }

        public string Content { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public double X { get; set; }

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