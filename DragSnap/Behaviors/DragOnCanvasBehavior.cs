namespace DragSnap.Behaviors
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using DragSnap.Commands;
    using DragSnap.Models;

    /// <summary>
    /// Draggable item behavior
    /// </summary>
    public class DragOnCanvasBehavior : Behavior<DependencyObject>
    {
        /// <summary>
        /// Registers the DraggableItem dependency property
        /// </summary>
        public static readonly DependencyProperty DraggableItemProperty =
            DependencyProperty.RegisterAttached(
                "DraggableItem",
                typeof(IDraggable),
                typeof(DragOnCanvasBehavior),
                new PropertyMetadata(new PropertyChangedCallback((d, e) =>
                {
                    ((DragOnCanvasBehavior)d).draggable = (IDraggable)e.NewValue;
                })));

        /// <summary>
        /// Stores the draggable element
        /// </summary>
        private IDraggable draggable;

        /// <summary>
        /// Stores the element position
        /// </summary>
        private Point elementPosition = new Point(0, 0);

        /// <summary>
        /// Stores the mouse starting position
        /// </summary>
        private Point mouseStartPosition = new Point(0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="DragOnCanvasBehavior"/> class
        /// </summary>
        public DragOnCanvasBehavior()
        {
            this.StartDrag = new RelayCommand((o) =>
            {
                this.OnStartDrag();
            });

            this.StopDrag = new RelayCommand((o) =>
            {
                this.OnStopDrag();
            });

            this.Dragging = new RelayCommand((o) =>
            {
                this.OnDragging();
            });
        }

        /// <summary>
        /// Gets or sets the draggable item
        /// </summary>
        public IDraggable DraggableItem
        {
            get
            {
                return (IDraggable)this.GetValue(DraggableItemProperty);
            }
            set
            {
                this.SetValue(DraggableItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the "mouse move" command
        /// </summary>
        public ICommand Dragging { get; private set; }

        /// <summary>
        /// Gets or sets the "left button down" command
        /// </summary>
        public ICommand StartDrag { get; private set; }

        /// <summary>
        /// Gets or sets the "left button up" command
        /// </summary>
        public ICommand StopDrag { get; private set; }

        /// <summary>
        /// Gets the mouse coordinates relative to the main application window
        /// </summary>
        /// <param name="e">The mouse event args</param>
        /// <returns>The mouse coordinates</returns>
        private Point GetMousePositionFromMainWindow()
        {
            Window mainWindow = Application.Current.MainWindow;
            return Mouse.GetPosition(mainWindow);
        }

        /// <summary>
        /// Fires when the mouse is moved over the item
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private void OnDragging()
        {
            if (!((UIElement)this.AssociatedObject).IsMouseCaptured || this.draggable == null)
            {
                return;
            }

            // calculate element movement
            Point mouseNewPos = this.GetMousePositionFromMainWindow();
            Vector movement = (mouseNewPos - this.mouseStartPosition);

            // make sure the mouse has moved since last time
            if (movement.Length > 0)
            {
                // save current mouse position
                this.mouseStartPosition = mouseNewPos;
                // move the element
                Point elementNewPos = this.elementPosition + movement;
                this.elementPosition = elementNewPos;
                // notify the viewmodel that the element has been moved
                this.DraggableItem.Moved(elementNewPos.X, elementNewPos.Y);
            }
        }

        /// <summary>
        /// Fires when the left mouse button is pressed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private void OnStartDrag()
        {
            if (this.draggable == null)
            {
                return;
            }

            // save the mouse position on button down
            // we only want a diff of the mouse position so we don't care much about which element we use as reference
            this.mouseStartPosition = this.GetMousePositionFromMainWindow();
            ((UIElement)this.AssociatedObject).CaptureMouse();
        }

        /// <summary>
        /// Fires when the left mouse button is released
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private void OnStopDrag()
        {
            UIElement element = (UIElement)this.AssociatedObject;
            element.ReleaseMouseCapture();

            // Send a message to the viewmodel that the mouse has been released
            if (this.draggable != null)
            {
                this.DraggableItem.Dropped();
            }
        }
    }
}