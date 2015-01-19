namespace DragSnap.Adorners
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Provides a template-based adorner
    /// </summary>
    public class TemplateAdorner : Adorner
    {
        /// <summary>
        /// The framework element of the adorner
        /// </summary>
        private readonly FrameworkElement frameworkElementAdorner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateAdorner"/> class
        /// </summary>
        /// <param name="adornedElement">The adorned element</param>
        /// <param name="frameworkElementAdorner">The framework element of the adorner</param>
        public TemplateAdorner(UIElement adornedElement, FrameworkElement frameworkElementAdorner) : base(adornedElement)
        {
            this.frameworkElementAdorner = frameworkElementAdorner;
            this.AddVisualChild(frameworkElementAdorner);
            this.AddLogicalChild(frameworkElementAdorner);
        }

        /// <summary>
        /// Gets the number of visual children
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1; 
            }
        }

        /// <summary>
        /// Positions child elements
        /// </summary>
        /// <param name="finalSize">The final size</param>
        /// <returns>The size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.frameworkElementAdorner.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        /// <summary>
        /// Gets the visual child
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The visual child</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.frameworkElementAdorner;
        }

        /// <summary>
        /// Measures the visual element
        /// </summary>
        /// <param name="constraint">The size constraints</param>
        /// <returns>The visual element size</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.frameworkElementAdorner.Width = constraint.Width;
            this.frameworkElementAdorner.Height = constraint.Height;

            return constraint;
        }
    }
}