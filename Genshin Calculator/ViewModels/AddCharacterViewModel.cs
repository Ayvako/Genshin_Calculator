using System.Windows.Media;

namespace Genshin_Calculator.ViewModels
{
    public class AddCharacterViewModel
    {
        public AddCharacterViewModel(string name, ImageSource imagePath)
        {
            this.Name = name;
            this.ImagePath = imagePath;
        }

        public string Name { get; set; }

        public ImageSource ImagePath { get; set; }
    }
}