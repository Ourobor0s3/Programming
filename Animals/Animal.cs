namespace Animals
{
    public abstract class Animal
    {
        protected Animal(string species)
        {
            if (string.IsNullOrWhiteSpace(species))
                throw new ArgumentException("Вид животного не может быть пустым.", nameof(species));

            Species = species;
        }

        private string Species { get; }

        public abstract void MakeSound();

        public virtual void Move()
        {
            Console.WriteLine($"{Species} движется...");
        }
    }
}