namespace Animals
{
    public class Shelter
    {
        private readonly List<Animal> _animals = [];

        public void AddAnimal(Animal animal)
        {
            ArgumentNullException.ThrowIfNull(animal);
            _animals.Add(animal);
        }

        public void ShowAll()
        {
            foreach (var animal in _animals)
            {
                animal.MakeSound();
                animal.Move();
            }
        }
    }
}