namespace ExampleLib
{
    public class ExampleClass2Hook
    {
        public string Name { get; private set; }

        public ExampleClass2Hook(string name)
        {
            Name = name;
        }

        public string GetHello()
        {
            return $"Hello, {Name}!";
        }

        public void UpdateName(string name)
        {
            Name = name;
        }
    }
}
