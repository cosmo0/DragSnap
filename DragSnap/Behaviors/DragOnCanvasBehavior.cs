namespace DragSnap.Behaviors
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using DragSnap.Adorners;
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

        public static readonly DependencyProperty MouseOverAdornerTemplateProperty =
            DependencyProperty.Register(
                "AdornerTemplate",
                typeof(DataTemplate),
                typeof(DragOnCanvasBehavior),
                new PropertyMetadata(new PropertyChangedCallback((d, o) =>
                {
                    if (null != ((DragOnCanvasBehavior)d).mouseOverAdornerControl)
                    {
                        ((DragOnCanvasBehavior)d).mouseOverAdornerControl.ContentTemplate = (DataTemplate)o.NewValue;
                    }
                })));

        /// <summary>
        /// Stores the draggable element
        /// </summary>
        private IDraggable draggable;

        /// <summary>
        /// Stores the element position
        /// </summary>
        private Point elementPosition = new Point(0, 0);

        private MouseOverAdorner mouseOverAdorner;

        private ContentControl mouseOverAdornerControl;

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

            this.ShowMouseOverAdorner = new RelayCommand((o) =>
            {
                this.OnShowMouseOverAdorner();
            });

            this.HideMouseOverAdorner = new RelayCommand((o) =>
            {
                this.OnHideMouseOverAdorner();
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

        public ICommand HideMouseOverAdorner { get; private set; }

        public DataTemplate MouseOverAdornerTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(MouseOverAdornerTemplateProperty);
            }
            set
            {
                this.SetValue(MouseOverAdornerTemplateProperty, value);
            }
        }

        public ICommand ShowMouseOverAdorner { get; private set; }

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

        private void OnHideMouseOverAdorner()
        {
            this.mouseOverAdornerControl.Visibility = Visibility.Collapsed;
        }

        private void OnShowMouseOverAdorner()
        {
            if (this.MouseOverAdornerTemplate != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.AssociatedObject as UIElement);

                if (null == adornerLayer)
                {
                    throw new NullReferenceException(string.Format("No adorner found in attached object: {0}", this.AssociatedObject));
                }

                // Create adorner
                this.mouseOverAdornerControl = new ContentControl();

                // Add to adorner
                adornerLayer.Add(this.mouseOverAdorner = new MouseOverAdorner(this.AssociatedObject as UIElement, this.mouseOverAdornerControl));

                // set realted bindings
                this.mouseOverAdornerControl.Content = this.MouseOverAdornerTemplate.LoadContent();

                // Bind internal dependency to external 
                Binding bindingMargin = new Binding("AdornerMargin");
                bindingMargin.Source = this;
                BindingOperations.SetBinding(this.mouseOverAdorner, ContentControl.MarginProperty, bindingMargin);
            }

            // Set Data context here because default template assigment is  not setting the context
            var dtContext = (this.AssociatedObject as FrameworkElement).DataContext;
            if (null == this.mouseOverAdornerControl.DataContext)
            {
                this.mouseOverAdornerControl.DataContext = dtContext;
            }

            this.mouseOverAdornerControl.Visibility = Visibility.Visible;
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