#define SHOWSTEPS
using System;
using System.Collections.Generic;
using System.Linq;

namespace CNG462_A2
{
    class CSP
    {
        enum Directions
        {
            right,
            down
        }

        class Variable
        {
            public int Number;
            public List<(Directions, int)> AvailableDirectionsAndTheirLength;
            public (int, int) Position;

            public Variable(int Number, (int, int) Position, List<(Directions, int)> AvailableDirectionsAndTheirLength)
            {
                this.AvailableDirectionsAndTheirLength = AvailableDirectionsAndTheirLength;
                this.Number = Number;
                this.Position = Position;
            }
        }

        private char[][] Matrix;
        private List<string> Domain;
        private List<Variable> Variables;

        public CSP(char[][] Matrix, List<string> Domain)
        {
#if SHOWSTEPS
            Console.WriteLine("CSP: WARNING! Show steps feauture is enabled");
#endif

            this.Matrix = Matrix;
            this.Domain = Domain;
            Variables = new List<Variable>();

            FindAndFillVariables();
        }

        private void FindAndFillVariables()
        {
#if SHOWSTEPS
            Console.WriteLine("Start finding variables");
#endif
            for (int i = 0; i < Matrix.Length; i++)
            {
                for (int j = 0; j < Matrix[i].Length; j++)
                {
                    if (Matrix[i][j] >= '1' && Matrix[i][j] <= '9')
                    {
                        int Number = Matrix[i][j] - '0';
#if SHOWSTEPS
                        Console.WriteLine("Found variable to create: " + Number);
                        Console.WriteLine("CREATING VARIABLE " + Number + ": Head Position: (" + i + "," + j + ")");
                        Console.WriteLine("CREATING VARIABLE " + Number + ": Changing " + Number + " to _ in matrix");
#endif
                        Matrix[i][j] = '_';
#if SHOWSTEPS
                        Console.WriteLine("CREATING VARIABLE " + Number + ": Start finding possible directions and lengths");

#endif
                        List<(Directions, int)> AvailableDirectionsAndTheirLengthTemp = new List<(Directions, int)>();

                        int RightLength = 0;
                        for (int t = j; t < Matrix[i].Length && Matrix[i][t] != '*'; t++)
                            RightLength++;

                        if (RightLength > 1)
                        {
#if SHOWSTEPS
                            Console.WriteLine("CREATING VARIABLE " + Number + ": Possible right direction solution found with length of " + RightLength);
#endif
                            AvailableDirectionsAndTheirLengthTemp.Add((Directions.right, RightLength));
                        }


                        int DownLength = 0;
                        for (int t = i; t < Matrix.Length && Matrix[t][j] != '*'; t++)
                            DownLength++;

                        if (DownLength > 1)
                        {
#if SHOWSTEPS
                            Console.WriteLine("CREATING VARIABLE " + Number + ": Possible down direction solution found with length of " + DownLength);
#endif
                            AvailableDirectionsAndTheirLengthTemp.Add((Directions.down, DownLength));
                        }

#if SHOWSTEPS
                        Console.WriteLine("CREATING VARIABLE " + Number + ": Sorting possible directions by their lengths (HEURISTIC)");
#endif

                        AvailableDirectionsAndTheirLengthTemp = AvailableDirectionsAndTheirLengthTemp.OrderByDescending(s => s.Item2).ToList(); // heuristic

                        Variables.Add(new Variable(Number, (i, j), AvailableDirectionsAndTheirLengthTemp));
#if SHOWSTEPS
                        Console.WriteLine("CREATING VARIABLE " + Number + ": Done");
#endif
                    }
                }
            }

#if SHOWSTEPS
            Console.WriteLine("Sorting variables by their max possible lengths (HEURISTIC)");
#endif

            Variables = Variables.OrderByDescending(s => s.AvailableDirectionsAndTheirLength.First().Item2).ToList(); // heuristic
        }

        public char[][] Solve()
        {
#if SHOWSTEPS
            Console.WriteLine("Begin to solving by calling recursive backtracking method");
            (var IsSuccess, var TheResultMatrix) = BackTrackingRecursive(Matrix, Domain, Variables, 1);
#else
            (var IsSuccess, var TheResultMatrix) = BackTrackingRecursive(Matrix, Domain, Variables);
#endif

            if (IsSuccess)
                return TheResultMatrix;
            else
                throw new InvalidOperationException();
        }

        public static void PrintMatrix(char[][] Matrix)
        {
            foreach (var theCharArray in Matrix)
                Console.WriteLine(theCharArray);
            
        }

#if SHOWSTEPS
        private static (bool, char[][]) BackTrackingRecursive(char[][] MatrixOriginal, List<string> Domain, List<Variable> Variables, int RecursiveCallDepth)
#else
        private static (bool, char[][]) BackTrackingRecursive(char[][] MatrixOriginal, List<string> Domain, List<Variable> Variables)
#endif
        {
#if SHOWSTEPS
            Console.WriteLine();
            Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Begin");
#endif

            if (Variables.Count == 0)
            {
#if SHOWSTEPS
                Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): There are no variables left to solve!");
                Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Found all solutions!");
                Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Returning success");
                Console.WriteLine("");
#endif
                return (true, MatrixOriginal);
            }
                

