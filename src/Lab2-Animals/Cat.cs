namespace Lab2
{
    public class Cat : Animal, IPet
    {
        public Cat(string name) : base("Кошка")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя питомца не может быть пустым.", nameof(name));

            Name = name;
        }

        public string Name { get; }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} говорит: Мяу-мяу!");
        }

        public override void Move()
        {
            Console.WriteLine($"{Name} крадётся на мягких лапках.");
        }

        public void Play()
        {
            Console.WriteLine($"{Name} играет с клубком ниток.");
        }
    }
}