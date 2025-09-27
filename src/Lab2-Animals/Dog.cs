namespace Lab2
{
    public class Dog : Animal, IPet
    {
        public Dog(string name) : base("Собака")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя питомца не может быть пустым.", nameof(name));

            Name = name;
        }

        public string Name { get; }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} говорит: Гав-гав!");
        }

        public override void Move()
        {
            Console.WriteLine($"{Name} бегает на четырех лапах.");
        }

        public void Play()
        {
            Console.WriteLine($"{Name} играет с мячом.");
        }
    }
}