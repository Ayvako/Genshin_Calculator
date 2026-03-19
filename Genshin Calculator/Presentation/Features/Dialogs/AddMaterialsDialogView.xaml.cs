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

    private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void NumericOnly_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            string text = (string)e.DataObject.GetData(typeof(string));
            Regex regex = new Regex("[^0-9]+");

            if (regex.IsMatch(text))
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