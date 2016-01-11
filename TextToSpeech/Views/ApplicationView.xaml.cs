namespace TextToSpeech.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : Window
    {
        public ApplicationView()
        {
            this.InitializeComponent();
        }

        private double GetTextBoxFontSize(int lineCount, double height)
        {
            double fontSize = 30;

            return lineCount < Math.Ceiling(height / (fontSize + fontSize * 0.1)) ? fontSize : fontSize / 2;
        }

        private void InputTextBox_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                textBox.FontSize = this.GetTextBoxFontSize(textBox.LineCount, e.NewSize.Height);
            }
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                textBox.FontSize = this.GetTextBoxFontSize(textBox.LineCount, textBox.ActualHeight);
            }
        }
    }
}