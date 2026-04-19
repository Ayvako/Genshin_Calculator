using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Genshin_Calculator.Presentation.Features.Dialogs;

public partial class AddMaterialsDialogView : Window
{
    public AddMaterialsDialogView()
    {
        this.InitializeComponent();
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumericOnlyRegex();

    private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = NumericOnlyRegex().IsMatch(e.Text);
    }

    private void NumericOnly_Pasting(object sender, DataObjectPastingEventArgs e)
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