using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmo : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Type { NORMAL, WAYPOINT}
    private const string wayPointFile = "Enemy";
    public Type type = Type.NORMAL;
    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (type == Type.NORMAL)
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _radius);
        }
        else if (type == Type.WAYPOINT)
        {
            Gizmos.color = _color;
            //Enemy 이미지 파일을 표시
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.0f, wayPointFile, true);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

    }
}
