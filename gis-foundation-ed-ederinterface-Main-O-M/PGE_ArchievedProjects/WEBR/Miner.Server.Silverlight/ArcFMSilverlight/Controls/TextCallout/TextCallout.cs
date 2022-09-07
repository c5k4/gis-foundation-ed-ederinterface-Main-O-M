using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArcFMSilverlight
{
    [TemplatePart(Name = ElementCallout, Type = typeof(Path))]
    public class TextCallout : ContentControl
    {
        private const string ElementCallout = "PART_CalloutPath";

        private Path _callout;
        private PathGeometry _calloutGeometry;
        private Size _currentSize;
        private bool _updated;

        public TextCallout()
        {
            DefaultStyleKey = typeof(TextCallout);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _callout = GetTemplateChild(ElementCallout) as Path;
            base.InvalidateArrange();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            if (!_updated && (_currentSize != size))
            {
                _calloutGeometry = CreateCalloutBox(size);
                _currentSize = size;

                LayoutUpdated += new EventHandler(TextCallout_LayoutUpdated);
            }
            _updated = false;

            return size;
        }

        void TextCallout_LayoutUpdated(object sender, EventArgs e)
        {
            _updated = true;
            LayoutUpdated -= TextCallout_LayoutUpdated;
            _callout.Data = _calloutGeometry;
        }

        private PathGeometry CreateCalloutBox(Size size)
        {
            const double cornerRadius = 5;
            double width = Math.Max(size.Width, 20);
            double height = Math.Max(size.Height, 20);

            PathGeometry geometry = new PathGeometry();

            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(0, 0);

            LineSegment segment = new LineSegment();
            segment.Point = new Point(20, 10);
            figure.Segments.Add(segment);

            segment = new LineSegment();
            segment.Point = new Point(width, 10);
            figure.Segments.Add(segment);

            ArcSegment arc = new ArcSegment();
            arc.Size = new Size(cornerRadius, cornerRadius);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.Point = new Point(width + cornerRadius, 15);
            figure.Segments.Add(arc);

            segment = new LineSegment();
            segment.Point = new Point(width + cornerRadius, height);
            figure.Segments.Add(segment);

            arc = new ArcSegment();
            arc.Size = new Size(cornerRadius, cornerRadius);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.Point = new Point(width, height + cornerRadius);
            figure.Segments.Add(arc);

            segment = new LineSegment();
            segment.Point = new Point(10, height + cornerRadius);
            figure.Segments.Add(segment);

            arc = new ArcSegment();
            arc.Size = new Size(cornerRadius, cornerRadius);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.Point = new Point(5, height);
            figure.Segments.Add(arc);

            segment = new LineSegment();
            segment.Point = new Point(5, 10);
            figure.Segments.Add(segment);

            segment = new LineSegment();
            segment.Point = new Point(0, 0);
            figure.Segments.Add(segment);

            geometry.Figures.Add(figure);

            return geometry;
        }
    }
}
