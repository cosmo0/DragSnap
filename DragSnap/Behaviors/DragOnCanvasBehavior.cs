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
        /// Registers the selection adorner dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedAdornerTemplateProperty =
            DependencyProperty.Register(
                "SelectedAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragOnCanvasBehavior),
                new PropertyMetadata(new PropertyChangedCallback((d, o) =>
                {
                    if (null != ((DragOnCanvasBehavior)d).selectedAdornerControl)
                    {
                        ((DragOnCanvasBehavior)d).selectedAdornerControl.ContentTemplate = (DataTemplate)o.NewValue;
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
        /// Whether the mouse over adorner is initialized
        /// </summary>
        private bool mouseOverAdornerInitialized = false;

        /// <summary>
        /// Stores a value indicating whether the adorner is shown
        /// </summary>
        private bool mouseOverAdornerShown = false;

        /// <summary>
        /// Stores the mouse starting position
        /// </summary>
        private Point mouseStartPosition = new Point(0, 0);

        /// <summary>
        /// The selected adorner template
        /// </summary>
        private TemplateAdorner selectedAdorner;

        /// <summary>
        /// The selected adorner control
        /// </summary>
        private ContentControl selectedAdornerControl;

        /// <summary>
        /// Whether the selection adorner is initialized
        /// </summary>
        private bool selectedAdornerInitialized;
  
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
        /// Gets or sets the selected adorner template
        /// </summary>
        public DataTemplate SelectedAdornerTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(SelectedAdornerTemplateProperty);
            }

            set
            {
                this.SetValue(SelectedAdornerTemplateProperty, value);
            }
        }

        protected override void OnAttached()
        {
            this.InitializeAdorner(this.MouseOverAdornerTemplate, ref this.mouseOverAdornerControl, ref this.mouseOverAdorner, ref this.mouseOverAdornerInitialized);
            this.InitializeAdorner(this.SelectedAdornerTemplate, ref this.selectedAdornerControl, ref this.selectedAdorner, ref this.selectedAdornerInitialized);

            UIElement element = (UIElement)this.AssociatedObject;

            // mouse-over events
            element.MouseEnter += this.element_MouseEnter;
            element.MouseLeave += this.element_MouseLeave;

            // mouse click events
            element.PreviewMouseLeftButtonDown += this.element_PreviewMouseLeftButtonDown;
            element.PreviewMouseLeftButtonUp += this.element_PreviewMouseLeftButtonUp;
            element.PreviewMouseMove += this.element_PreviewMouseMove;
        }

        private void element_MouseEnter(object sender, MouseEventArgs e)
        {
            this.OnShowMouseOverAdorner();
        }

        private void element_MouseLeave(object sender, MouseEventArgs e)
        {
            this.OnHideMouseOverAdorner();
        }

        private void element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.OnSelectItem();
            this.OnStartDrag();
        }

        private void element_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.OnStopDrag();
        }

        private void element_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            this.OnDragging();
        }

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
        /// Initializes an adorner
        /// </summary>
        /// <param name="template">The adorner data template</param>
        /// <param name="control">The adorner control</param>
        /// <param name="adorner">The adorner</param>
        /// <param name="initialized">Whether the adorner has been initialized</param>
        private void InitializeAdorner(DataTemplate template, ref ContentControl control, ref TemplateAdorner adorner, ref bool initialized)
        {
            if (initialized)
            {
                return;
            }

            if (template != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.AssociatedObject as UIElement);

                if (adornerLayer == null)
                {
                    throw new NullReferenceException(string.Format("No adorner found in attached object: {0}", this.AssociatedObject));
                }

                // Create adorner
                control = new ContentControl();

                // Add to adorner
                adornerLayer.Add(adorner = new TemplateAdorner(this.AssociatedObject as UIElement, control));

                // set realted bindings
                control.Content = template.LoadContent();

                // Bind internal dependency to external 
                Binding bindingMargin = new Binding("AdornerMargin");
                bindingMargin.Source = this;
                BindingOperations.SetBinding(adorner, ContentControl.MarginProperty, bindingMargin);
            }

            // Set Data context here because default template assigment is  not setting the context
            var dataContext = (this.AssociatedObject as FrameworkElement).DataContext;
            if (control.DataContext == null)
            {
                control.DataContext = dataContext;
            }

            control.Visibility = Visibility.Collapsed;

            initialized = true;
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
        /// Selects the current item
        /// </summary>
        private void OnSelectItem()
        {
            this.selectedAdornerControl.Visibility = Visibility.Visible;

            this.DraggableItem.Select();
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