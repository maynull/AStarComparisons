using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int Index { get; set; }
    public int Value
    {
        get { return _value; }
        set
        {
            if (value == 0)
            {
                ValueText.gameObject.SetActive(false);
            }
            else
            {
                ValueText.gameObject.SetActive(true);
                ValueText.text = value.ToString();
            }
            _value = value;
        }
    }
    public Tile[] NeighbourTiles;
    public Text ValueText { get; private set; }

    private int _value;

    void Awake()
    {
        ValueText = transform.GetChild(0).GetComponent<Text>();
        if(ValueText.text == "0")
            ValueText.gameObject.SetActive(false);
    }

    public List<Node> GetSuccessors(Node node)
    {
        List<Node> result = new List<Node>();
        foreach (Tile neighbourTile in NeighbourTiles)
        {
            SuccessorCalculationRule.AddSuccessor(node, result, neighbourTile.Index);
        }
        return result;
    }

    public bool Match(Node node)
    {
        return node.EmptyTileIndex == Index;
    }
}
