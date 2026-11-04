using At.luki0606.ClassPulse.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Diagnostics.CodeAnalysis;

namespace At.luki0606.ClassPulse
{
    /// <summary>
    /// Given a view model, returns the corresponding view if possible.
    /// </summary>
    [RequiresUnreferencedCode(
        "Default implementation of ViewLocator involves reflection which may be trimmed away.",
        Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
    [ExcludeFromCodeCoverage]
    public class ViewLocator : IDataTemplate
    {
        /// <summary>
        /// Creates the view for a view model by replacing <c>ViewModel</c> with <c>View</c> in its full type name.
        /// </summary>
        /// <param name="param">The view model to create a view for.</param>
        /// <returns>The new view, a text block naming the missing type if no view exists, or <see langword="null"/> if <paramref name="param"/> is <see langword="null"/>.</returns>
        public Control? Build(object? param)
        {
            if (param is null)
            {
                return null;
            }

            string name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            Type? type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        /// <summary>
        /// Determines whether this locator handles the given object.
        /// </summary>
        /// <param name="data">The object to test.</param>
        /// <returns><see langword="true"/> if <paramref name="data"/> derives from <see cref="ViewModelBase"/>.</returns>
        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
