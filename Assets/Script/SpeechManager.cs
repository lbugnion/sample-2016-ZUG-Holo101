using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    private KeywordRecognizer _keywordRecognizer;
    private readonly Dictionary<string, System.Action> _keywords = new Dictionary<string, System.Action>();
    private readonly System.Random _random = new System.Random();
    private GameObject _container;
    private GameObject _firstCube;

    // Use this for initialization
    private void Start()
    {
        // Note: Using GameObject.Find is not a good practice because:
        // - It has bad performance 
        // - It relies on "magic strings".
        // It is better to use references set in the editor like for the Cursor script.
        // I leave this here for educational purposes ;)
        // See https://akbiggs.silvrback.com/please-stop-using-gameobject-find
        _container = GameObject.Find("Container");
        _firstCube = GameObject.Find("Cube");

        _keywords.Add("Add cube", AddCube);
        _keywords.Add("Reset scene", ResetScene);
        _keywords.Add("Toggle", ToggleGravity);

        _keywordRecognizer = new KeywordRecognizer(_keywords.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        _keywordRecognizer.Start();
    }

    private void ToggleGravity()
    {
        Cursor.UseGravity = !Cursor.UseGravity;
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;
        if (_keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private void ResetScene()
    {
        foreach (Transform t in _container.transform)
        {
            var cube = t.gameObject;
            Destroy(cube);
        }
    }

    private void AddCube()
    {
        var newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.SetParent(_container.transform, true);
        newCube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        newCube.transform.rotation = Quaternion.Euler(
            45f,
            0,
            45f);

        newCube.transform.localPosition = new Vector3(0f, 0f, 0f);

        var rigidBody = newCube.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;

        var renderer = _firstCube.GetComponent<Renderer>();

        var material = new Material(renderer.material.shader);

        var color = new Color(
            (float)_random.NextDouble(),
            (float)_random.NextDouble(),
            (float)_random.NextDouble());

        material.SetColor("_Color", color);

        var newRenderer = newCube.GetComponent<Renderer>();
        newRenderer.material = material;
    }
}