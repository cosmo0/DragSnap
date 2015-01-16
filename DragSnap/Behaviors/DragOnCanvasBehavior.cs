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
        public static readonly DependencyProperty DraggableItemProperty =
            DependencyProperty.RegisterAttached(
                "DraggableItem",
                typeof(IDraggable),
                typeof(DragOnCanvasBehavior),
                new PropertyMetadata(new PropertyChangedCallback((d, e) =>
                {
                    ((DragOnCanvasBehavior)d).draggable = (IDraggable)e.NewValue;
                })));

        private IDraggable draggable;

        private Point elementPosition = new Point(0, 0);

        private Point mouseStartPosition = new Point(0, 0);

        public DragOnCanvasBehavior()
        {
            this.MouseLeftButtonDownCommand = new RelayCommand((o) =>
            {
                ((UIElement)this.AssociatedObject).MouseLeftButtonDown += this.ElementOnMouseLeftButtonDown;
            });

            this.MouseLeftButtonUpCommand = new RelayCommand((o) =>
            {
                ((UIElement)this.AssociatedObject).MouseLeftButtonUp += this.ElementOnMouseLeftButtonUp;
            });

            this.MouseMoveCommand = new RelayCommand((o) =>
            {
                ((UIElement)this.AssociatedObject).MouseMove += this.ElementOnMouseMove;
            });
        }

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

        public ICommand MouseLeftButtonDownCommand { get; private set; }

        public ICommand MouseLeftButtonUpCommand { get; private set; }

        public ICommand MouseMoveCommand { get; private set; }

        private void ElementOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.draggable == null)
            {
                return;
            }

            // save the mouse position on button down
            // we only want a diff of the mouse position so we don't care much about which element we use as reference
            this.mouseStartPosition = this.GetMousePositionFromMainWindow(e);
            ((UIElement)sender).CaptureMouse();
        }

        private void ElementOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement element = (UIElement)sender;
            element.ReleaseMouseCapture();

            // Send a message to the viewmodel that the mouse has been released
            if (this.draggable != null)
            {
                this.DraggableItem.Dropped();
            }
        }

        private void ElementOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!((UIElement)sender).IsMouseCaptured || this.draggable == null)
            {
                return;
            }

            // calculate element movement
            Point mouseNewPos = this.GetMousePositionFromMainWindow(e);
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
        /// Gets the mouse coordinates relative to the main application window
        /// </summary>
        /// <param name="e">The mouse event args</param>
        /// <returns>The mouse coordinates</returns>
        private Point GetMousePositionFromMainWindow(MouseEventArgs e)
        {
            Window mainWindow = Application.Current.MainWindow;
            return e.GetPosition(mainWindow);
        }
    }
}