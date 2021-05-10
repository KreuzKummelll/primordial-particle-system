using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PrimordialParticle : MonoBehaviour
{
    
    
    [SerializeField] private bool _drawRadiusGizmos;

    [SerializeField] private Material[] _materials;


    /// <summary>
    /// "Bernstein's constant"
    /// </summary>
    private double β = 0.280169499023f;


    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigid2d;
    private Collider2D _collider;
    private Collider2D[] _neighbors;


    /// <summary>
    /// The `awareness` radius of the particle 
    /// </summary>
    [Tooltip("The radius of the particle influence, aka 'awareness influence'")]
    [RangeAttribute(0, 5)]
    public float r = 3;

    /// <summary>
    /// The orientation of the particle, aka rotation around the z axis
    /// </summary>
    [Tooltip("The orientation of the particlex")]
    [SerializeField] private float φ;

    /// <summary>
    /// A constant vector speed
    /// </summary>
    [Tooltip("Constant speed")]
    [RangeAttribute(0,10)]
    [SerializeField] private float v;


    /// <summary>
    /// The angle that the particle rotates every timestep
    /// </summary>
    [Tooltip("the angle that the particle rotates every timestep")]
    [SerializeField] private double α;

    /// <summary>
    /// The number of particles within radius r
    /// </summary>
    [Tooltip("The number of particles in the radius r")]
    [SerializeField] private int Nt;

    [SerializeField] private int ntLeft = 0;
    [SerializeField] private int ntRight = 0;


    public Vector2 pos
    {
        get => new Vector2(transform.position.x, transform.position.y);
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigid2d = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _neighbors = new Collider2D[10];
    }

    // Update is called once per frame
    void Update()
    {
        
        _rigid2d.velocity = Vector2.one * v;

        if (FindNeighborsInArea())
        {
            print("total neighbors :" + Nt);
            UpdateMaterial();
            

        }

        CalculateInfluenceFromNeighbors();
        CalculateAngle();
        CalculateFinalMotion();
        
        ApplyFinalMotion();
    }

    private void OnDrawGizmos()
    {
        if (_drawRadiusGizmos)
        {
            Gizmos.DrawWireSphere(transform.position, r);
        }
    }

    /// <summary>
    /// It finds the number of neighbors in the area by the radius.
    /// </summary>
    /// <returns>Represents if the value was updated since last time. If truthy, update material and other data</returns>
    bool FindNeighborsInArea()
    {
        List<Collider2D> localNeighbors = new List<Collider2D>();
        var list = Physics2D.OverlapCircleAll(_rigid2d.position, r);
        localNeighbors.AddRange(list);
        if (localNeighbors.Contains(_collider)) { localNeighbors.Remove(_collider); }
        // must subtract one, or else the sprite will be a neighbor to itself
        if (Nt == localNeighbors.Count) return false;
        Nt = localNeighbors.Count;
        
        _neighbors = localNeighbors.ToArray();
        return true;

    }

    void UpdateMaterial()
    {
        if (Nt < 0) return; 
        if (Nt == 0) {
            _spriteRenderer.material = _materials[0];
        } else if (Nt == 1)
        {
            _spriteRenderer.material = _materials[1];
        } else if (Nt == 2)
        {
            _spriteRenderer.material = _materials[2];
        } else if (Nt > 2)
        {
            _spriteRenderer.material = _materials[3];
        }
    }

    void CalculateInfluenceFromNeighbors()
    {

       
        if (_neighbors.Length < 1) return;

        ntLeft = 0;
        ntRight = 0;

        
        foreach (var neighbor in _neighbors)
        {
            if (neighbor == _collider) continue;
            Vector3 neighborPos = neighbor.transform.GetComponent<Rigidbody2D>().position;
            float dotProduct = Vector2.Dot(pos - new Vector2(neighborPos.x, neighborPos.y), -transform.right);
            if (dotProduct > 0)
            {
                ntRight++;
            } else if (dotProduct < 0)
            {
                ntLeft++;
            }
        }


    }

    void CalculateAngle()
    {
        if (gameObject.name != "Particle") return;
        //α = Mathf.Sign((ntRight - ntLeft) * β * Nt);
        float ntDiff = ntRight - ntLeft;
        double signOfNtDif = Mathf.Sign(ntDiff);
        double signOfNtDifTimesβ = signOfNtDif * β;
        α = signOfNtDifTimesβ * Nt;
        //print("α = " + α);
    }


    void CalculateFinalMotion()
    {
        //  Δφ/Δt = α + β * Nt * sign(Rt - Lt)

        var basf = ((α + (β * (Nt * Mathf.Sign(ntRight - ntLeft)))) / Time.deltaTime);
        
        φ += (float)basf;
    }

    void ApplyFinalMotion()
    {
        _rigid2d.SetRotation(Quaternion.AngleAxis(φ, Vector3.forward));
    }
}
