namespace Lab6
{
    public class Guest
    {
        public string Name { get; set; }
        internal Glass HeldGlass { get; set; }
        public Guest(string name)
        {
            Name = name;
        }
    }
}