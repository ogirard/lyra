using System;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Lyra.Framework
{
    public static class VisualTreeExtensions
    {
        public static T FindChild<T>(this DependencyObject parent, Func<T, bool> predicate = null)
            where T : FrameworkElement
        {
            if (parent == null)
            {
                return null;
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                {
                    if (predicate?.Invoke(typedChild) ?? true)
                    {
                        return typedChild;
                    }
                }

                var result = child.FindChild(predicate);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static StringBuilder LogVisualTree(this FrameworkElement root, StringBuilder logger = null, string indent = "")
        {
            logger ??= new StringBuilder();

            logger.AppendLine($"{indent}{root.Name}");
            var childrenCount = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < childrenCount; i++)
            {
                if (VisualTreeHelper.GetChild(root, i) is FrameworkElement child)
                {
                    child.LogVisualTree(logger, indent + "  ");
                }
            }

            return logger;
        }
    }
}