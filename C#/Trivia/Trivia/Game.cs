﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Trivia
{
    public class Game
    {
        private readonly IConsole consoleWrapper;
        List<string> players = new List<string>();

        private const Int32 MaximumNumberOfPlayers = 6;
        int[] boardPositions = new int[MaximumNumberOfPlayers];
        int[] victoryPoints = new int[MaximumNumberOfPlayers];

        bool[] inPenaltyBox = new bool[MaximumNumberOfPlayers];

        LinkedList<string> popQuestions = new LinkedList<string>();
        LinkedList<string> scienceQuestions = new LinkedList<string>();
        LinkedList<string> sportsQuestions = new LinkedList<string>();
        LinkedList<string> rockQuestions = new LinkedList<string>();

        int _currentPlayer = 0;
        bool _isGettingOutOfPenaltyBox;

        public Game(IConsole consoleWrapper)
        {
            this.consoleWrapper = consoleWrapper;
            for (int i = 0; i < 50; i++)
            {
                popQuestions.AddLast("Pop Question " + i);
                scienceQuestions.AddLast(("Science Question " + i));
                sportsQuestions.AddLast(("Sports Question " + i));
                rockQuestions.AddLast(CreateRockQuestion(i));
            }
        }

        public String CreateRockQuestion(int index)
        {
            return "Rock Question " + index;
        }

        public bool IsPlayable()
        {
            const Int32 minimumNumberOfPlayers = 2;
            bool thereAreMinimumNumberOfPlayers = GetNumberOfPlayersParticipating() >= minimumNumberOfPlayers;
            return thereAreMinimumNumberOfPlayers;
        }

        public bool AddPlayer(String playerName)
        {
            players.Add(playerName);
            boardPositions[GetNumberOfPlayersParticipating()] = 0;
            victoryPoints[GetNumberOfPlayersParticipating()] = 0;
            inPenaltyBox[GetNumberOfPlayersParticipating()] = false;

            Console.WriteLine(playerName + " was added");
            Console.WriteLine("They are player number " + players.Count);
            return true;
        }

        public int GetNumberOfPlayersParticipating()
        {
            return players.Count;
        }

        public void MoveCurrentPlayer(int roll)
        {
            Console.WriteLine(players[_currentPlayer] + " is the current player");
            Console.WriteLine("They have rolled a " + roll);

            if (inPenaltyBox[_currentPlayer])
            {
                if (DiceComputation.IsOdd(roll))
                {
                    _isGettingOutOfPenaltyBox = true;

                    Console.WriteLine(players[_currentPlayer] + " is getting out of the penalty box");
                    boardPositions[_currentPlayer] = boardPositions[_currentPlayer] + roll;

                    bool boardHasReachedTheEnd = boardPositions[_currentPlayer] > 11;
                    if (boardHasReachedTheEnd) boardPositions[_currentPlayer] = boardPositions[_currentPlayer] - 12;

                    Console.WriteLine(players[_currentPlayer]
                            + "'s new location is "
                            + boardPositions[_currentPlayer]);
                    Console.WriteLine("The category is " + CurrentCategory());
                    AskQuestion();
                }
                else
                {
                    Console.WriteLine(players[_currentPlayer] + " is not getting out of the penalty box");
                    _isGettingOutOfPenaltyBox = false;
                }
            }
            else
            {
                boardPositions[_currentPlayer] = boardPositions[_currentPlayer] + roll;

                bool boardHasReachedTheEnd = boardPositions[_currentPlayer] > 11;
                if (boardHasReachedTheEnd) boardPositions[_currentPlayer] = boardPositions[_currentPlayer] - 12;

                Console.WriteLine(players[_currentPlayer]
                        + "'s new location is "
                        + boardPositions[_currentPlayer]);
                Console.WriteLine("The category is " + CurrentCategory());
                AskQuestion();
            }
        }

        private void AskQuestion()
        {
            LinkedList<string> questionList = GetQuestionListForCurrentCategory();

            string currentQuestion = questionList.First();
            consoleWrapper.ConsoleWriteLine(currentQuestion);
            questionList.RemoveFirst();
        }

        private LinkedList<string> GetQuestionListForCurrentCategory()
        {
            Dictionary<string, LinkedList<string>> questionCategories = GetQuestionCategoryTypes();
            return questionCategories[CurrentCategory()];
        }

        private Dictionary<string, LinkedList<string>> GetQuestionCategoryTypes()
        {
            Dictionary<string, LinkedList<string>> questionCategories = new Dictionary<string, LinkedList<string>>();
            questionCategories.Add("Pop", popQuestions);
            questionCategories.Add("Science", scienceQuestions);
            questionCategories.Add("Sports", sportsQuestions);
            questionCategories.Add("Rock", rockQuestions);
            return questionCategories;
        }

        private String CurrentCategory()
        {
            if (boardPositions[_currentPlayer] == 0) return "Pop";
            if (boardPositions[_currentPlayer] == 4) return "Pop";
            if (boardPositions[_currentPlayer] == 8) return "Pop";
            if (boardPositions[_currentPlayer] == 1) return "Science";
            if (boardPositions[_currentPlayer] == 5) return "Science";
            if (boardPositions[_currentPlayer] == 9) return "Science";
            if (boardPositions[_currentPlayer] == 2) return "Sports";
            if (boardPositions[_currentPlayer] == 6) return "Sports";
            if (boardPositions[_currentPlayer] == 10) return "Sports";
            return "Rock";
        }

        public bool ContinuePlaying()
        {
            if (inPenaltyBox[_currentPlayer] && !_isGettingOutOfPenaltyBox)
            {
                GoToNextPlayer();
                return true;
            }

            string message = "";
            if (inPenaltyBox[_currentPlayer] && _isGettingOutOfPenaltyBox)
            {
                message = "Answer was correct!!!!";
            }
            else
                message = "Answer was corrent!!!!";

            return MoveOutOfPenaltyBox(message);
        }

        private bool MoveOutOfPenaltyBox(string message)
        {
            AwardCoin(message);

            bool continuePlaying = PlayerDidNotWin();
            GoToNextPlayer();

            return continuePlaying;
        }

        private void GoToNextPlayer()
        {
            _currentPlayer++;
            if (currentPlayerIsLast()) _currentPlayer = 0;
        }

        private void AwardCoin(string message)
        {
            Console.WriteLine(message);
            victoryPoints[_currentPlayer]++;
            Console.WriteLine(players[_currentPlayer]
                              + " now has "
                              + victoryPoints[_currentPlayer]
                              + " Gold Coins.");
        }

        public bool WrongAnswer()
        {
            Console.WriteLine("Question was incorrectly answered");
            Console.WriteLine(players[_currentPlayer] + " was sent to the penalty box");
            inPenaltyBox[_currentPlayer] = true;

            GoToNextPlayer();

            return true;
        }

        private bool currentPlayerIsLast()
        {
            return _currentPlayer == players.Count;
        }

        private bool PlayerDidNotWin()
        {
            bool currentPlayerWonSixPoints = victoryPoints[_currentPlayer] == 6;
            return !currentPlayerWonSixPoints;
        }
    }
}