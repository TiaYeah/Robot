using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Computer_Graphics2.Common;
using Computer_Graphics2.Render;
using OpenTK.Graphics.OpenGL4;


namespace Computer_Graphics2
{

    public class Game : GameWindow
    {
        public int vertexBufferObject;
        public int vertexArrayObject;
        public int elementBufferObject;
        Vector3 LightPos = new Vector3(0.0f, 2.6f, 0.0f);
        List<ObjectRender> ObjectRenderList = new List<ObjectRender>();


        private readonly float[] floorVertices =
        {
            // Position                          Texture coordinates
            1.0f,  1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
            1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
           -1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
           -1.0f,  1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f
        };

        private readonly uint[] floorIndices =
        {
            0, 1, 3,   // first triangle
            1, 2, 3
        };

        Camera camera;

        uint[] indices, sideIndices;
        float[] vertices, sideVertices;
        double Time;
        int Side = 1;
        const double Degrees = 40;
        int indicesLength = 0;

        Texture DiffuseHead, SpecularHead, difFloor, specFloor, difRobotSide, specRobotSide;

        Shader robotShader, floorShader;
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings): base(gameWindowSettings, nativeWindowSettings)
        {
        }

        private void DefineShader(Shader Shader, int flag)
        {
            if (flag == 1)
            {
                Shader.SetInt("material.diffuse", 0);
                Shader.SetInt("material.specular", 1);
            }
            else
            {
                Shader.SetInt("material.diffuse", 2);
                Shader.SetInt("material.specular", 3);
            }
            //Shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            Shader.SetFloat("material.shininess", 10.0f);
            Shader.SetVector3("light.position", LightPos);
            Shader.SetFloat("light.constant", 0.1f);
            Shader.SetFloat("light.linear", 0.09f);
            Shader.SetFloat("light.quadratic", 0.018f);
            Shader.SetVector3("light.ambient", new Vector3(0.2f));
            Shader.SetVector3("light.diffuse", new Vector3(0.5f));
            Shader.SetVector3("light.specular", new Vector3(1.0f));
            Shader.Use();
        }

       
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.LightBlue);
            GL.Enable(EnableCap.DepthTest);
            Cylinder robot = new Cylinder(0.0f, 0.0f, 0.5f, 0.5f, 0.25f, 1);
            Cylinder robotSide = new Cylinder(0.0f, 0.0f, 0.5f, 0.5f, 0.25f, 1);

            


            robotShader = new Shader(@"D:\Projects\Computer_Graphics2\Computer_Graphics2\data\Shaders\shader_base.vert", @"D:\Projects\Computer_Graphics2\Computer_Graphics2\data\Shaders\lightning.frag");
            DefineShader(robotShader,1);
            floorShader = new Shader(@"D:\Projects\Computer_Graphics2\Computer_Graphics2\data\Shaders\shader_base.vert", @"D:\Projects\Computer_Graphics2\Computer_Graphics2\data\Shaders\lightning.frag");
            DefineShader(floorShader, 0);

            Matrix4 view = Matrix4.CreateTranslation(0.0f, -0.25f, -3.0f);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 800 / 800, 0.1f, 100.0f);
            robotShader.SetMatrix4("view", view);
            robotShader.SetMatrix4("projection", projection);

            floorShader.SetMatrix4("view", view);
            floorShader.SetMatrix4("projection", projection);

            //Matrix4 view = Matrix4.CreateTranslation(0.0f, -0.15f, -3.0f);
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 800 / 800, 0.1f, 100.0f);
            //robotShader.SetMatrix4("view", view);
            //robotShader.SetMatrix4("projection", projection);

            DiffuseHead = Texture.LoadFromFile("../../../Resources/robot.jpg");
            SpecularHead = Texture.LoadFromFile("../../../Resources/SpecularMap.png");
            difFloor = Texture.LoadFromFile("../../../Resources/floor2.jpg");
            specFloor = Texture.LoadFromFile("../../../Resources/floor2specular.png");
            //difRobotSide = Texture.LoadFromFile("../../../Resources/robotSide.jpg");
            //specRobotSide = Texture.LoadFromFile("../../../Resources/robotSideSpecular.png");

            ObjectRenderList.Add(new ObjectRender(robotSide.GetAllTogether(), robotSide.GetIndices(), floorShader, difFloor, specFloor, 0));
            
            //robot.buildTopVerices();
            indices = robot.GetIndices();
            vertices = robot.GetAllTogether();

            ObjectRenderList.Add(new ObjectRender(vertices, indices, robotShader, DiffuseHead, SpecularHead, 1));

            //indices = robotSide.GetIndices();
            //vertices = robotSide.GetAllTogether();
            //ObjectRenderList.Add(new ObjectRender(vertices, indices, robotShader, difRobotSide, specRobotSide, 1));
            //sideVertices = robot.getSideVertices();
            //indices = robot.GetIndices();
            //indicesLength = indices.Length;

            //ObjectRenderList.Add(new ObjectRender(sideVertices, indices, robotShader, difRobotSide, specRobotSide, 1));
            //ObjectRenderList.Add(new ObjectRender(vertices, indices, robotShader, difRobotSide, specRobotSide, 1));

            //
            //Cylinder robot = new Cylinder(0.0f, 0.0f, 0.5f, 0.5f, 0.25f, 1);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            //GL.BindVertexArray(vertexArrayObject);
            //indices = robot.GetIndices();
            //vertices = robot.GetAllTogether();
            //GL.NamedBufferStorage(
            //  vertexBufferObject,
            //  vertices.Length * sizeof(float),        // the size needed by this buffer
            //  vertices,                           // data to initialize with
            //  BufferStorageFlags.MapWriteBit);
            //GL.EnableVertexArrayAttrib(vertexArrayObject, 0);
            //GL.VertexArrayVertexBuffer(vertexArrayObject, 0, vertexArrayObject, IntPtr.Zero, 8 * sizeof(float));
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
            //

        }

        public void ShaderAttribute(Shader shader)
        {

            var positionLocation = shader.GetAttribLocation("aPos​");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        

        

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Time += 35.0 * args.Time;

            // if (Math.Abs(Time) > Degrees) Side *= -1;
            //var RotationMatrixZ = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(Time));

            //Time += 15 * args.Time;

            //var RotationMatrix = Matrix4.CreateRotationY(0.4f * (float)Time) *
            //                     Matrix4.CreateRotationZ(0.15f * (float)Time) *
            //                     Matrix4.CreateRotationX((float)-10.0f) *
            //Matrix4.CreateTranslation(0.0f, 0.0f, -2.5f) * Matrix4.CreatePerspectiveFieldOfView(
            //   0.6f, 900 / 900, 1.0f, 10.0f);

            //var model = Matrix4.Identity * RotationMatrix * ScaleMatrix;
            //robotShader.SetMatrix4("viewPos", camera.GetViewMatrix());
            //robotShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            foreach (var Obj in ObjectRenderList)
            {
                if (Obj.getFlag() == 1)
                {
                    var RotationMatrixYrad = Matrix4.CreateRotationY(0.01f * (float)Time);
                    var RotationMatrixHorizontal = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(90));
                    var TranslationMatrix = Matrix4.CreateTranslation(0.0f, 0.75f, 0.0f);
                    var ScaleMatrix = Matrix4.CreateScale(0.5f);

                    //var model = Matrix4.Identity * RotationMatrixZ * TranslationMatrix * RotationMatrixY;
                    var model = ScaleMatrix * Matrix4.Identity * TranslationMatrix * RotationMatrixHorizontal * RotationMatrixYrad;
                    Obj.Bind();
                    Obj.ApplyTexture();
                    Obj.UpdateShaderModel(model);
                    Obj.ShaderAttribute();
                    Obj.useShader();
                    Obj.Render();
                }
                else
                {
                    var TranslationMatrix = Matrix4.CreateTranslation(0.0f, -0.50f, -1.25f);
                    var RotationMatrixYrad = Matrix4.CreateRotationY(0.01f * (float)Time);
                    var ScaleMatrix = Matrix4.CreateScale(2.0f);
                    var RotationMatrixHorizontal = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(90));
                    var model = Matrix4.Identity * ScaleMatrix * RotationMatrixHorizontal * TranslationMatrix;
                    Obj.Bind();
                    Obj.ApplyTexture();
                    Obj.UpdateShaderModel(model);
                    Obj.ShaderAttribute();
                    Obj.useShader();
                    Obj.Render();
                }
            }

            //{
            //    GL.BindVertexArray(vertexArrayObject);

            //    ShaderAttribute();
            //    DiffuseHead.Use(TextureUnit.Texture0);
            //    SpecularHead.Use(TextureUnit.Texture1);

            //    var RotationMatrixYrad = Matrix4.CreateRotationY(0.01f * (float)Time);
            //    var RotationMatrixHorizontal = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(90));
            //    var TranslationMatrix = Matrix4.CreateTranslation(0.0f, 0.75f, 0.0f);
            //    var ScaleMatrix = Matrix4.CreateScale(0.5f);

            //    //var model = Matrix4.Identity * RotationMatrixZ * TranslationMatrix * RotationMatrixY;
            //    var model = ScaleMatrix * Matrix4.Identity * TranslationMatrix * RotationMatrixHorizontal * RotationMatrixYrad;
            //    robotShader.SetMatrix4("model", model);
            //    //_shader.SetMatrix4("view", _camera.GetViewMatrix());
            //    //_shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            //    //_shader.SetVector3("viewPos", _camera.Position);
            //    //_shader.Use();
            //    Matrix4 view = Matrix4.CreateTranslation(0.0f, -0.25f, -3.0f);
            //    Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 800 / 800, 0.1f, 100.0f);
            //    robotShader.SetMatrix4("view", view);
            //    robotShader.SetMatrix4("projection", projection);
            //    GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            //}
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
