using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ErrorHandler
{
    public static void Print(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nError: {ex.Message}");
        Console.ResetColor();
    }
}
