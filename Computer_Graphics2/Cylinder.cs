using System;
using System.Collections.Generic;
using System.Text;

namespace Computer_Graphics2
{
    class Cylinder
    {

        public List<float> Vertecies = new List<float>();
        public List<float> sideVertecies = new List<float>();
        public List<float> Normals = new List<float>();
        public List<float> TexCoords = new List<float>();

        public List<uint> Indices = new List<uint>();
        public List<int> lineIndices = new List<int>();

        public float baseRadius, height;
        public int sectorCount;

        private int baseCenterIndex;
        private int topCenterIndex;

        float x, y, z; 

        public Cylinder(float x, float y, float z, float baseRadius, float height, int sectorCount = 100)
        {
            this.baseRadius = baseRadius;
            this.height = height;
            this.sectorCount = sectorCount;
            this.x = x;
            this.y = y;
            this.z = z;
            this.buildVertices();
        }

        public List<float> getUnitCircleVertices()
        {
            const float PI = 3.1415926f;
            float sectorStep = 2 * PI / sectorCount;
            float sectorAngle;  // radian

            List<float> unitCircleVertices = new List<float>();
            for (int i = 0; i <= sectorCount; ++i)
            {
                sectorAngle = i * sectorStep;
                unitCircleVertices.Add((float)Math.Cos(sectorAngle)); // x
                unitCircleVertices.Add((float)Math.Sin(sectorAngle)); // y
                unitCircleVertices.Add(0);                // z
            }
            return unitCircleVertices;
        }

        public void buildVertices()
        {
            List<float> unitVertices = getUnitCircleVertices();
            for (int i = 0; i < 2; ++i)
            {
                float h = -height / 2.0f + i * height;           // z value; -h/2 to h/2

                for (int j = 0, k = 0; j <= sectorCount; ++j, k += 3)
                {
                    float ux = unitVertices[k];
                    float uy = unitVertices[k + 1];
                    float uz = unitVertices[k + 2];

                    Vertecies.Add(ux * baseRadius + x);             // vx
                    Vertecies.Add(uy * baseRadius + y);             // vy
                    Vertecies.Add(h + z);                       // vz
                                                                // normal vector
                    Normals.Add(ux);                       // nx
                    Normals.Add(uy);                       // ny
                    Normals.Add(uz);                       // nz
                                                           // texture coordinate
                    TexCoords.Add(1); //(float)j / sectorCount
                    TexCoords.Add(1);                      
                }
            }
            baseCenterIndex = (int)Vertecies.Count / 3;
            topCenterIndex = baseCenterIndex + sectorCount + 1;// include center vertex
           
            // put base and top vertices to arrays
            for (int i = 0; i < 2; ++i)
            {
                float h = -height / 2.0f + i * height;           // z value; -h/2 to h/2
                float nz = -1 + i * 2;                           // z value of normal; -1 to 1

                // center point
                Vertecies.Add(0 + x); Vertecies.Add(0 + y); Vertecies.Add(h + z);
                Normals.Add(0); Normals.Add(0); Normals.Add(nz);
                TexCoords.Add(0.5f); TexCoords.Add(0.5f);

                for (int j = 0, k = 0; j < sectorCount; ++j, k += 3)
                {
                    float ux = unitVertices[k];
                    float uy = unitVertices[k + 1];

                    Vertecies.Add(ux * baseRadius + x);             // vx
                    Vertecies.Add(uy * baseRadius + y);             // vy
                    Vertecies.Add(h + z);                       // vz
                                                                // normal vector
                    Normals.Add(0);                        // nx
                    Normals.Add(0);                        // ny
                    Normals.Add(nz);                       // nz
                                                           // texture coordinate
                    TexCoords.Add(-ux * 0.5f + 0.5f);      
                    TexCoords.Add(-uy * 0.5f + 0.5f);
                }
            }
        }

        public uint[] GetIndices()
        {
            int k1 = 0;
            int k2 = sectorCount + 1;

            for (int i = 0; i < sectorCount; ++i, ++k1, ++k2)
            {
                // 2 triangles per sector
                Indices.Add(Convert.ToUInt32(k1));
                Indices.Add(Convert.ToUInt32(k1 + 1));
                Indices.Add(Convert.ToUInt32(k2));
                
                Indices.Add(Convert.ToUInt32(k2));
                Indices.Add(Convert.ToUInt32(k1 + 1));
                Indices.Add(Convert.ToUInt32(k2 + 1));
            }

            for (int i = 0, k = baseCenterIndex + 1; i < sectorCount; ++i, ++k)
            {
                if (i < sectorCount - 1)
                {
                    Indices.Add(Convert.ToUInt32(baseCenterIndex));
                    Indices.Add(Convert.ToUInt32(k + 1));
                    Indices.Add(Convert.ToUInt32(k));
                }
                else // last triangle
                {
                    Indices.Add(Convert.ToUInt32(baseCenterIndex));
                    Indices.Add(Convert.ToUInt32(baseCenterIndex + 1));
                    Indices.Add(Convert.ToUInt32(k));
                }
            }

            // Indices for the top surface
            for (int i = 0, k = topCenterIndex + 1; i < sectorCount; ++i, ++k)
            {
                if (i < sectorCount - 1)
                {
                    Indices.Add(Convert.ToUInt32(topCenterIndex));
                    Indices.Add(Convert.ToUInt32(k));
                    Indices.Add(Convert.ToUInt32(k + 1));
                }
                else // last triangle
                {
                    Indices.Add(Convert.ToUInt32(topCenterIndex));
                    Indices.Add(Convert.ToUInt32(k));
                    Indices.Add(Convert.ToUInt32(topCenterIndex + 1));
                }
            }


            return Indices.ToArray();
        }

        public float[] GetAllTogether()
        {
            List<float> result = new List<float>();

            for (int i = 0; i < Vertecies.Count / 3; ++i)
            {
                result.Add(Vertecies[i * 3]);
                result.Add(Vertecies[i * 3 + 1]);
                result.Add(Vertecies[i * 3 + 2]);

                result.Add(Normals[i * 3]);
                result.Add(Normals[i * 3 + 1]);
                result.Add(Normals[i * 3 + 2]);

                result.Add(TexCoords[i * 2]);
                result.Add(TexCoords[i * 2 + 1]);
            }

            return result.ToArray();
        }
    } 
}


