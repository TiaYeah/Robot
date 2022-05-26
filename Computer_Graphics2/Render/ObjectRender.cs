using Computer_Graphics2.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Graphics2.Render
{
    internal class ObjectRender
    {
        private int VertexArrayObject = GL.GenVertexArray();
        private int ElementBufferObject = GL.GenBuffer();
        private int VertexBufferObject = GL.GenBuffer();

        private int IndicesLenght;
        Shader Shader;
        Texture Diffuse, Specular;
        int flag;




        public ObjectRender(float[] Vertices, uint[] Indices, Shader shader, Texture dff, Texture spcl, int _flag)
        {
            IndicesLenght = Indices.Length;
            this.Shader = shader;
            this.Diffuse = dff;
            this.Specular = spcl;
            this.flag = _flag;
            this.Bind();
            this.ShaderAttribute();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.NamedBufferStorage(
               VertexBufferObject,
               Vertices.Length * sizeof(float),        // the size needed by this buffer
               Vertices,                           // data to initialize with
               BufferStorageFlags.MapWriteBit);    // at this point we will only write to the buffer

            GL.EnableVertexArrayAttrib(VertexArrayObject, 0);

            GL.VertexArrayVertexBuffer(VertexArrayObject, 0, VertexBufferObject, IntPtr.Zero, 8 * sizeof(float));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.DynamicDraw);
        }

        

        public void Bind()
        {
            GL.BindVertexArray(VertexArrayObject);
        }

        public void Render()
        {
            GL.DrawElements(PrimitiveType.Triangles, IndicesLenght, DrawElementsType.UnsignedInt, 0);
            
        }

        public void ShaderAttribute()
        {
            this.Bind();

            var positionLocation = Shader.GetAttribLocation("aPos​");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = Shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = Shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

        }

        public void ApplyTexture()
        {
            if (flag == 1)
            {
                Diffuse.Use(TextureUnit.Texture0);
                Specular.Use(TextureUnit.Texture1);
            } else
            {
                Diffuse.Use(TextureUnit.Texture2);
                Specular.Use(TextureUnit.Texture3);
            }

        }
         
        public void UpdateShaderModel(Matrix4 model)
        {
            Shader.SetMatrix4("model", model);
        }

        public int getFlag()
        {
            return flag;
        }

        public void useShader()
        {
            Shader.Use();
        }
    }
}
