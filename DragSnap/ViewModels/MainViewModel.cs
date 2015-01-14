namespace DragSnap.ViewModels
{
    using Caliburn.Micro;
    using DragSnap.Events;
    using PropertyChanged;

    /// <summary>
    /// Main view model for the application
    /// </summary>
    [ImplementPropertyChanged]
    public class MainViewModel : PropertyChangedBase, IHandle<ItemReleasedEvent>
    {
        /// <summary>
        /// Stores the events aggregator
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class
        /// </summary>
        /// <param name="events">The events</param>
        public MainViewModel(IEventAggregator events)
        {
            this.events = events;
            this.events.Subscribe(this);

            this.Items = new BindableCollection<ItemViewModel>();
        }

        /// <summary>
        /// Gets or sets a collection of items displayed in the canvas
        /// </summary>
        public BindableCollection<ItemViewModel> Items { get; set; }

        public void Handle(ItemReleasedEvent message)
        {
            // TODO: checks if the item has coordinates within another item's coordinates
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds an item to the canvas
        /// </summary>
        public void AddItem()
        {
            this.Items.Add(new ItemViewModel(this.events));

            this.NotifyOfPropertyChange(() => this.Items);
        }
    }
}