            Variable TheVariable = Variables.First();

#if SHOWSTEPS
            Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Got variable to solve: " + TheVariable.Number);
#endif

            foreach (var TheDirectionAndLength in TheVariable.AvailableDirectionsAndTheirLength)
            {
                List<string> CandidateWords = Domain.Where(a => a.Length == TheDirectionAndLength.Item2).ToList();

#if SHOWSTEPS
                Console.Write("BackTrackingRecursive(" + RecursiveCallDepth + "): Solving variable: " + TheVariable.Number + ": Trying solve by using direction "
                    + TheDirectionAndLength.Item1 + " with length of " + TheDirectionAndLength.Item2 + ": Candidate words are:");
                foreach (string TheCandidateWord in CandidateWords)
                    Console.Write(" " + TheCandidateWord);
                Console.WriteLine();
#endif

                foreach (string TheCandidateWord in CandidateWords)
                {
#if SHOWSTEPS
                    Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Solving variable: " + TheVariable.Number + ": Trying solve by using direction "
                        + TheDirectionAndLength.Item1 + " with length of " + TheDirectionAndLength.Item2 + ": Trying solve by using candidate word: "
                        + TheCandidateWord + "; Generating copy of matrix");
#endif
                    char[][] Matrix = MatrixOriginal.Select(a => a.ToArray()).ToArray();
                    char[] TheCandidateWordInCharArray = TheCandidateWord.ToCharArray();
                    bool TheCandidateWordSuccess = true;

#if SHOWSTEPS
                    Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Solving variable: " + TheVariable.Number + ": Trying solve by using direction "
                        + TheDirectionAndLength.Item1 + " with length of " + TheDirectionAndLength.Item2 + ": Trying solve by using candidate word: "
                        + TheCandidateWord + "; Begining to insert");
#endif

                    if (TheDirectionAndLength.Item1 == Directions.right)
                    {
                        for (int i = TheVariable.Position.Item2, j = 0; j < TheCandidateWord.Length; i++, j++)
                        {
                            if (Matrix[TheVariable.Position.Item1][i] == '_')
                                Matrix[TheVariable.Position.Item1][i] = TheCandidateWordInCharArray[j];

                            if (Matrix[TheVariable.Position.Item1][i] != TheCandidateWordInCharArray[j])
                            {
                                TheCandidateWordSuccess = false;
                                break;
                            }

                        }
                    }

                    if (TheDirectionAndLength.Item1 == Directions.down)
                    {
                        for (int i = TheVariable.Position.Item1, j = 0; j < TheCandidateWord.Length; i++, j++)
                        {
                            if (Matrix[i][TheVariable.Position.Item2] == '_')
                                Matrix[i][TheVariable.Position.Item2] = TheCandidateWordInCharArray[j];

                            if (Matrix[i][TheVariable.Position.Item2] != TheCandidateWordInCharArray[j])
                            {
                                TheCandidateWordSuccess = false;
                                break;
                            }
                        }
                    }

                    if (!TheCandidateWordSuccess)
                    {
#if SHOWSTEPS
                        Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Solving variable: " + TheVariable.Number + ": Trying solve by using direction "
                            + TheDirectionAndLength.Item1 + " with length of " + TheDirectionAndLength.Item2 + ": Trying solve by using candidate word: "
                            + TheCandidateWord + "; Failed to insert!");
                        Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Backtracking by continuing the loop");
#endif
                        continue;
                    }

#if SHOWSTEPS
                    Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Solving variable: " + TheVariable.Number + ": Trying solve by using direction "
                        + TheDirectionAndLength.Item1 + " with length of " + TheDirectionAndLength.Item2 + ": Trying solve by using candidate word: "
                        + TheCandidateWord + "; Inserted successfully!");
                    Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Last status of the matrix:");
                    PrintMatrix(Matrix);
                    Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Continue the solving process by calling BackTrackingRecursive again");
                    (var IsSuccess, var TheResultMatrix) = BackTrackingRecursive(Matrix, Domain.Where(a => a != TheCandidateWord).ToList(), Variables.Where(a => a != TheVariable).ToList(), RecursiveCallDepth + 1);
#else
                    (var IsSuccess, var TheResultMatrix) = BackTrackingRecursive(Matrix, Domain.Where(a => a != TheCandidateWord).ToList(), Variables.Where(a => a != TheVariable).ToList());
#endif

                    if (IsSuccess)
                    {
#if SHOWSTEPS
                        Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Got result from recursive BackTrackingRecursive: Success!");
                        Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Returning success");
                        Console.WriteLine();
#endif
                        return (true, TheResultMatrix);
                    }
#if SHOWSTEPS
                    else {
                        Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Got result from recursive BackTrackingRecursive: Not Success!");
                        Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Backtracing: Continuing loop");
                    }
#endif

                }
            }
#if SHOWSTEPS
            Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Solving variable: " + TheVariable.Number + ": Could not find a proper solution!");
            Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Failed to solve");
            Console.WriteLine("BackTrackingRecursive(" + RecursiveCallDepth + "): Backtracing: Returning fail");
            Console.WriteLine("");
#endif
            return (false, null);
        }
    }
}
