using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Genshin_Calculator.Presentation.Helpers;

public static partial class TextBoxExtensions
{
    public static readonly DependencyProperty NumericOnlyProperty =
        DependencyProperty.RegisterAttached(
            "NumericOnly",
            typeof(bool),
            typeof(TextBoxExtensions),
            new PropertyMetadata(false, OnNumericOnlyChanged));

    public static bool GetNumericOnly(DependencyObject obj) => (bool)obj.GetValue(NumericOnlyProperty);

    public static void SetNumericOnly(DependencyObject obj, bool value) => obj.SetValue(NumericOnlyProperty, value);

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumericOnlyRegex();

    private static void OnNumericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
        {
            return;
        }

        bool isEnabled = (bool)e.NewValue;

        if (isEnabled)
        {
            textBox.PreviewTextInput += BlockNonNumericText;
            DataObject.AddPastingHandler(textBox, OnPaste);
        }
        else
        {
            textBox.PreviewTextInput -= BlockNonNumericText;
            DataObject.RemovePastingHandler(textBox, OnPaste);
        }
    }

    private static void BlockNonNumericText(object sender, TextCompositionEventArgs e)
    {
        e.Handled = NumericOnlyRegex().IsMatch(e.Text);
    }

    private static void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            string text = (string)e.DataObject.GetData(typeof(string));
            if (NumericOnlyRegex().IsMatch(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }
}