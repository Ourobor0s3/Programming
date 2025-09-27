namespace Lab2
{
    public static class AnimalProgram
    {
        public static void Main()
        {
            try
            {
                var shelter = new Shelter();

                var dog = new Dog("Шарик");
                var cat = new Cat("Мурзик");

                shelter.AddAnimal(dog);
                shelter.AddAnimal(cat);

                shelter.ShowAll();

                dog.Play();
                cat.Play();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}