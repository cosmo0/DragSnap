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

        /// <summary>
        /// Registers the MouseOverAdornerTemplate dependency property
        /// </summary>
        public static readonly DependencyProperty MouseOverAdornerTemplateProperty =
            DependencyProperty.Register(
                "MouseOverAdornerTemplate",
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

        /// <summary>
        /// Stores the mouse over adorner template
        /// </summary>
        private TemplateAdorner mouseOverAdorner;

        /// <summary>
        /// Stores the mouse over adorner control (visual representation of the template)
        /// </summary>
        private ContentControl mouseOverAdornerControl;

        /// <summary>
        /// Stores a value indicating whether the adorner is shown
        /// </summary>
        private bool mouseOverAdornerShown = false;

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
        /// Gets the "mouse move" command
        /// </summary>
        public ICommand Dragging { get; private set; }

        /// <summary>
        /// Gets the "hide mouse over adorner" command
        /// </summary>
        public ICommand HideMouseOverAdorner { get; private set; }

        /// <summary>
        /// Gets or sets the mouse over adorner template
        /// </summary>
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

        /// <summary>
        /// Gets the "show mouse over adorner" command
        /// </summary>
        public ICommand ShowMouseOverAdorner { get; private set; }

        /// <summary>
        /// Gets the "left button down" command
        /// </summary>
        public ICommand StartDrag { get; private set; }

        /// <summary>
        /// Gets the "left button up" command
        /// </summary>
        public ICommand StopDrag { get; private set; }

        /// <summary>
        /// Gets the mouse coordinates relative to the main application window
        /// </summary>
        /// <returns>The mouse coordinates</returns>
        private Point GetMousePositionFromMainWindow()
        {
            Window mainWindow = Application.Current.MainWindow;
            return Mouse.GetPosition(mainWindow);
        }

        /// <summary>
        /// Drags the item with the mouse
        /// </summary>
        private void OnDragging()
        {
            if (!((UIElement)this.AssociatedObject).IsMouseCaptured || this.draggable == null)
            {
                return;
            }

            // calculate element movement
            Point mouseNewPos = this.GetMousePositionFromMainWindow();
            Vector movement = mouseNewPos - this.mouseStartPosition;

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
        /// Hides the mouse-over adorner
        /// </summary>
        private void OnHideMouseOverAdorner()
        {
            this.mouseOverAdornerControl.Visibility = Visibility.Collapsed;
            this.mouseOverAdornerShown = false;
        }

        /// <summary>
        /// Shows the mouse-over adorner
        /// </summary>
        private void OnShowMouseOverAdorner()
        {
            if (this.mouseOverAdornerShown)
            {
                return;
            }

            if (this.MouseOverAdornerTemplate != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.AssociatedObject as UIElement);

                if (adornerLayer == null)
                {
                    throw new NullReferenceException(string.Format("No adorner found in attached object: {0}", this.AssociatedObject));
                }

                // Create adorner
                this.mouseOverAdornerControl = new ContentControl();

                // Add to adorner
                adornerLayer.Add(this.mouseOverAdorner = new TemplateAdorner(this.AssociatedObject as UIElement, this.mouseOverAdornerControl));

                // set realted bindings
                this.mouseOverAdornerControl.Content = this.MouseOverAdornerTemplate.LoadContent();

                // Bind internal dependency to external 
                Binding bindingMargin = new Binding("AdornerMargin");
                bindingMargin.Source = this;
                BindingOperations.SetBinding(this.mouseOverAdorner, ContentControl.MarginProperty, bindingMargin);
            }

            // Set Data context here because default template assigment is  not setting the context
            var dataContext = (this.AssociatedObject as FrameworkElement).DataContext;
            if (this.mouseOverAdornerControl.DataContext == null)
            {
                this.mouseOverAdornerControl.DataContext = dataContext;
            }

            this.mouseOverAdornerControl.Visibility = Visibility.Visible;
            this.mouseOverAdornerShown = true;
        }

        /// <summary>
        /// Starts the item drag
        /// </summary>
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
        /// Stops the item drag
        /// </summary>
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