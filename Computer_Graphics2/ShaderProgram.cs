using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics2
{
    class ShaderProgram
    {
        private readonly int _vertexShader = 0;
        private readonly int _fragmentShader = 0;
        private readonly int _program = 0;
        public ShaderProgram(string vertexfile, string fragmentfile)
        {
            _vertexShader = createShader(ShaderType.VertexShader, vertexfile);
            _fragmentShader = createShader(ShaderType.FragmentShader, fragmentfile);

            _program = GL.CreateProgram();
            GL.AttachShader(_program, _vertexShader);
            GL.AttachShader(_program, _fragmentShader);

            GL.LinkProgram(_program);
            GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out var code);

            if (code != (int)All.True)
            {
                var infoLog = GL.GetProgramInfoLog(_program);
                throw new Exception($"Ошибка линковки шейдерной программы № {_program} \n\n {infoLog}");
            }

            deleteShader(_vertexShader);
            deleteShader(_fragmentShader);
        }

        private int createShader(ShaderType shaderType, string shaderFile)
        {
            string shaderStr = File.ReadAllText(shaderFile);
            int shaderID = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderID, shaderStr);
            GL.CompileShader(shaderID);
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shaderID);
                throw new Exception($"Ошибка прикомпиляции шейдера № {shaderID} \n\n {infoLog}");
            }

            return shaderID;
        }
       
        public void activateProgram()
        {
            GL.UseProgram(_program);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(_program);
            //GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void deactivateProgram()
        {
            GL.UseProgram(0);
        }
        public void deleteProgram()
        {
            GL.DeleteProgram(_program);
        }

        public int getAttribLocation(string name)
        {
           return GL.GetAttribLocation(_program, name);
        }

        private void deleteShader(int shader)
        {
            GL.DetachShader(_program, shader);
            GL.DeleteShader(shader);
        }
    }
}
