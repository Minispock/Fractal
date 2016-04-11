using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

    public Mesh[] meshes;
    public Material material;
    public int maxDepth;
    private int depth;
    public float childScale;
    public float spawnProbability;
    public float maxRotationSpeed;
    private float rotationSpeed;
    public float maxTwist;

    private Material[,] materials;

    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1, 3];
        for (int i = 0; i <= maxDepth; i++)
        {
            float t = i / (maxDepth - 1F);
            t *= t;
            materials[i, 0] = new Material(material);
            materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
            materials[i, 1] = new Material(material);
            materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
            materials[i, 2] = new Material(material);
            materials[i, 2].color = Color.Lerp(Color.white, Color.green, t);
        }

        materials[maxDepth, 0].color = Color.magenta;
        materials[maxDepth, 1].color = Color.black;
        materials[maxDepth, 2].color = Color.blue;
    }

    // Use this for initialization
    void Start()
    {
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0F, 0F);//twisting

        if (materials == null)
        {
            InitializeMaterials();
        }

        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 3)];
        //GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.yellow, (float)depth / maxDepth); //Lerp is shorthand for linear interpolation. Its typical signature is Lerp(a, b, t) and it computes a + (b - a) * t, with t clamped to the 0–1 range. Multiple versions exist for various types, including floats, vectors, and colors.

        if (depth < maxDepth)
        {

            StartCoroutine(CreateChildren());
            //new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.up);
            //new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.right);
        }
	
	}

    private void Initialize(Fractal parent, int childIndex)
    {
        meshes = parent.meshes;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        maxTwist = parent.maxTwist;
        depth = parent.depth + 1;
        childScale = parent.childScale; // pass this value from parent to child 
        transform.parent = parent.transform; //вкладывает в иерархии объекты друг в друга (children with nesting)
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[childIndex] * (0.5F + 0.5F * childScale);
        transform.localRotation = chieldOrientations[childIndex];
        spawnProbability = parent.spawnProbability;
        maxRotationSpeed = parent.maxRotationSpeed;
    }

    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] chieldOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0F, 0F, -90F),
        Quaternion.Euler(0F, 0F, 90F),
        Quaternion.Euler(-90F, 0F, 0F),
        Quaternion.Euler(90F, 0F, 0F)
    };

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            if (Random.value < spawnProbability)
            {
                yield return new WaitForSeconds(Random.Range(0.1F, 0.5F));
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);//creating childs
            }
            
        }       

    }

    
	// Update is called once per frame
	void Update () {

        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);//to move

    }
}
