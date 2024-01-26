/*
 * Project:	    ASSIGNMENT 01 – – PARALLEL PROGRAMMING
 * Author:	    Hoang Phuc Tran
 * Student ID:  8789102
 * Date:		2024-01-24
 * Description:  A C# console application that demonstrates how well sequential and parallel
 * algorithms guess a password with numbers. It compares the speed and guess count of the two approaches 
 * using.NET's Task Parallel Library, highlighting the benefits of multi-core processing.
 */
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string password = "";

        // If arguments are passed through the command line
        if (args.Length > 0)
        {
            password = args[0];
        }
        else
        {
            Console.WriteLine("Please enter a numeric password between 6 and 18 digits:");
            password = Console.ReadLine();
        }

        // Validate the password
        if (IsValidPassword(password))
        {
        }
        else
        {
            Console.WriteLine("Invalid password. Please ensure it is 6-18 digits long and contains only numbers.\n");
            Environment.Exit(1); // Exit the program if the password is invalid
        }

        Console.WriteLine("Enter time limit in seconds for guessing the password:");
        if (!int.TryParse(Console.ReadLine(), out int timeLimitSeconds))
        {
            Console.WriteLine("Invalid input for time limit.");
            Environment.Exit(1);
        }

        // Run Sequential Password Guessing
        CancellationTokenSource sequentialx = new CancellationTokenSource();
        sequentialx.CancelAfter(TimeSpan.FromSeconds(timeLimitSeconds));

        try
        {
            Console.WriteLine("\nStarting Sequential Guess...");
            GuessPasswordSequential(password, sequentialx.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Time limit exceeded in Sequential Guess.");
        }

        // Run Parallel Password Guessing
        CancellationTokenSource parallelx = new CancellationTokenSource();
        parallelx.CancelAfter(TimeSpan.FromSeconds(timeLimitSeconds));

        try
        {
            Console.WriteLine("\nStarting Parallel Guess...");
            await Task.Run(() => GuessPasswordUsingParallel(password, parallelx.Token), parallelx.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Time limit exceeded in Parallel Guess.");
        }
    }

    /// <summary>
    /// Validates if the given password is a valid numeric password.
    /// </summary>
    /// <param name="password">Password string to validate.</param>
    /// <returns>Boolean indicating whether the password is valid (true) or not (false).</returns>
    static bool IsValidPassword(string password)
    {
        return password.Length >= 6 && password.Length <= 18 && IsAllDigits(password);
    }

    /// <summary>
    /// Checks if a given string consists only of digit characters.
    /// </summary>
    /// <param name="str">String to check.</param>
    /// <returns>Boolean indicating whether the string contains only digits (true) or not (false).</returns>
    static bool IsAllDigits(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }
        return true;
    }


    /// <summary>
    /// Attempts to guess a numeric password using a sequential approach.
    /// </summary>
    /// <param name="password">The password to guess.</param>
    /// <param name="token">Cancellation token for stopping the process.</param>
    static void GuessPasswordSequential(string password, CancellationToken token)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        long numberOfGuesses = 0;
        bool found = false;

        // Define the maximum length for the password guess
        int maxLength = password.Length;

        // Sequential approach
        for (int length = 1; length <= maxLength; length++)
        {
            var currentGuess = new char[length];
            if (SequentialFunc(currentGuess, 0, password, ref numberOfGuesses, token))
            {
                found = true;
                break;
            }
        }

        stopwatch.Stop();

        if (found)
        {
            Console.WriteLine($"-> Password found. Time (Squential) : {stopwatch.Elapsed.TotalSeconds} seconds. Guesses (Squential) : {numberOfGuesses}");
        }
        else
        {
            Console.WriteLine($"-> Password not found. Time (Squential) : {stopwatch.Elapsed.TotalSeconds} seconds. Guesses (Squential) : {numberOfGuesses}");
        }

        if (token.IsCancellationRequested)
        {
            Console.WriteLine("***Time limit exceeded (Squential).***");
        }
    }


    /// <summary>
    /// Recursively generates and tests each combination for the password.
    /// </summary>
    /// <param name="totalGuess">Current guess array.</param>
    /// <param name="position">Current position in the guess array.</param>
    /// <param name="password">The password to guess.</param>
    /// <param name="myGuesses">Counter for the number of guesses made.</param>
    /// <param name="token">Cancellation token to handle timeout.</param>
    /// <returns>Boolean indicating whether the password is found.</returns>
    static bool SequentialFunc(char[] totalGuess, int position, string password, ref long myGuesses, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return false;
        }

        if (position == totalGuess.Length)
        {
            // Increment the guess count only when a full guess is made
            if (totalGuess.Length == password.Length)
            {
                myGuesses++;
            }

            return new string(totalGuess) == password;
        }

        for (int i = 0; i < 10; i++)
        {
            totalGuess[position] = (char)('0' + i);
            if (SequentialFunc(totalGuess, position + 1, password, ref myGuesses, token))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to guess a numeric password using parallel processing.
    /// </summary>
    /// <param name="password">The password to guess.</param>
    /// <param name="token">Cancellation token to handle timeout.</param>
    static void GuessPasswordUsingParallel(string password, CancellationToken token)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        long maxNumber = long.Parse(password);
        int maxLength = password.Length;
        long totalGuesses = 0;
        bool found = false;

        Parallel.For(0L, (long)Math.Pow(10, maxLength), () => 0L, (i, state, localSum) =>
        {
            if (found || token.IsCancellationRequested)
            {
                state.Stop();
                return localSum;
            }

            if (i == maxNumber)
            {
                found = true;
                state.Stop();
            }
            return localSum + 1;
        },
        localSum => Interlocked.Add(ref totalGuesses, localSum));

        stopwatch.Stop();

        string result = found ? "found" : "not found";
        Console.WriteLine($"-> Password {result}. Time (Parallel) : {stopwatch.Elapsed.TotalSeconds} seconds. Guesses (Parallel) : {totalGuesses}");
    }

}

