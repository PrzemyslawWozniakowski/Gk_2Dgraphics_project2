using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace gk_projekt_2
{
    public class Vertice
    {
        public int X;
        public int Y;

        public Vertice(int x, int y)
        {
            X = x;
            Y = y;
        }

    }
    public class Triangle
    {
        public List<Vertice> vertices = new List<Vertice>();
        public int m = 10;
        public double kd = 0.5;
        public double ks = 0.5;


        private static bool
        AetCustomPredicate((double, double, double, double) tuple, double d1, double d2)
        {
            return tuple.Item1 == d1 && tuple.Item2 == d2;
        }

        public List<Color> ColorsInVertices;
        public List<(Color, (double, double, double))> ColorAndVectorsinVertices;

        public void CalculateColorsinVertices(ref Color[,] colorMap, ref Color[,] vectorMap, (double, double, double) observerVector, (double, double, double) lightSourceVector, Color lightColor, double k_s, double k_d, int m)
        {
            ColorsInVertices = new List<Color>();
            foreach (var el in vertices)
            {
                ColorsInVertices.Add(CalculateIdealColor(el.X, el.Y, ref colorMap, ref vectorMap, observerVector, lightSourceVector, lightColor, k_s, k_d, m));
            }
        }

        public void CalculateColorAndVectorsinVertices(ref Color[,] colorMap, ref Color[,] vectorMap)
        {
            ColorAndVectorsinVertices = new List<(Color, (double, double, double))>();
            foreach (var el in vertices)
            {
                ColorAndVectorsinVertices.Add((colorMap[el.X,el.Y], getVector(el.X, el.Y, ref vectorMap)));
            }
        }

        public void FillTriangle(ref Color[,] colorTable, ref Color[,] vectorTable, ref Color[,] drawTable,(double,double,double) observerVector, (double, double, double) lightSourceVector, Color lightColor, double k_s, double k_d, int m, int calculateColorType)
        {
            if (calculateColorType == 1)
                CalculateColorsinVertices(ref colorTable, ref vectorTable, observerVector, lightSourceVector, lightColor, k_s, k_d, m);

            if (calculateColorType == 2)
                CalculateColorAndVectorsinVertices(ref colorTable, ref vectorTable);

            List<Vertice> worklist = new List<Vertice>(vertices);
            Vertice[] verticeTab = vertices.ToArray();
            int[] ind = new int[worklist.Count];
            List<(double, double, double, double)> Aet2 = new List<(double, double, double, double)>();

            for (int i = 0; i < worklist.Count; i++)
                ind[i] = i;

            for (int i = 0; i < worklist.Count - 1; i++)
            {
                for (int j = 0; j < worklist.Count - 1; j++)
                {
                    if (verticeTab[j].Y > verticeTab[j + 1].Y || (verticeTab[j].Y == verticeTab[j + 1].Y && verticeTab[j].X > verticeTab[j + 1].X))
                    {
                        Vertice temp = verticeTab[j];
                        verticeTab[j] = verticeTab[j + 1];
                        verticeTab[j + 1] = temp;
                        int temp2 = ind[j];
                        ind[j] = ind[j + 1];
                        ind[j + 1] = temp2;
                    }
                }
            }
            int k = 0;
            for (int y = vertices[ind[0]].Y; y <= vertices[ind[vertices.Count - 1]].Y; y++)
            {
                while (k < vertices.Count && vertices[ind[k]].Y == y)
                {
                    if (vertices[(ind[k] + 1) % 3].Y > vertices[ind[k]].Y)
                        Aet2.Add((vertices[(ind[k] + 1) % 3].Y, vertices[ind[k]].X, vertices[ind[k]].X, getA(vertices[(ind[k] + 1) % 3], vertices[ind[k]])));
                    else
                        Aet2.RemoveAll(item => item.Item1 == vertices[ind[k]].Y && item.Item2 == vertices[(ind[k] + 1) % 3].X);

                    if (vertices[(ind[k] + 2) % 3].Y > vertices[ind[k]].Y)
                        Aet2.Add((vertices[(ind[k] + 2) % 3].Y, vertices[ind[k]].X, vertices[ind[k]].X, getA(vertices[(ind[k] + 2) % 3], vertices[ind[k]])));
                    else
                        Aet2.RemoveAll(item => item.Item1 == vertices[ind[k]].Y && item.Item2 == vertices[(ind[k] + 2) % 3].X);

                    k++;
                }

                Aet2.Sort((p1, p2) =>
                {
                    return (int)(p1.Item3 - p2.Item3);
                });

                int c = Aet2.Count / 2;
                for (int i = 0; i < c; i++)
                {
                    for (int d = (int)Aet2[2 * i].Item3; d < (int)Aet2[2 * i + 1].Item3; d++)
                    {
                        Color color;
                        switch (calculateColorType)
                        {
                            case 2:
                                color = CalculateHybrid(d, y, observerVector, lightSourceVector, lightColor, k_s, k_d, m);
                                break;
                            case 1:
                                color = CalculateInterpolation(d, y, observerVector, lightSourceVector, lightColor, k_s, k_d, m);
                                break;
                            case 0:
                            default:
                                color = CalculateIdealColor(d, y, ref colorTable, ref vectorTable, observerVector, lightSourceVector, lightColor, k_s, k_d, m);
                                break;
                        }
                        drawTable[d, y] = color;
                    }
                }
                for (int i = 0; i < Aet2.Count; i++)
                {
                    Aet2[i] = (Aet2[i].Item1, Aet2[i].Item2, Aet2[i].Item3 + Aet2[i].Item4, Aet2[i].Item4);
                }

            }
        }

        public Color CalculateIdealColor(int x, int y, ref Color[,] colorMap, ref Color[,] normalMap,(double, double, double) observerVector, (double, double, double) lightSourceVector, Color lightColor, double k_s, double k_d, int m)
        {
            if (x < 0) x = 0;
            var mappixel = colorMap[x,y];
            (double, double, double) normalV = getVector(x, y, ref normalMap);
            (double, double, double) observerV = observerVector;
            (double, double, double) lightV = NormalizeVector(lightSourceVector);

            double iloczyn1 = (normalV.Item1 * lightV.Item1 + normalV.Item2 * lightV.Item2 + normalV.Item3 * lightV.Item3); // /(Math.Sqrt(Math.Pow(normalV.Item1,2)+ Math.Pow(normalV.Item2,2)+ Math.Pow(normalV.Item3,2)) * Math.Sqrt(Math.Pow(lightV.Item1, 2) + Math.Pow(lightV.Item2, 2) + Math.Pow(lightV.Item3, 2)));
            (double, double, double) R = (2 * normalV.Item1 - lightV.Item1, 2 * normalV.Item2 - lightV.Item2, 2 * normalV.Item3 - lightV.Item3);
            double iloczyn2 = (observerV.Item1 * R.Item1 + observerV.Item2 * R.Item2 + observerV.Item3 * R.Item3);//  /(Math.Sqrt(Math.Pow(R.Item1, 2) + Math.Pow(R.Item2, 2) + Math.Pow(R.Item3, 2)) * Math.Sqrt(Math.Pow(observerV.Item1, 2) + Math.Pow(observerV.Item2, 2) + Math.Pow(observerV.Item3, 2)));
           

            int red = (int)(k_d * lightColor.R * mappixel.R * iloczyn1 + k_s * lightColor.R * mappixel.R * Math.Pow(iloczyn2, m));
            int blue = (int)(k_d * lightColor.B * mappixel.B * iloczyn1 + k_s * lightColor.B * mappixel.B * Math.Pow(iloczyn2, m));
            int green = (int)(k_d * lightColor.G * mappixel.G * iloczyn1 + k_s * lightColor.G * mappixel.G * Math.Pow(iloczyn2, m));

            return Color.FromArgb(Clamp(red, 0, 255), Clamp(green, 0, 255), Clamp(blue, 0, 255));
        }

        public Color CalculateInterpolation(int x, int y, (double, double, double) observerVector, (double, double, double) lightSourceVector, Color lightColor, double k_s, double k_d, int m)
        {
            if (x < 0) x = 0;
            double fullField = getField(getLength(vertices[0], vertices[1]), getLength(vertices[0], vertices[2]), getLength(vertices[1], vertices[2]));
            double Field0 = getField(getLength(new Vertice(x, y), vertices[1]), getLength(vertices[2], vertices[1]), getLength(new Vertice(x, y), vertices[2]));
            double Field1 = getField(getLength(new Vertice(x, y), vertices[0]), getLength(vertices[2], vertices[0]), getLength(new Vertice(x, y), vertices[2]));
            double Field2 = getField(getLength(new Vertice(x, y), vertices[1]), getLength(vertices[0], vertices[1]), getLength(new Vertice(x, y), vertices[0]));

            int red = (int)(Field0 / fullField * ColorsInVertices[0].R + Field1 / fullField * ColorsInVertices[1].R + Field2 / fullField * ColorsInVertices[2].R);
            int green = (int)(Field0 / fullField * ColorsInVertices[0].G + Field1 / fullField * ColorsInVertices[1].G + Field2 / fullField * ColorsInVertices[2].G);
            int blue = (int)(Field0 / fullField * ColorsInVertices[0].B + Field1 / fullField * ColorsInVertices[1].B + Field2 / fullField * ColorsInVertices[2].B);

            return Color.FromArgb(Clamp(red, 0, 255), Clamp(green, 0, 255), Clamp(blue, 0, 255));
        }

        public Color CalculateHybrid(int x, int y, (double, double, double) observerVector, (double, double, double) lightSourceVector, Color lightColor, double k_s, double k_d, int m)
        {
            if (x < 0) x = 0;
            double fullField = getField(getLength(vertices[0], vertices[1]), getLength(vertices[0], vertices[2]), getLength(vertices[1], vertices[2]));
            double Field0 = getField(getLength(new Vertice(x, y), vertices[1]), getLength(vertices[2], vertices[1]), getLength(new Vertice(x, y), vertices[2]));
            double Field1 = getField(getLength(new Vertice(x, y), vertices[0]), getLength(vertices[2], vertices[0]), getLength(new Vertice(x, y), vertices[2]));
            double Field2 = getField(getLength(new Vertice(x, y), vertices[1]), getLength(vertices[0], vertices[1]), getLength(new Vertice(x, y), vertices[0]));


            int colorred = (int)(Field0 / fullField * ColorAndVectorsinVertices[0].Item1.R + Field1 / fullField * ColorAndVectorsinVertices[1].Item1.R + Field2 / fullField * ColorAndVectorsinVertices[2].Item1.R);
            int colorgreen = (int)(Field0 / fullField * ColorAndVectorsinVertices[0].Item1.G + Field1 / fullField * ColorAndVectorsinVertices[1].Item1.G + Field2 / fullField * ColorAndVectorsinVertices[2].Item1.G);
            int colorblue = (int)(Field0 / fullField * ColorAndVectorsinVertices[0].Item1.B + Field1 / fullField * ColorAndVectorsinVertices[1].Item1.B + Field2 / fullField * ColorAndVectorsinVertices[2].Item1.B);



            var mappixel = Color.FromArgb(Clamp(colorred, 0, 255), Clamp(colorgreen, 0, 255), Clamp(colorblue, 0, 255));
            (double, double, double) normalV = (Field0 / fullField * ColorAndVectorsinVertices[0].Item2.Item1 + Field1 / fullField * ColorAndVectorsinVertices[1].Item2.Item1 + Field2 / fullField * ColorAndVectorsinVertices[2].Item2.Item1,
                                                Field0 / fullField * ColorAndVectorsinVertices[0].Item2.Item2 + Field1 / fullField * ColorAndVectorsinVertices[1].Item2.Item2 + Field2 / fullField * ColorAndVectorsinVertices[2].Item2.Item2,
                                                Field0 / fullField * ColorAndVectorsinVertices[0].Item2.Item3 + Field1 / fullField * ColorAndVectorsinVertices[1].Item2.Item3 + Field2 / fullField * ColorAndVectorsinVertices[2].Item2.Item3);
            normalV =NormalizeVector(normalV);
            (double, double, double) observerV = observerVector;
            (double, double, double) lightV = NormalizeVector(lightSourceVector);

            double iloczyn1 = (normalV.Item1 * lightV.Item1 + normalV.Item2 * lightV.Item2 + normalV.Item3 * lightV.Item3);
            (double, double, double) R = (2 * normalV.Item1 - lightV.Item1, 2 * normalV.Item2 - lightV.Item2, 2 * normalV.Item3 - lightV.Item3);
            double iloczyn2 = (observerV.Item1 * R.Item1 + observerV.Item2 * R.Item2 + observerV.Item3 * R.Item3);



            int red = (int)(k_d * lightColor.R * mappixel.R * iloczyn1 + k_s * lightColor.R * mappixel.R * Math.Pow(iloczyn2, m));
            int blue = (int)(k_d * lightColor.B * mappixel.B * iloczyn1 + k_s * lightColor.B * mappixel.B * Math.Pow(iloczyn2, m));
            int green = (int)(k_d * lightColor.G * mappixel.G * iloczyn1 + k_s * lightColor.G * mappixel.G * Math.Pow(iloczyn2, m));


            return Color.FromArgb(Clamp(red, 0, 255), Clamp(green, 0, 255), Clamp(blue, 0, 255));
        }


        public double getField(double a, double b, double c)
        {
            double p = (a + b + c) * 0.5;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        public double getLength(Vertice v1, Vertice v2)
        {
            return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
        }

        public double getA(Vertice p1, Vertice p2)
        {
            if (p1.X == p2.X) return 0;
            return (((double)p1.X - (double)p2.X) / ((double)p1.Y - (double)p2.Y));
        }

        //public Color getColor(int x, int y, ref Color[,] colorMap)
        //{
        //    //if (Values.Usemap == false) return Values.fillColor;
        //    else return colorMap[x, y];
        //}

        public (double, double, double) getVector(int x, int y, ref Color[,] vectorMap)
        {
            //if (!Values.VectorUseMap) return (0, 0, 1);
            var vectorpixel = vectorMap[x, y];
            (double, double, double) normalV = ((double)((double)vectorpixel.R - 127) / 127, (double)(double)(vectorpixel.G - 127) / 127, (double)((double)vectorpixel.B - 127) / 127);
            normalV = NormalizeVector(normalV);
            return normalV;
        }

        public int Clamp(int val, int from, int to)
        {
            if (to < from) return -1;
            if (val > to) val = to;
            if (val < from) val = from;
            return val;
        }

        public (double, double, double) NormalizeVector((double, double, double) vector)
        {
            double length = Math.Pow(vector.Item1, 2) + Math.Pow(vector.Item2, 2) + Math.Pow(vector.Item3, 2);
            length = Math.Sqrt(length);
            return (vector.Item1 / length, vector.Item2 / length, vector.Item3 / length);
        }
    }


}
