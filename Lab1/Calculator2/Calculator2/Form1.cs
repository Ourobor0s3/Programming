namespace Calculator2;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private TextBox _input;

    private void button1_Click(object sender, EventArgs e)
    {
        _input.Text += "1";
    }

    private void button8_Click(object sender, EventArgs e)
    {
        _input.Text += "2";
    }

    private void button7_Click(object sender, EventArgs e)
    {
        _input.Text += "3";
    }

    private void button6_Click(object sender, EventArgs e)
    {
        _input.Text += "4";
    }

    private void button5_Click(object sender, EventArgs e)
    {
        _input.Text += "5";
    }

    private void button4_Click(object sender, EventArgs e)
    {
        _input.Text += "6";
    }

    private void button3_Click(object sender, EventArgs e)
    {
        _input.Text += "7";
    }

    private void button2_Click(object sender, EventArgs e)
    {
        _input.Text += "8";
    }

    private void button10_Click(object sender, EventArgs e)
    {
        _input.Text += "9";
    }

    private void button9_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_input.Text)) _input.Text += "0";
    }

    private void button11_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_input.Text)) _input.Text += " ^ ";
    }

    private void button16_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_input.Text)) _input.Text += " + ";
    }

    private void button15_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_input.Text)) _input.Text += " - ";
    }

    private void button14_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_input.Text)) _input.Text += " / ";
    }

    private void button13_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_input.Text)) _input.Text += " * ";
    }

    private void button12_Click(object sender, EventArgs e)
    {
        try
        {
            var parts = _input.Text!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                _input.Text = "Error: write the full format expression: 4 * 5";
            }
            else
            {
                _input.Text = parts[1] switch
                {
                    "+" => $"= {double.Parse(parts[0]) + double.Parse(parts[2])}",
                    "-" => $"= {double.Parse(parts[0]) - double.Parse(parts[2])}",
                    "*" => $"= {double.Parse(parts[0]) * double.Parse(parts[2])}",
                    "/" => $"= {double.Parse(parts[0]) / double.Parse(parts[2])}",
                    "^" => $"= {Math.Pow(double.Parse(parts[0]), double.Parse(parts[2]))}",
                    _ => $"Error: not find {parts[1]}!",
                };
            }
        }
        catch (Exception err)
        {
            _input.Text = err.Message;
        }
    }

    private void button17_Click(object sender, EventArgs e)
    {
        _input.Text = "";
    }
}