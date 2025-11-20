using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueVisualManager : MonoBehaviour
{
    [Header("Prefabs")]
    
    [Tooltip("The button that will pop up containing a dialogue option for the player.")]
    [SerializeField] private GameObject _optionPrefab;
    [Tooltip("The text bubble that will appear above characters when they are speaking.")]
    [SerializeField] private GameObject _textBubblePrefab;
    [Tooltip("The canvas in which your text bubles are meant to appear in.")]
    [SerializeField] private GameObject _textBubbleCanvasPrefab;
    
    [Header("References")]
    
    [Tooltip("The object containing the canvas in which the dialogue options will appear")]
    [SerializeField] private GameObject _optionsCanvas;
    
    [Header("Tweening Settings")]
    [SerializeField] public float _tweenTime = 0.1f;
    [SerializeField] public Ease _tweenEase = Ease.Linear;
    
    private Transform _npcTransform;
    private Transform _playerTransform;

    private GameObject _playerTextBubbleCanvas;
    private TextMeshProUGUI _playerTextBubble;
    private Dictionary<GameObject, DialogueNode> _playerOptions = new Dictionary<GameObject, DialogueNode>();
    
    private GameObject _npcTextBubbleCanvas;
    private TextMeshProUGUI _npcTextBubble;
    
    private Transform _cameraTransform;
    
    private void Start()
    {
        _cameraTransform = Camera.main?.transform;
        if (_cameraTransform == null)
        {
            Debug.LogWarning("Main camera transform is null. Disabling dialogue visual.");
            this.enabled = false;
        }
        
        InstantiatePlayerCanvas();
    }

    private void Update()
    {
        _playerTextBubbleCanvas.transform.LookAt(_cameraTransform);
        
        if (_npcTextBubbleCanvas != null)
            _npcTextBubbleCanvas.transform.LookAt(_cameraTransform);
    }
    
    public void InstantiateCharacterCanvas(Transform character)
    {
        _npcTransform = character;

        _npcTextBubbleCanvas = Instantiate(_textBubbleCanvasPrefab, _npcTransform);
        _npcTextBubbleCanvas.transform.localScale = Vector3.zero;
        _npcTextBubbleCanvas.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 1);
        _npcTextBubbleCanvas.transform.LookAt(Camera.main?.transform);
        
        _npcTextBubble = Instantiate(_textBubblePrefab, _npcTextBubbleCanvas.transform)
            .GetComponentInChildren<TextMeshProUGUI>();
    }

    public void DisplayTextBubble(DialogueNode node)
    {
        // If node contains multiple next node, that means the player will have options on how to respond
        // therefore we can deduce it is the character speaking
        if (node.NextNodes.Count > 1)
        {
            _npcTextBubbleCanvas.SetActive(true);
            _npcTextBubble.text = node.Text;
        }
        else
        {
            _playerTextBubbleCanvas.SetActive(true);
            _playerTextBubble.text = node.Text;
        }
    }

    public void DisplayOptions(List<BaseNode> options, UnityAction<BaseNode> chooseOption)
    {
        // Catch if canvas doesnt exist for some reason
        if (_optionsCanvas == null)
        {
            Debug.LogWarning("Options canvas is null.");
            return;
        }
        
        // Get object in which options will be spawned
        Transform optionsParent = _optionsCanvas
            .GetComponentInChildren<HorizontalLayoutGroup>()
            .transform;
        
        // Ensures previous options and option buttons are removed
        ClearPreviousOptions();
        
        // Enable options canvas
        _optionsCanvas.SetActive(true);
        
        // Loop through each option and spawn button
        foreach (var optionNode in options)
        {
            if (optionNode == null)
                continue;

            if (optionNode is EndNode)
            {
                Debug.LogWarning("End Node cannot be an option. Ignoring it.");
                continue;
            }

            DialogueNode option = optionNode as DialogueNode;

            // Spawn button in appropriate canvas
            GameObject optionButton = Instantiate(
                _optionPrefab,
                optionsParent
            );
            
            TextMeshProUGUI optionText = optionButton.GetComponentInChildren<TextMeshProUGUI>();
            Button optionButtonButton = optionButton.GetComponent<Button>();
            
            optionText.text = option.Text;
            optionButtonButton.onClick.AddListener(() => chooseOption(option));
            
            _playerOptions.Add(optionText.gameObject, option);
        }
    }
    
    public void DestroyNpcCanvas() => Destroy(_npcTextBubbleCanvas);
    
    public void HideAllCanvases()
    {
        HideCanvas(_playerTextBubbleCanvas);
        HideCanvas(_npcTextBubbleCanvas);
        HideCanvas(_optionsCanvas);
    }

    private void HideCanvas(GameObject canvas)
    {
        if (canvas == null) return; // not created yet
    
        var worldCanvas = canvas.GetComponent<WorldCanvas>();
        if (worldCanvas != null)
            worldCanvas.Hide();
        else
            canvas.SetActive(false); // fallback if component missing
    }
    
    private void InstantiatePlayerCanvas()
    {
        if (_playerTransform == null)
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        _playerTextBubbleCanvas = Instantiate(_textBubbleCanvasPrefab, _playerTransform);
        _playerTextBubbleCanvas.transform.localScale = Vector3.zero;
        _playerTextBubbleCanvas.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, GetMeshTopY(_playerTransform.gameObject) * 3, 0);
        _playerTextBubbleCanvas.transform.LookAt(Camera.main?.transform);
        
        _playerTextBubbleCanvas.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("RenderCamera").GetComponent<Camera>();
        
        _playerTextBubble = Instantiate(_textBubblePrefab, _playerTextBubbleCanvas.transform)
            .GetComponentInChildren<TextMeshProUGUI>();
    }
    
    private void ClearPreviousOptions()
    {
        // Clear UI Options
        foreach (var pair in _playerOptions)
        {
            Destroy(pair.Key.transform.parent.gameObject);
        }
        
        // Clear option variables
        _playerOptions.Clear();
    }
    
    // Finds the Y position of the top of a gameobject's mesh
    private float GetMeshTopY(GameObject go)
    {
        SkinnedMeshRenderer mf = go.GetComponentInChildren<SkinnedMeshRenderer>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("GameObject has no MeshFilter or mesh.");
            return go.transform.position.y; // fallback to pivot position
        }

        Mesh mesh = mf.sharedMesh;
        Bounds bounds = mesh.bounds;

        // Top of mesh in local space
        Vector3 localTop = new Vector3(0f, bounds.max.y, 0f);

        // Convert to world space
        Vector3 worldTop = go.transform.TransformPoint(localTop);

        return worldTop.y;
    }
}
