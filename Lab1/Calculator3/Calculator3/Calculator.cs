namespace Calculator3
{
    public class Calculator
    {
        private readonly ILogger<Calculator> _logger;
        private List<string> _history;
        private const int _maxHistory = 3;

        public Calculator(ILogger<Calculator> logger)
        {
            _logger = logger;
            _history = [];
        }

        public void Run()
        {
            _logger.LogInformation("Calculator started!");
            Console.WriteLine("Calculator started!");

            while (true)
            {
                Console.Write("Enter the expression: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Task completed!");
                    _logger.LogInformation("Task completed!");
                    break;
                }

                try
                {
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (parts is ["h"])
                    {
                        for (var i = 0; i < Math.Min(_history.Count, _maxHistory); i++)
                        {
                            Console.WriteLine($"{i + 1}: {_history[i]}");
                        }
                        continue;
                    }

                    if (parts.Length != 3)
                    {
                        Console.WriteLine("Error: write the full format expression: 4 * 5");
                        _logger.LogError("Invalid input format. User wrote: '{Input}'", input);
                    }
                    else
                    {
                        if (!double.TryParse(parts[0], out var left) ||
                            !double.TryParse(parts[2], out var right))
                        {
                            Console.WriteLine("Error: operands must be numbers.");
                            _logger.LogError("Non-numeric operands: '{Operand1}' or '{Operand2}'", parts[0], parts[2]);
                            continue;
                        }

                        var op = parts[1];
                        double result;

                        switch (op)
                        {
                            case "+":
                                result = left + right;
                                break;
                            case "-":
                                result = left - right;
                                break;
                            case "*":
                                result = left * right;
                                break;
                            case "/":
                                if (right == 0)
                                {
                                    Console.WriteLine("Error: Division by zero!");
                                    _logger.LogError("Division by zero attempted.");
                                    continue;
                                }
                                result = left / right;
                                break;
                            case "^":
                                result = Math.Pow(left, right);
                                break;
                            default:
                                Console.WriteLine($"Error: operator '{op}' not supported!");
                                _logger.LogError("Unsupported operator: '{Operator}'", op);
                                continue;
                        }

                        Console.WriteLine($"= {result}");
                        _history.Add($"{input} = {result}");
                        if (_history.Count > _maxHistory)
                        {
                            _history.RemoveRange(0, _history.Count - _maxHistory);
                        }
                        _logger.LogInformation("{Expression} = {Result}", input, result);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
                    _logger.LogError(e, "Unexpected exception occurred");
                }
            }
        }
    }
}