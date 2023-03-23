using OpenTK;
using System;
using System.Collections.Generic;
using System.IO; 

namespace OBJ_Import
{
    class ObjParser
    {
        static private List<Vector3> vertices = new List<Vector3>();
        static private List<Vector3> normals = new List<Vector3>();
        static private List<Vector2> texCoords = new List<Vector2>();
        static private List<int[]> faces = new List<int[]>();

        static public void ParseFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length == 0)
                        continue;

                    switch (tokens[0])
                    {
                        case "v":
                            float x = float.Parse(tokens[1]);
                            float y = float.Parse(tokens[2]);
                            float z = float.Parse(tokens[3]);
                            vertices.Add(new Vector3(x, y, z));
                            break;

                        case "vn":
                            float nx = float.Parse(tokens[1]);
                            float ny = float.Parse(tokens[2]);
                            float nz = float.Parse(tokens[3]);
                            normals.Add(new Vector3(nx, ny, nz));
                            break;

                        case "vt":
                            float u = float.Parse(tokens[1]);
                            float v = float.Parse(tokens[2]);
                            texCoords.Add(new Vector2(u, v));
                            break;

                        case "f":
                            int[] face = new int[9];
                            for (int i = 0; i < 3; i++)
                            {
                                string[] vertexTokens = tokens[i + 1].Split(new char[] { '/' }, StringSplitOptions.None);
                                face[i * 3] = int.Parse(vertexTokens[0]) - 1; // vertex index
                                face[i * 3 + 1] = int.Parse(vertexTokens[1]) - 1; // texture coordinate index
                                face[i * 3 + 2] = int.Parse(vertexTokens[2]) - 1; // normal index
                            }
                            faces.Add(face);
                            break;
                    }
                }
            }
        }

        public static List<Vector3> Vertices { get { return vertices; } }
        public static List<Vector3> Normals { get { return normals; } }
        public static List<Vector2> TexCoords { get { return texCoords; } }
        public static List<int[]> Faces { get { return faces; } }
    }
}

