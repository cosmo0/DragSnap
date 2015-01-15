namespace DragSnap.Behaviors
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using DragSnap.Models;

    /// <summary>
    /// Draggable item behavior
    /// Modified from <see href="https://github.com/RobertHedgate/Behavior-lab-xaml/" />
    /// </summary>
    public class DragOnCanvasBehavior
    {
        /// <summary>
        /// Registers a DropHandler dependency property
        /// </summary>
        public static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.RegisterAttached(
                "DropHandler",
                typeof(IDragDropHandler),
                typeof(DragOnCanvasBehavior),
                new PropertyMetadata(OnDropHandlerChanged));

        /// <summary>
        /// The instance of the behavior
        /// </summary>
        private static DragOnCanvasBehavior instance = new DragOnCanvasBehavior();

        /// <summary>
        /// The element position (for movement calculations)
        /// </summary>
        private Point elementPosition = new Point(0, 0);

        /// <summary>
        /// The mouse starting position
        /// </summary>
        private Point mouseStartPosition = new Point(0, 0);

        /// <summary>
        /// Gets or sets the behavior instance
        /// </summary>
        public static DragOnCanvasBehavior Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// The drop handler
        /// </summary>
        private IDragDropHandler DropHandler { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a drop handler
        /// </summary>
        private bool HasDropHandler
        {
            get
            {
                return this.DropHandler != null;
            }
        }

        /// <summary>
        /// Gets the DropHandler property
        /// </summary>
        /// <param name="target">The drop target</param>
        /// <returns>The drop target</returns>
        public static IDragDropHandler GetDropHandler(UIElement target)
        {
            return (IDragDropHandler)target.GetValue(DropHandlerProperty);
        }

        /// <summary>
        /// Sets the drop handler property
        /// </summary>
        /// <param name="target">The drop target</param>
        /// <param name="value">The value of the property</param>
        public static void SetDropHandler(UIElement target, IDragDropHandler value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        /// <summary>
        /// Sets the OnDrop handler on the instance
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event values</param>
        private static void OnDropHandlerChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)sender;
            IDragDropHandler handler = (IDragDropHandler)(e.NewValue);

            Instance = new DragOnCanvasBehavior();
            Instance.DropHandler = handler;

            if (Instance.HasDropHandler)
            {
                element.MouseLeftButtonDown += Instance.ElementOnMouseLeftButtonDown;
                element.MouseLeftButtonUp += Instance.ElementOnMouseLeftButtonUp;
                element.MouseMove += Instance.ElementOnMouseMove;
            }
            else
            {
                element.MouseLeftButtonDown -= Instance.ElementOnMouseLeftButtonDown;
                element.MouseLeftButtonUp -= Instance.ElementOnMouseLeftButtonUp;
                element.MouseMove -= Instance.ElementOnMouseMove;
            }
        }

        /// <summary>
        /// Handles the mouse button down event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="mouseButtonEventArgs">The event values</param>
        private void ElementOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // save the mouse position on button down
            // we only want a diff of the mouse position so we don't care much about which element we use as reference
            this.mouseStartPosition = this.GetMousePositionFromMainWindow(mouseButtonEventArgs);
            ((UIElement)sender).CaptureMouse();
        }

        /// <summary>
        /// Handles the mouse button up event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="mouseButtonEventArgs">The event values</param>
        private void ElementOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            UIElement element = (UIElement)sender;
            element.ReleaseMouseCapture();

            // Send a message to the viewmodel that the mouse has been released
            if (this.HasDropHandler)
            {
                this.DropHandler.Dropped();
            }
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="mouseEventArgs">The event values</param>
        private void ElementOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!((UIElement)sender).IsMouseCaptured || !this.HasDropHandler)
            {
                return;
            }

            // calculate element movement
            Point mouseNewPos = this.GetMousePositionFromMainWindow(mouseEventArgs);
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
                this.DropHandler.Moved(elementNewPos.X, elementNewPos.Y);
            }
        }

        /// <summary>
        /// Gets the mouse coordinates relative to the main application window
        /// </summary>
        /// <param name="mouseEventArgs">The mouse event args</param>
        /// <returns>The mouse coordinates</returns>
        private Point GetMousePositionFromMainWindow(MouseEventArgs mouseEventArgs)
        {
            Window mainWindow = Application.Current.MainWindow;
            return mouseEventArgs.GetPosition(mainWindow);
        }
    }
}