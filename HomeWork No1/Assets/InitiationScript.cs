using System.Collections.Generic;
using NUnit.Framework;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class InitiationScript : MonoBehaviour
{

    // choose number of primitives etc 
    // instantiate circle 
    // then check "Animation" bool for oscilliation

    public Vector3 Center;
    public float Radius;
    public int NumberOfPrimitives = 20;
    public float Delay = 1.0f; //delay for each primitive
    private float _delay; //countdown for each delay
    public float OscillationRange = 0.1f;
    public bool Animate = false;
    private int primitiveIndex = 0;
    private bool _canCreate = true;

    [SerializeField]
    private GameObject[] _primitives;

    public void Start()
    {
        //initialize
        if (NumberOfPrimitives <= 0)
        {
            Destroy(this);  //Something wrong //avoid division by 0 later
        }

        Center = transform.position;
        _delay = Delay;

        _primitives = new GameObject[NumberOfPrimitives];

        InitCircle(Center, Radius); //initialize circle formation
    }

    private void Update()
    {
        _delay -= Time.deltaTime; //time since last frame // _delay = _delay - dT


        if (_delay <= 0 && primitiveIndex < NumberOfPrimitives) //when _delay gets to 0, it resets to Delay and gets to the next primitive creation
        {
            _delay = Delay;

            CreatePrimitive();

            primitiveIndex++;
        }

        if (Animate) // need to set bool
        {
            MovePrimitives();
        }
    }

    //initialize a circle using center x,y,z and float radius
    public void InitCircle(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }


    //create primitives in a Circle
    private void CreatePrimitive() 
    {
        if (!_canCreate)
        {
            Destroy(_primitives[NumberOfPrimitives - primitiveIndex--].gameObject);
            _canCreate = primitiveIndex == 0;
        }
        else
        {


            //each primitive's posisition on the circle 
            Vector3 currentPos = PointOnCircle(Center, Radius, primitiveIndex);

            //choose random primitive type 
            //primtype is an enum (list of specific values)
            int primNumber = Random.Range(0, System.Enum.GetValues(typeof(PrimitiveType)).Length - 2);

            //maxIndex = numOfPrims - 2 ("-2" bcs of Unity isssues)
            GameObject currentPrimitive = GameObject.CreatePrimitive((PrimitiveType)primNumber);

            //set position //each primitive has a Transform that holds its Position Rotation Scale 
            currentPrimitive.transform.position = currentPos;

            //calc angle
            float angle = 2 * Mathf.PI * Mathf.Rad2Deg * (float)primitiveIndex / NumberOfPrimitives - 90;
            //angle between prims (360/numprims) 
            // *i για να πάνε στα i πολλαπλάσια των γωνιών

            //set rotation
            currentPrimitive.transform.rotation = Quaternion.AngleAxis(angle, currentPrimitive.transform.forward);

            _primitives[primitiveIndex] = currentPrimitive;
            _canCreate = primitiveIndex < NumberOfPrimitives;
        }
    }

    private void MovePrimitives()
    {
        foreach (var primitive in _primitives)
        {
            if (primitive == null) continue;

            Vector3 direction = (primitive.transform.position - Center).normalized;
            primitive.transform.position += direction * OscillationRange * Mathf.Cos(Time.time); 

        }
    }
    Vector3 PointOnCircle(Vector3 center, float radius, int index)
    {
        Vector3 position = new Vector3();
        position.x = center.x + radius * Mathf.Cos(2 * Mathf.PI * (float)index / NumberOfPrimitives);
        position.z = center.z;
        position.y = center.z + radius * Mathf.Sin(2 * Mathf.PI * (float)index / NumberOfPrimitives);
        return position;
    }
}