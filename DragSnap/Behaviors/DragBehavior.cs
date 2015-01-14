namespace DragSnap.Behaviors
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using DragSnap.Models;

    /// <summary>
    /// Draggable item behavior
    /// <see href="https://github.com/RobertHedgate/Behavior-lab-xaml/blob/master/src/BehaviorsLabSolution/BehaviorsLab/Behaviors/DragBehavior.cs" />
    /// </summary>
    public class DragBehavior
    {
        /// <summary>
        /// Registers a DropHandler dependency property
        /// </summary>
        public static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.RegisterAttached(
                "DropHandler",
                typeof(IDropHandler),
                typeof(DragBehavior),
                new PropertyMetadata(OnDropHandlerChanged));

        /// <summary>
        /// Registers a Drag dependency property
        /// </summary>
        public static readonly DependencyProperty IsDragProperty =
            DependencyProperty.RegisterAttached(
                "Drag",
                typeof(bool),
                typeof(DragBehavior),
                new PropertyMetadata(false, OnDragChanged));

        /// <summary>
        /// The instance of the behavior
        /// </summary>
        private static DragBehavior _instance = new DragBehavior();

        /// <summary>
        /// The applied transformation
        /// </summary>
        private readonly TranslateTransform Transform = new TranslateTransform();

        /// <summary>
        /// The element starting position
        /// </summary>
        private Point _elementStartPosition2 = new Point(0, 0);

        /// <summary>
        /// The mouse starting position
        /// </summary>
        private Point _mouseStartPosition2 = new Point(0, 0);

        /// <summary>
        /// The drop handler
        /// </summary>
        private IDropHandler dropHandler;

        /// <summary>
        /// Gets or sets the behavior instance
        /// </summary>
        public static DragBehavior Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Gets the IsDrag property
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <returns>Whether the item is draggable</returns>
        public static bool GetDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragProperty);
        }

        /// <summary>
        /// Gets the DropHandler property
        /// </summary>
        /// <param name="target">The drop target</param>
        /// <returns>The drop target</returns>
        public static IDropHandler GetDropHandler(UIElement target)
        {
            return (IDropHandler)target.GetValue(DropHandlerProperty);
        }

        /// <summary>
        /// Sets the IsDrag property
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <param name="value">The value of the property</param>
        public static void SetDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragProperty, value);
        }

        /// <summary>
        /// Sets the drop handler property
        /// </summary>
        /// <param name="target">The drop target</param>
        /// <param name="value">The value of the property</param>
        public static void SetDropHandler(UIElement target, IDropHandler value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        /// <summary>
        /// Attaches or detaches the instance events handlers
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event values</param>
        private static void OnDragChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // ignoring error checking
            var element = (UIElement)sender;
            var isDrag = (bool)(e.NewValue);

            ((UIElement)sender).RenderTransform = Instance.Transform;

            if (isDrag)
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
        /// Sets the OnDrop handler on the instance
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event values</param>
        private static void OnDropHandlerChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var handler = (IDropHandler)(e.NewValue);

            Instance.dropHandler = handler;
        }

        /// <summary>
        /// Handles the mouse button down event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="mouseButtonEventArgs">The event values</param>
        private void ElementOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var parent = Application.Current.MainWindow;
            this._mouseStartPosition2 = mouseButtonEventArgs.GetPosition(parent);
            ((UIElement)sender).CaptureMouse();
        }

        /// <summary>
        /// Handles the mouse button up event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="mouseButtonEventArgs">The event values</param>
        private void ElementOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            ((UIElement)sender).ReleaseMouseCapture();
            this._elementStartPosition2.X = this.Transform.X;
            this._elementStartPosition2.Y = this.Transform.Y;

            if (this.dropHandler != null)
            {
                this.dropHandler.Dropped();
            }
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="mouseEventArgs">The event values</param>
        private void ElementOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var parent = Application.Current.MainWindow;
            var mousePos = mouseEventArgs.GetPosition(parent);
            var diff = (mousePos - this._mouseStartPosition2);
            if (!((UIElement)sender).IsMouseCaptured)
            {
                return;
            }

            this.Transform.X = this._elementStartPosition2.X + diff.X;
            this.Transform.Y = this._elementStartPosition2.Y + diff.Y;
        }
    }
}