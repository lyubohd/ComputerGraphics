namespace Coursework
{
    using System;
    using Tao.FreeGlut;
    using OpenGL;
    using System.Diagnostics;

    public class StartUp
    {
        private static int width = 1280, height = 720;
        private static ShaderProgram ShaderProgram;
        private static VBO<Vector3> pyramid, cube;
        private static VBO<Vector3> pyramidColor, cubeColor;
        private static VBO<int> pyramidTriangles, cubeQuads;
        private static Stopwatch stopWatch;
        private static float pyramidAngle, cubeAngle;
        private static int pyramidSpeed = 1, cubeSpeed = 1;
        private static int pyramidRodationSide = 1, cubeRotationSide = 1;

        public static void Main()
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("Lyubo's Coursework");

            Glut.glutIdleFunc(RenderFrame);

            Glut.glutKeyboardFunc(KeyboardPress);

            Gl.Enable(EnableCap.DepthTest);
            
            ShaderProgram = new ShaderProgram(VertexShader, FragmentShader);

            ShaderProgram.Use();
            ShaderProgram["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            ShaderProgram["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

            pyramid = new VBO<Vector3>(new Vector3[] {
                new Vector3(0, 1, 0), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),        
                new Vector3(0, 1, 0), new Vector3(1, -1, 1), new Vector3(1, -1, -1),        
                new Vector3(0, 1, 0), new Vector3(1, -1, -1), new Vector3(-1, -1, -1),      
                new Vector3(0, 1, 0), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1) });    
            pyramidColor = new VBO<Vector3>(new Vector3[] {
                new Vector3(0, 1, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 1),
                new Vector3(1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 0),
                new Vector3(1, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 1),
                new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0) });
            pyramidTriangles = new VBO<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, BufferTarget.ElementArrayBuffer);

            cube = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1),
                new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1),
                new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),
                new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1),
                new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),
                new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1) });
            cubeColor = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0),
                new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1),
                new Vector3(1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 0),
                new Vector3(1, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 1),
                new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0),
                new Vector3(0, 1, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 1) });
            cubeQuads = new VBO<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, BufferTarget.ElementArrayBuffer);

            stopWatch = Stopwatch.StartNew();

            Glut.glutMainLoop();
        }

        private static void RenderFrame()
        {
            stopWatch.Stop();
            float pyramidDeltaTime = (float)stopWatch.ElapsedTicks / Stopwatch.Frequency * pyramidSpeed;
            float cubeDeltaTime = (float)stopWatch.ElapsedTicks / Stopwatch.Frequency * cubeSpeed;
            stopWatch.Restart();

            pyramidAngle += pyramidDeltaTime;
            cubeAngle += cubeDeltaTime;
            
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            Gl.UseProgram(ShaderProgram);

            ShaderProgram["model_matrix"].SetValue(Matrix4.CreateRotationY(pyramidAngle * pyramidRodationSide) * Matrix4.CreateTranslation(new Vector3(-1.5f, 0, 0)));
            Gl.BindBufferToShaderAttribute(pyramid, ShaderProgram, "vertexPosition");
            Gl.BindBufferToShaderAttribute(pyramidColor, ShaderProgram, "vertexColor");
            Gl.BindBuffer(pyramidTriangles);
            
            Gl.DrawElements(BeginMode.Triangles, pyramidTriangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            
            ShaderProgram["model_matrix"].SetValue(Matrix4.CreateRotationY((cubeAngle / 2) * cubeRotationSide) * Matrix4.CreateRotationX(cubeAngle * cubeRotationSide) * Matrix4.CreateTranslation(new Vector3(1.5f, 0, 0)));
            Gl.BindBufferToShaderAttribute(cube, ShaderProgram, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeColor, ShaderProgram, "vertexColor");
            Gl.BindBuffer(cubeQuads);
            
            Gl.DrawElements(BeginMode.Quads, cubeQuads.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Glut.glutSwapBuffers();
        }

        private static void KeyboardPress(byte key, int x, int y)
        {
            if (key == '[') pyramidRodationSide = -1;
            else if (key == ']') pyramidRodationSide = 1;
            else if (key == 'q') pyramidSpeed = 0;
            else if (key == 'w') pyramidSpeed = 1;
            else if (key == 'e') pyramidSpeed = 2;
            else if (key == 'r') pyramidSpeed = 3;
            else if (key == 't') pyramidSpeed = 4;

            else if (key == '.') cubeRotationSide = -1;
            else if (key == '/') cubeRotationSide = 1;
            else if (key == 'z') cubeSpeed = 0;
            else if (key == 'x') cubeSpeed = 1;
            else if (key == 'c') cubeSpeed = 2;
            else if (key == 'v') cubeSpeed = 3;
            else if (key == 'b') cubeSpeed = 4;

            else if (key == 27) Glut.glutLeaveMainLoop();
        }

        public static string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexColor;

out vec3 color;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    color = vertexColor;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        public static string FragmentShader = @"
#version 130

in vec3 color;

out vec4 fragment;

void main(void)
{
    fragment = vec4(color, 1);
}
";
    }
}