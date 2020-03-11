namespace CustomMessageAnnouncer.Models
{
    public class TextCommand
    {
        public TextCommand() { }
        public TextCommand(string name, string help, string color, string message)
        {
            Name = name;
            Help = help;
            Color = color;
            Message = message;
        }
        public string Name { get; set; }
        public string Help { get; set; }
        public string Color { get; set; }
        public string Message { get; set; }
    }
}