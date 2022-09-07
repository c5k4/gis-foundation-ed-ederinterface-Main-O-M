using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Theming.TelventGray.Control
{
    public class RotatedPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
            }

            double maxWidth = Children.MaxOrDefault(child => child.DesiredSize.Width, 0);
            double maxHeight = Children.MaxOrDefault(child => child.DesiredSize.Height, 0);

            return new Size(maxHeight, maxWidth);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                child.Arrange(
                    new Rect(
                        0, 
                        (finalSize.Height/2) + (child.DesiredSize.Width/2), 
                        child.DesiredSize.Width,
                        child.DesiredSize.Height));
            }

            return finalSize;
        }
    }

    internal static class SizeHelpers
    {
        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum Double value 
        /// if the sequence is not empty; otherwise returns the specified default value. 
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The maximum value in the sequence or default value if sequence is empty.</returns>
        public static double MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector,
                                                   double defaultValue)
        {
            return source.Count() == 0 ? defaultValue : source.Max(selector);
        }
    }
}