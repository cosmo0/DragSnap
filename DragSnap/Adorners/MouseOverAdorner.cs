namespace DragSnap.Adorners
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class MouseOverAdorner : Adorner
    {
        private readonly FrameworkElement frameworkElementAdorner;

        public MouseOverAdorner(UIElement adornedElement, FrameworkElement frameworkElementAdorner) : base(adornedElement)
        {
            // Assure we get mouse hits
            this.frameworkElementAdorner = frameworkElementAdorner;
            this.AddVisualChild(frameworkElementAdorner);
            this.AddLogicalChild(frameworkElementAdorner);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1; 
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.frameworkElementAdorner.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.frameworkElementAdorner;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.frameworkElementAdorner.Width = constraint.Width;
            this.frameworkElementAdorner.Height = constraint.Height;

            return constraint;
        }
    }
}