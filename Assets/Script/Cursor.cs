using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class Cursor : MonoBehaviour
{
    public GameObject TheCursor;
    public Material InactiveCursorMaterial;
    public Material ActiveCursorMaterial;
    public Material ActiveFloatingCursorMaterial;

    private GestureRecognizer _gestureRecognizer;
    private RaycastHit _hitInfo;
    private GameObject _focusedObject;

    public static bool UseGravity
    {
        get;
        set;
    }

    // Use this for initialization
    private void Start()
    {
        UseGravity = true;

        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.TappedEvent += GestureRecognizerOnTappedEvent;
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        _gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizerOnTappedEvent(InteractionSourceKind source, int tapcount, Ray headray)
    {
        if (_focusedObject == null)
        {
            return;
        }

        var rigidBody = _focusedObject.GetComponent<Rigidbody>();

        if (rigidBody != null)
        {
            if (UseGravity)
            {
                if (rigidBody.useGravity)
                {
                    rigidBody.AddForce(-_hitInfo.normal * 100f);
                }
                else
                {
                    rigidBody.useGravity = true;
                }
            }
            else
            {
                rigidBody.AddForce(-_hitInfo.normal * 100f);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
	    if (TheCursor == null)
	    {
	        return;
	    }

        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        var cursorRenderer = TheCursor.GetComponent<Renderer>();

        if (Physics.Raycast(headPosition, gazeDirection, out _hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            // But not the spatial mapping!
            var obj = _hitInfo.collider.gameObject;

            if (!obj.name.StartsWith("spatial")) // Fugly...
            {
                _focusedObject = obj;

                if (UseGravity)
                {
                    cursorRenderer.material = ActiveCursorMaterial;
                }
                else
                {
                    cursorRenderer.material = ActiveFloatingCursorMaterial;
                }
            }
            else
            {
                _focusedObject = null;
                cursorRenderer.material = InactiveCursorMaterial;
            }

            TheCursor.transform.position = _hitInfo.point;
            TheCursor.transform.forward = _hitInfo.normal;
        }
    }
}