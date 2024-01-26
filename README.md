
### Overview
The emergence of multi-core computers has created a demand for the development of parallel programs. Microsoft .NET includes the Task Parallel Library (TPL), which aids in the programming of multi-processor/core computers. This assignment involves developing an application to demonstrate performance improvements using TPL.

### Objectives
- To explore programming language support for parallel processing, including .NET support for Windows.

### Requirements
- **Program Specifications:**
  1. Develop a Console application in C# utilizing TPL.
  2. Accept user input either interactively or through command line arguments. Ensure all input is validated.
  3. Require the user to input a numeric password for the program to guess. The password should only contain digits 0-9 and be 6-18 digits in length.
  4. Implement a user-defined time limit for finding the password. The password search must terminate if this limit is exceeded, for both sequential and parallel algorithms.

- **For both single-threaded and parallel execution:**
  a. Locate the user-entered password using a sequential search approach.
  b. Record the time taken to guess the password using the Stopwatch in the System.Diagnostics namespace.
  c. Count and report the number of guesses required to find the password.
  d. Indicate if the time limit was exceeded.
  e. Adhere to good OOP practices and SET programming standards.

