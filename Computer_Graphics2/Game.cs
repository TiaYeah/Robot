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
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace Computer_Graphics2
{

    public class Game : GameWindow
    {
        Vector3 LightPos = new Vector3(0.0f, 3.0f, 0.0f);
        List<ObjectRender> ObjectRenderList = new List<ObjectRender>();
        Camera _camera;

        uint[] indices;
        float[] vertices;
        double Time;

        Texture difRobot, specRobot, difFloor, specFloor;

        Shader Shader;
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings): base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.LightBlue);
            GL.Enable(EnableCap.DepthTest);

            Cylinder robot = new Cylinder(0.0f, 0.0f, 0.5f, 0.7f, 0.25f);
            Cylinder floor = new Cylinder(0.0f, 0.0f, -0.150f, 4.0f, 0.001f);

            Shader = new Shader(@"D:\Projects\Computer_Graphics2\Computer_Graphics2\data\Shaders\shader_base.vert", @"D:\Projects\Computer_Graphics2\Computer_Graphics2\data\Shaders\lightning.frag");
            DefineShader(Shader);

            difRobot = Texture.LoadFromFile("../../../Resources/robot.jpg");
            specRobot = Texture.LoadFromFile("../../../Resources/robotSpecular.png");
            difFloor = Texture.LoadFromFile("../../../Resources/floor3.jpg");
            specFloor = Texture.LoadFromFile("../../../Resources/floor3specular.png");

            ObjectRenderList.Add(new ObjectRender(floor.GetAllTogether(), floor.GetIndices(), Shader, difFloor, specFloor, 0)); // пол
            
            indices = robot.GetIndices();
            vertices = robot.GetAllTogether();

            ObjectRenderList.Add(new ObjectRender(vertices, indices, Shader, difRobot, specRobot, 1)); // робот
            _camera = new Camera(new Vector3(0.0f,1.0f,4.0f), Size.X / (float)Size.Y);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1.5f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)args.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)args.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)args.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)args.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)args.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)args.Time; // Down
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Time += 35.0 * args.Time;
            Shader.SetMatrix4("view", _camera.GetViewMatrix());
            Shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            foreach (var Obj in ObjectRenderList)
            {
                if (Obj.getFlag() == 1)
                {
                    var RotationMatrixYrad = Matrix4.CreateRotationY(0.01f * (float)Time);
                    var RotationMatrixHorizontal = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(90));
                    var TranslationMatrix = Matrix4.CreateTranslation(0.0f, 1.5f, 0.0f);
                    var ScaleMatrix = Matrix4.CreateScale(0.5f);
                    var model = ScaleMatrix * Matrix4.Identity * TranslationMatrix * RotationMatrixHorizontal * RotationMatrixYrad;
                    Obj.ApplyTexture();
                    Obj.UpdateShaderModel(model);
                    Obj.setShaderAttribute();
                    Obj.Render();
                }
                else
                {
                    var TranslationMatrix = Matrix4.CreateTranslation(0.0f, -0.5f, -1.25f);
                    var ScaleMatrix = Matrix4.CreateScale(1.0f);
                    var RotationMatrixHorizontal = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(90));
                    var model = Matrix4.Identity * ScaleMatrix * RotationMatrixHorizontal * TranslationMatrix;
                    Obj.ApplyTexture();
                    Obj.UpdateShaderModel(model);
                    Obj.setShaderAttribute();
                    Obj.Render();
                }
            }
            SwapBuffers();
        }
        private void DefineShader(Shader Shader)
        {
            Shader.SetInt("material.diffuse", 0);
            Shader.SetInt("material.specular", 1);
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


        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
