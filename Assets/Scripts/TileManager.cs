using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }
    public Button ShuffleButton, SolveButton;
    public Text StatusText;
    public Text[] MartaStatusTexts;
    public Tile[] Tiles;
    public static System.Random R = new System.Random();
    void Awake()
    {
        Instance = this;
        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i].Index = i;
        }
        StatusText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public void ShuffleTiles()
    {
        StatusText.gameObject.SetActive(false);
        var array = Enumerable.Range(0, 9).OrderBy(t => R.Next()).ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            Tiles[i].Value = array[i];
        }
    }

    public void SolvePuzzleWithAStar()
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
        StartCoroutine(Algorithms.AStar.PathFinder.Execute(startNode, goalNode, StatusText, SetFinalResult));

    }

    public void SolvePuzzleWithRtaStar()
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
        Algorithms.RTAStar.PathFinder finder = new Algorithms.RTAStar.PathFinder();
        StartCoroutine(finder.Execute(startNode, goalNode, 5,
            SetFinalResult, SetTilesFromNode, s => StatusText.text = s));

    }

    private int latestRoutine = 0;
    public void SolvePuzzleWithMarta()
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
            Debug.Log(MartaStatusTexts[index].text);
            var routine = agent.AgentStep(startNode, goalNode, 5, result =>
            {
                SetFinalResult(result);
                foreach (var agentRoutine in agentRoutines)
                {
                    StopCoroutine(agentRoutine);
                }
                agentRoutines.Clear();
            }, SetTilesFromNode, s => MartaStatusTexts[i].text = s);
            agentRoutines.Add(routine);
        }
        StartCoroutine(agentRoutines[latestRoutine]);
    }

    private void SetFinalResult(Node result)
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
            //StatusText.gameObject.SetActive(false);
            SetTilesFromNode(result);
            StatusText.text = "Finished " + StatusText.text;
        }
    }

    private Node GetNodeFromTiles()
    {
        int[] startTiles = new int[9];
        for (int i = 0; i < Tiles.Length; i++)
        {
            startTiles[i] = Tiles[i].Value;
        }
        var startNode = new Node
        {
            Tiles = new[] { 8, 6, 7, 2, 5, 4, 3, 0, 1 }
        };
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
        //Debug.Log(curNode.EmptyTileIndex);
        var matchedTile = Tiles.Single(r => r.Match(curNode));
        return matchedTile.GetSuccessors(curNode);
    }
}
