using TextCopy;

namespace Tendeos.Utils.Input
{
    public static class Clipboard
    {
        public static string Text
        {
            get => ClipboardService.GetText() ?? "";
            set => ClipboardService.SetText(value);
        }
    }
}
