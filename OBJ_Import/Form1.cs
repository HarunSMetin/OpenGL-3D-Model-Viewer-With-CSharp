using System;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

using StbImageSharp;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Globalization;

namespace OBJ_Import
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            this.InitializeComponent(); 
        }
        private void btnLoadModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "OBJ File |*.obj";
            file.Title = "Select your OBJ File..";
            if (file.ShowDialog() == DialogResult.OK)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Clear(ClearBufferMask.DepthBufferBit); 
                ModelLoadToBuffer(file.FileName.Substring(0, file.FileName.IndexOf(".obj")));
                glControl1.Invalidate();
            }
        }
        Thread thread = null;
        private void btnRotate_Click(object sender, EventArgs e)
        {
            if (thread == null)
            {
                thread = new Thread(() =>
                {
                    while (true)
                    {
                        x_angle_3d_model += 0.05f;
                        y_angle_3d_model -= 0.05f;
                        z_angle_3d_model += 0.05f;
                        glControl1.Invalidate();
                        Thread.Sleep(100);
                    }
                });
                thread.Start();
            }
            else
            {
                thread.Abort();
                x_angle_3d_model = 0;
                y_angle_3d_model = 0;
                z_angle_3d_model = 0;
                thread = null;
                glControl1.Invalidate();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }

        #region 3D NESNE OPENGL

        struct Material
        {
            public Vector3 ambient;
            public Vector3 diffuse;
            public Vector3 specular;
            public float shininess;
        };

        struct DirectionalLight
        {
            public Vector3 direction;
            public Vector3 ambient;
            public Vector3 diffuse;
            public Vector3 specular;
            public float intensity;
        };


        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> texCoords = new List<Vector2>();
        private List<int[]> faces = new List<int[]>();

        int vertexCount;
        int VertexBufferObject, VertexArrayObject, ShaderObject, TextureObject;
        float x_angle_3d_model = 0, y_angle_3d_model = 0, z_angle_3d_model = 0;


        Material modelMat;
        DirectionalLight light;

        Vector3 cameraDir = new Vector3(0, 0, 0);
        Vector3 cameraPos = new Vector3(0, 0, 6);

        float coordinateLinesLong = 10;
        float moveVelocity = 1f;
        private void Form1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            Console.WriteLine(e.KeyChar); 
            char key = e.KeyChar.ToString().ToUpper()[0];
            if (key == (char)Keys.W)
            {
                cameraPos.Z -= moveVelocity;
                coordinateLinesLong+=moveVelocity;
            }
            else if (key == (char)Keys.S)
            {
                cameraPos.Z += moveVelocity;
                coordinateLinesLong+=moveVelocity;
            }
            if (key == (char)Keys.A)
            {
                cameraPos.X -= moveVelocity;
                coordinateLinesLong+=moveVelocity;
            }
            else if (key == (char)Keys.D)
            {
                cameraPos.X += moveVelocity;
                coordinateLinesLong+=moveVelocity;
            }
            if (key == (char)Keys.Space || key == (char)Keys.Q)
            {
                cameraPos.Y += moveVelocity;
                coordinateLinesLong+=moveVelocity;
            }
            else if (key == (char)Keys.E)
            {
                cameraPos.Y -= moveVelocity; 
                coordinateLinesLong += moveVelocity;
            }

            //cameraDir = cameraPos + new Vector3(0, 0, -1);

            glControl1.Invalidate();
        }


        private void glControl1_AutoSizeChanged(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, glControl1.AspectRatio, 0.1f, 1000);
            Matrix4 lookat = Matrix4.LookAt(cameraPos.X, cameraPos.Y, cameraPos.Z, cameraDir.X, cameraDir.Y, cameraDir.Z, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);

        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, glControl1.AspectRatio, 0.1f, 1000);
            Matrix4 lookat = Matrix4.LookAt(cameraPos.X, cameraPos.Y, cameraPos.Z, cameraDir.X, cameraDir.Y, cameraDir.Z, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            //GL.Viewport(0, 0, glControl1.Width, glControl1.Height);

            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0.0, -1*coordinateLinesLong);
            GL.Vertex3(0.0, 0.0, coordinateLinesLong);


            GL.Color3(Color.Green);
            GL.Vertex3(0.0, -1 * coordinateLinesLong, 0.0);
            GL.Vertex3(0.0, coordinateLinesLong, 0.0);

            GL.Color3(Color.Red);
            GL.Vertex3(-1 * coordinateLinesLong, 0.0, 0);
            GL.Vertex3(coordinateLinesLong, 0.0, 0);
            GL.End();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexArrayObject);
            GL.BindVertexArray(VertexArrayObject);
            GL.UseProgram(ShaderObject);


            int modelAddress = GL.GetUniformLocation(ShaderObject, "model");
            int viewAddress = GL.GetUniformLocation(ShaderObject, "view");
            int projectionAddress = GL.GetUniformLocation(ShaderObject, "projection");

            Matrix4 rotX, rotY, rotZ;
            rotX = Matrix4.CreateRotationX(x_angle_3d_model);
            rotY = Matrix4.CreateRotationY(y_angle_3d_model);
            rotZ = Matrix4.CreateRotationZ(z_angle_3d_model);

            Matrix4 model = Matrix4.Identity * rotZ * rotY * rotX;

            //Matrix4 view = Matrix4.LookAt(
            //    -5, 0, 0,
            //    0, 0, 0,
            //    0, 1, 0);


            GL.UniformMatrix4(modelAddress, false, ref model);
            GL.UniformMatrix4(viewAddress, false, ref lookat);
            GL.UniformMatrix4(projectionAddress, false, ref perspective);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureObject);

            int texLocation = GL.GetUniformLocation(TextureObject, "texture_diffuse1");

            GL.Uniform1(texLocation, 0);

            int location = GL.GetUniformLocation(ShaderObject, "material.ambient");
            GL.Uniform3(location, modelMat.ambient);
            location = GL.GetUniformLocation(ShaderObject, "material.diffuse");
            GL.Uniform3(location, modelMat.diffuse);
            location = GL.GetUniformLocation(ShaderObject, "material.specular");
            GL.Uniform3(location, modelMat.specular);
            location = GL.GetUniformLocation(ShaderObject, "material.shininess");
            GL.Uniform1(location, modelMat.shininess);

            location = GL.GetUniformLocation(ShaderObject, "light.ambient");
            GL.Uniform3(location, light.ambient);
            location = GL.GetUniformLocation(ShaderObject, "light.diffuse");
            GL.Uniform3(location, light.diffuse);
            location = GL.GetUniformLocation(ShaderObject, "light.specular");
            GL.Uniform3(location, light.specular);
            location = GL.GetUniformLocation(ShaderObject, "light.direction");
            GL.Uniform3(location, light.direction);
            location = GL.GetUniformLocation(ShaderObject, "light.intensity");
            GL.Uniform1(location, light.intensity);

            location = GL.GetUniformLocation(ShaderObject, "camPos");
            GL.Uniform3(location, new Vector3(0, 0, 8));

            GL.DrawArrays(BeginMode.Triangles, 0, vertexCount);

            //GraphicsContext.CurrentContext.VSync = true;

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            glControl1.SwapBuffers();

        }
        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);//sonradan yazdık
        }
        private void ModelLoadToBuffer(string filePath)
        {

            ObjParser.ParseFile(filePath + ".obj");

            vertices = ObjParser.Vertices;
            normals = ObjParser.Normals;
            texCoords = ObjParser.TexCoords;
            faces = ObjParser.Faces;

            System.Console.WriteLine("vertices : " + vertices.Count);
            System.Console.WriteLine("triangles : " + triangles.Count);
            System.Console.WriteLine("normals : " + normals.Count);
            System.Console.WriteLine("texCoords : " + texCoords.Count);


            List<float> vertexBuffer = new List<float>();
            foreach (int[] face in faces)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 vertex = vertices[face[i * 3]];
                    Vector2 texCoord = texCoords[face[i * 3 + 1]];
                    Vector3 normal = normals[face[i * 3 + 2]];

                    vertexBuffer.Add(vertex.X);
                    vertexBuffer.Add(vertex.Y);
                    vertexBuffer.Add(vertex.Z);
                    vertexBuffer.Add(texCoord.X);
                    vertexBuffer.Add(texCoord.Y);
                    vertexBuffer.Add(normal.X);
                    vertexBuffer.Add(normal.Y);
                    vertexBuffer.Add(normal.Z);
                }
            }

            vertexCount = vertexBuffer.Count / 8;

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertexBuffer.Count, vertexBuffer.ToArray(), BufferUsageHint.StaticDraw);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            string vertexPath = "model_vert.shader";
            string fragmentPath = "model_frag.shader";

            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            ShaderObject = GL.CreateProgram();

            GL.AttachShader(ShaderObject, VertexShader);
            GL.AttachShader(ShaderObject, FragmentShader);

            GL.LinkProgram(ShaderObject);

            GL.GetProgram(ShaderObject, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(ShaderObject);
                Console.WriteLine(infoLog);
            }
            TextureObject = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureObject);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            StbImage.stbi_set_flip_vertically_on_load(1);

            // Load the image.
            try
            {
                ImageResult image = ImageResult.FromStream(File.OpenRead(filePath + "_diffuse.png"), ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }
            catch
            {
                ImageResult image = ImageResult.FromStream(File.OpenRead(filePath.Substring(0, filePath.LastIndexOf("/")) + "../Model/default_diffuse.png"), ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

                modelMat.ambient = new Vector3(0.5f, 0.5f, 0.5f);
                modelMat.diffuse = new Vector3(0.7f, 0.7f, 0.7f);
                modelMat.specular = new Vector3(0.9f, 0.9f, 0.9f);
                modelMat.shininess = 10;

                light.ambient = new Vector3(0.5f, 0.5f, 0.5f);
                light.diffuse = new Vector3(0.7f, 0.7f, 0.7f);
                light.specular = new Vector3(0.9f, 0.9f, 0.9f);
                light.direction = new Vector3(0, -1, 0);
                light.intensity = 4;
            }
           
            modelMat.ambient = new Vector3(0.5f, 0.5f, 0.5f);
            modelMat.diffuse = new Vector3(0.7f, 0.7f, 0.7f);
            modelMat.specular = new Vector3(0.9f, 0.9f, 0.9f);
            modelMat.shininess = 10;

            light.ambient = new Vector3(0.5f, 0.5f, 0.5f);
            light.diffuse = new Vector3(0.7f, 0.7f, 0.7f);
            light.specular = new Vector3(0.9f, 0.9f, 0.9f);
            light.direction = new Vector3(0, -1, 0);
            light.intensity = 4;
        }
        #endregion
    }
}
