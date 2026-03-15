using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace Genshin_Calculator.Presentation.Features.Characters;

public partial class CharacterEditView : Window
{
    public CharacterEditView()
    {
        this.InitializeComponent();
        this.LocationChanged += (sender, e) => { ResetPopupLocation(); };
    }

    private static void ResetPopupLocation()
    {
        foreach (var popup in GetOpenPopups())
        {
            var offset = popup.HorizontalOffset;
            popup.HorizontalOffset = offset + 1;
            popup.HorizontalOffset = offset;
        }
    }

    private static IEnumerable<Popup> GetOpenPopups()
    {
        return PresentationSource.CurrentSources.OfType<HwndSource>()
            .Select(h => h.RootVisual)
            .OfType<FrameworkElement>()
            .Select(f => f.Parent)
            .OfType<Popup>()
            .Where(p => p.IsOpen);
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }
}