using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public enum Algorithm
    {
        AStar,
        RTAStar,
        MARTAStar
    }
    public static TileManager Instance { get; private set; }
    public Button ShuffleButton, SolveButton;
    public Text StatusText;
    public Text[] MartaStatusTexts;
    public Tile[] Tiles;
    public static System.Random R = new System.Random();
    public  Algorithm ChosenAlgorithm;
    private int[] _premadePuzzleStart;
    public InputField DepthBox;
    private int _depth = 5;
    void Awake()
    {
        Instance = this;
        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i].Index = i;
        }
        Init();
    }

    private void Init()
    {
        MartaStatusTexts.ToList().ForEach(x => x.gameObject.SetActive(false));
        SetTilesFromNode(GetNodeFromTiles());
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public void Stop()
    {
        StopAllCoroutines();
        ShuffleButton.interactable = true;
        SolveButton.interactable = true;
    }

    public void SetChosenAlgorithm(int option)
    {
        switch (option)
        {
            case 0:
                Init();
                SetTilesFromNode(GetNodeFromTiles());
                ChosenAlgorithm = Algorithm.AStar;
                DepthBox.gameObject.SetActive(false);
                break;
            case 1:
                Init();
                SetTilesFromNode(GetNodeFromTiles());
                ChosenAlgorithm = Algorithm.RTAStar;
                DepthBox.gameObject.SetActive(true);
                break;
            case 2:
                Init();
                SetTilesFromNode(GetNodeFromTiles());
                ChosenAlgorithm = Algorithm.MARTAStar;
                DepthBox.gameObject.SetActive(true);
                break;
        }
    }

    public void ChoosePremadeStartNode(int option)
    {
        switch (option)
        {
            case 0:
                _premadePuzzleStart = new[] {4, 1, 3, 0, 2, 6, 7, 5, 8};
                break;
            case 1:
                _premadePuzzleStart = new[] {3, 0, 5, 7, 8, 6, 1, 2, 4};
                break;
            case 2:
                _premadePuzzleStart = new[] {8, 6, 7, 2, 5, 4, 3, 0, 1};
                break;
        }
        SetTilesFromNode(GetNodeFromTiles());
    }

    public void ShuffleTiles()
    {
        StatusText.gameObject.SetActive(false);
        _premadePuzzleStart = Enumerable.Range(0, 9).OrderBy(t => R.Next()).ToArray();
        SetTilesFromNode(GetNodeFromTiles());
    }

    public void SolvePuzzle()
    {
        ShuffleButton.interactable = false;
        SolveButton.interactable = false;
        StatusText.gameObject.SetActive(true);
        var startNode = GetNodeFromTiles();
        SetTilesFromNode(startNode);
        var goalNode = new Node
        {
            Tiles = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 }
        };
        int depth;
        int.TryParse(DepthBox.text, out depth);
        if (depth != 0)
            _depth = depth;

        switch (ChosenAlgorithm)
        {
            case Algorithm.AStar:
                SolvePuzzleWithAStar(startNode, goalNode);
                break;
            case Algorithm.RTAStar:
                Algorithms.RTAStar.PathFinder.StoredNodes.Clear();
                SolvePuzzleWithRtaStar(startNode, goalNode);
                break;
            case Algorithm.MARTAStar:
                Algorithms.RTAStar.PathFinder.StoredNodes.Clear();
                MartaStatusTexts.ToList().ForEach(x => x.gameObject.SetActive(true));
                SolvePuzzleWithMarta(startNode, goalNode);
                break;
        }
    }

    public void SolvePuzzleWithAStar(Node startNode, Node goalNode)
    {
        StartCoroutine(Algorithms.AStar.PathFinder.Execute(startNode, goalNode, StatusText, SetFinalResult));
    }

    public void SolvePuzzleWithRtaStar(Node startNode, Node goalNode)
    {
        Algorithms.RTAStar.PathFinder finder = new Algorithms.RTAStar.PathFinder();
        StartCoroutine(finder.Execute(startNode, goalNode, _depth,
            SetFinalResult, SetTilesFromNode, s => StatusText.text = "Solving at " + s + " cycle"));

    }
    public void SolvePuzzleWithMarta(Node startNode, Node goalNode)
    {
        Agent[] agents = new Agent[3];
        for (int i = 0; i < 3; i++)
        {
            agents[i] = new Agent();
        }
        List<IEnumerator> agentRoutines = new List<IEnumerator>();
        for (var index = 0; index < agents.Length; ++index)
        {
            int i = index;
            Agent agent = agents[index];
            var routine = agent.AgentStep(i, startNode, goalNode, _depth, (result, cycle) =>
            {
                SetFinalResult(result, cycle);
                foreach (var agentRoutine in agentRoutines)
                {
                    StopCoroutine(agentRoutine);
                }
                agentRoutines.Clear();
            }, SetTilesFromNode, s => MartaStatusTexts[i].text = "Agent " + i +" at " + s + " cycle");
            agentRoutines.Add(routine);
            StartCoroutine(routine);
        }
    }

    private void SetFinalResult(Node result, int cycles)
    {
        ShuffleButton.interactable = true;
        SolveButton.interactable = true;
        if (result == null)
        {
            StatusText.gameObject.SetActive(true);
            StatusText.text = "No Solution";
        }
        else
        {
            SetTilesFromNode(result);
            TimeSpan totalTime;
            switch (ChosenAlgorithm)
            {
                case Algorithm.AStar:
                    totalTime = TimeSpan.FromSeconds(Algorithms.AStar.PathFinder.TotalTime);
                    StatusText.text = "Finished Solving in " + cycles + "\ncycles " + GetStep(result) + " steps and " +
                                      string.Format("{0}", totalTime.Seconds) + " sec.";
                    break;
                case Algorithm.RTAStar:
                    totalTime = TimeSpan.FromSeconds(Algorithms.RTAStar.PathFinder.TotalTime);
                    StatusText.text = "Finished Solving in " + cycles + " cycles and " +
                                      string.Format("{0}", totalTime.Seconds) + " sec.";
                    break;
                case Algorithm.MARTAStar:
                    MartaStatusTexts.ToList().ForEach(x => x.gameObject.SetActive(true));
                    totalTime = TimeSpan.FromSeconds(Algorithms.RTAStar.PathFinder.TotalTime);
                    var finishedIndex = MartaStatusTexts.ToList()
                        .FindIndex(x => int.Parse(Regex.Match(x.text, @"(\d+)(?!.*\d)").Value) == cycles);
                    MartaStatusTexts[finishedIndex].text = "Agent " + finishedIndex + " solved in " + cycles +
                                                           " cycles and " + string.Format("{0}", totalTime.Seconds) +
                                                           " sec.";
                    break;
            }
        }
    }

    public int GetStep(Node resultNode)
    {
        var stack = new Stack<Node>();
        do
        {
            stack.Push(resultNode);
        } while ((resultNode = resultNode.Parent) != null);
        return stack.Count;
    }

    private Node GetNodeFromTiles()
    {
        var startNode = new Node();
        if (_premadePuzzleStart != null)
            startNode.Tiles = _premadePuzzleStart;
        else
        {
            int[] startTiles = new int[9];
            for (int i = 0; i < Tiles.Length; i++)
            {
                startTiles[i] = Tiles[i].Value;
            }
            startNode.Tiles = startTiles;
        }
        return startNode;
    }

    public void SetTilesFromNode(Node node)
    {
        for (int i = 0; i < node.Tiles.Length; i++)
        {
            Tiles[i].Value = node.Tiles[i];
        }
    }

    public List<Node> GenerateSuccessors(Node curNode)
    {
        var matchedTile = Tiles.Single(r => r.Match(curNode));
        return matchedTile.GetSuccessors(curNode);
    }
}
