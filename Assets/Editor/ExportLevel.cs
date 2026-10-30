using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

public class ExportLevel
{
    [MenuItem("Tools/Export level as OBJ")]
    public static void Export()
    {
        string path = Application.dataPath + "/ExportedLevels/Level1.obj";

        List<Collider2D> grabbables = new();
        foreach (Grabbable grabbable in Object.FindObjectsByType<Grabbable>())
        {
            grabbables.Add(grabbable.gameObject.GetComponentInParent<Collider2D>());
        }

        StringBuilder obj = new();

        int vertexOffset = 0;

        obj.AppendLine("o grabbable");
        foreach (Collider2D col in grabbables)
        {
            Mesh mesh = CreateMesh(col);

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            if (vertices.Length == 0) continue;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 world;

                if (col is PolygonCollider2D)
                {
                    world = vertex + col.transform.position;
                }
                else
                {
                    world = col.transform.TransformPoint(vertex);
                }

                obj.AppendLine(
                    "v " +
                    FormatValue(world.x) + " " +
                    FormatValue(world.y) + " " +
                    FormatValue(world.z)
                );
            }

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int a = triangles[i];
                int b = triangles[i + 1];
                int c = triangles[i + 2];

                Vector3 va = col.transform.TransformPoint(
                    vertices[a]
                );

                Vector3 vb = col.transform.TransformPoint(
                    vertices[b]
                );

                Vector3 vc = col.transform.TransformPoint(
                    vertices[c]
                );

                float crossZ =
                    (vb.x - va.x) * (vc.y - va.y) -
                    (vb.y - va.y) * (vc.x - va.x);

                if (crossZ < 0)
                {
                    (c, b) = (b, c);
                }

                int ia = vertexOffset + a + 1;
                int ib = vertexOffset + b + 1;
                int ic = vertexOffset + c + 1;

                obj.AppendLine(
                    $"f {ia} {ib} {ic}"
                );
            }

            vertexOffset += vertices.Length;
            Object.DestroyImmediate(mesh);
        }

        File.WriteAllText(path, obj.ToString());

        Debug.Log("Export succesful!");
    }

    private static Mesh CreateMesh(Collider2D col)
    {
        switch (col)
        {
            case PolygonCollider2D polygon:
                return polygon.CreateMesh(false, true);

            case BoxCollider2D box:
                {
                    Vector2 size = box.size;
                    Vector2 offset = box.offset;

                    Vector3[] vertices =
                    {
                        new(offset.x - size.x / 2, offset.y - size.y / 2, 0),
                        new(offset.x + size.x / 2, offset.y - size.y / 2, 0),
                        new(offset.x + size.x / 2, offset.y + size.y / 2, 0),
                        new(offset.x - size.x / 2, offset.y + size.y / 2, 0)
                    };

                    int[] triangles =
                    {
                        0, 1, 2,
                        0, 2, 3
                    };

                    Mesh mesh = new()
                    {
                        vertices = vertices,
                        triangles = triangles
                    };
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    return mesh;
                }

            case CircleCollider2D circle:
                {
                    const int nVertices = 32;

                    float radius = circle.radius;
                    Vector2 offset = circle.offset;

                    List<Vector3> vertices = new()
                    {
                        new(offset.x, offset.y, 0)
                    };

                    float triAngle = 2 * Mathf.PI / nVertices;
                    for (int i = 0; i < nVertices; i++)
                    {
                        float angle = triAngle * i;
                        vertices.Add(new(Mathf.Cos(angle) * radius + offset.x, Mathf.Sin(angle) * radius + offset.y));
                    }

                    List<int> triangles = new();
                    for (int i = 0; i < nVertices; i++)
                    {
                        triangles.Add(0);
                        triangles.Add(i + 1);
                        if (i < nVertices - 1)
                        {
                            triangles.Add(i + 2);
                        }
                        else
                        {
                            triangles.Add(1);
                        }
                    }

                    Mesh mesh = new()
                    {
                        vertices = vertices.ToArray(),
                        triangles = triangles.ToArray()
                    };
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    return mesh;
                }

            default:
                Debug.Log("unknown Collider2D");
                return new();
        }
    }

    private static string FormatValue(float value)
    {
        return value.ToString(
            "R",
            CultureInfo.InvariantCulture
        );
    }
}
