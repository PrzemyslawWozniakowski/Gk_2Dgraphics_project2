﻿using System;
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
        public List<(Color, Vec3)> ColorAndVectorsinVertices;

        public void CalculateColorsinVertices(ref Color[,] colorMap, ref Color[,] vectorMap, Vec3 observerVector, Vec3 lightSourceVector, Vec3 lightColor, double k_s, double k_d, int m)
        {
            ColorsInVertices = new List<Color>();
            foreach (var el in vertices)
            {
                ColorsInVertices.Add(CalculateIdealColor(el.X, el.Y, ref colorMap, ref vectorMap, observerVector, lightSourceVector, lightColor, k_s, k_d, m));

            }
        }

        public void CalculateColorAndVectorsinVertices(ref Color[,] colorMap, ref Color[,] vectorMap)
        {
            ColorAndVectorsinVertices = new List<(Color, Vec3)>();
            foreach (var el in vertices)
            {
                ColorAndVectorsinVertices.Add((colorMap[el.X, el.Y], getVector(el.X, el.Y, ref vectorMap)));
            }
        }

        public void FillTriangle(ref Color[,] colorTable, ref Color[,] vectorTable, ref Color[,] drawTable, Vec3 observerVector, Vec3 lightSource, Vec3 lightColor, double k_s, double k_d, int m, int calculateColorType, bool useReflectors, Vec3 redR, Vec3 greenR, Vec3 blueR)
        {
            if (calculateColorType == 1)
            {
                CalculateColorsinVertices(ref colorTable, ref vectorTable, observerVector, lightSource, lightColor, k_s, k_d, m);
            }
            if (calculateColorType == 2)
                CalculateColorAndVectorsinVertices(ref colorTable, ref vectorTable);

            List<Vertice> worklist = new List<Vertice>(vertices);
            Vertice[] verticeTab = vertices.ToArray();
            int[] ind = new int[worklist.Count];
            List<(double, double, double, double)> Aet = new List<(double, double, double, double)>();

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

            //aet fill algorithm
            int k = 0;
            for (int y = vertices[ind[0]].Y; y <= vertices[ind[vertices.Count - 1]].Y; y++)
            {
                while (k < vertices.Count && vertices[ind[k]].Y == y)
                {
                    if (vertices[(ind[k] + 1) % 3].Y > vertices[ind[k]].Y)
                        Aet.Add((vertices[(ind[k] + 1) % 3].Y, vertices[ind[k]].X, vertices[ind[k]].X, getA(vertices[(ind[k] + 1) % 3], vertices[ind[k]])));
                    else
                        Aet.RemoveAll(item => item.Item1 == vertices[ind[k]].Y && item.Item2 == vertices[(ind[k] + 1) % 3].X);

                    if (vertices[(ind[k] + 2) % 3].Y > vertices[ind[k]].Y)
                        Aet.Add((vertices[(ind[k] + 2) % 3].Y, vertices[ind[k]].X, vertices[ind[k]].X, getA(vertices[(ind[k] + 2) % 3], vertices[ind[k]])));
                    else
                        Aet.RemoveAll(item => item.Item1 == vertices[ind[k]].Y && item.Item2 == vertices[(ind[k] + 2) % 3].X);

                    k++;
                }

                Aet.Sort((p1, p2) =>
                {
                    return (int)(p1.Item3 - p2.Item3);
                });

                int c = Aet.Count / 2;
                for (int i = 0; i < c; i++)
                {
                    for (int d = (int)Aet[2 * i].Item3; d < (int)Aet[2 * i + 1].Item3; d++)
                    {
                        Color color;
                        if (!useReflectors)
                        {
                            switch (calculateColorType)
                            {
                                case 2:
                                    color = CalculateHybrid(d, y, observerVector, lightSource, lightColor, k_s, k_d, m);
                                    break;
                                case 1:
                                    color = CalculateInterpolation(d, y, lightColor, k_s, k_d, m);
                                    break;
                                case 0:
                                default:
                                    color = CalculateIdealColor(d, y, ref colorTable, ref vectorTable, observerVector, lightSource, lightColor, k_s, k_d, m);
                                    break;
                            }
                        }
                        else
                        {
                            Color colorR, colorB, colorG;


                            Vec3 Vp1 = new Vec3(d, y, 0) - redR;
                            Vec3 lightC1;
                            Vp1.NormalizeVector();
                            double productR1 = Vp1.ScalarProduct(new Vec3(0, 0, -1));
                            productR1 = Math.Pow(productR1, 100);
                            if (productR1 < 0.0001) colorR = Color.FromArgb(0, 0, 0);
                            else
                            {
                                lightC1 = new Vec3(1 * productR1, 0, 0);
                                colorR = CalculateIdealColor(d, y, ref colorTable, ref vectorTable, observerVector, redR, lightC1, k_s, k_d, m);
                            }
                            Vp1 = new Vec3(d, y, 0) - greenR;
                            Vp1.NormalizeVector();
                            double productG1 = Vp1.ScalarProduct(new Vec3(0, 0, -1));
                            productR1 = Math.Pow(productG1, 100);
                            if (productR1 < 0.0001) colorG = Color.FromArgb(0, 0, 0);
                            else
                            {
                                lightC1 = new Vec3(0, 1 * productR1, 0);
                                colorG = CalculateIdealColor(d, y, ref colorTable, ref vectorTable, observerVector, greenR, lightC1, k_s, k_d, m);
                            }
                            Vp1 = new Vec3(d, y, 0) - blueR;
                            Vp1.NormalizeVector();
                            double productB1 = Vp1.ScalarProduct(new Vec3(0, 0, -1));
                            productR1 = Math.Pow(productB1, 100);
                            if (productR1 < 0.00001) colorB = Color.FromArgb(0, 0, 0);
                            else
                            {
                                lightC1 = new Vec3(0, 0, 1 * productR1);
                                colorB = CalculateIdealColor(d, y, ref colorTable, ref vectorTable, observerVector, blueR, lightC1, k_s, k_d, m);
                            }
                            color = Color.FromArgb(Clamp(colorR.R + colorB.R + colorG.R, 0, 255), Clamp(colorR.G + colorG.G + colorB.G, 0, 255), Clamp(colorR.B + colorG.B + colorB.B, 0, 255));

                        }
                        drawTable[d, y] = color;
                    }
                }
                for (int i = 0; i < Aet.Count; i++)
                {
                    Aet[i] = (Aet[i].Item1, Aet[i].Item2, Aet[i].Item3 + Aet[i].Item4, Aet[i].Item4);
                }

            }
        }

        public Color CalculateIdealColor(int x, int y, ref Color[,] colorMap, ref Color[,] normalMap, Vec3 observerVector, Vec3 lightSource, Vec3 lightColor, double k_s, double k_d, int m)
        {

            if (x < 0) x = 0;
            var mappixel = colorMap[x, y];
            Vec3 normalV = getVector(x, y, ref normalMap);
            Vec3 observerV = observerVector;
            Vec3 lightV = lightSource - new Vec3(x, y, 0);
            lightV.NormalizeVector();

            double iloczyn1 = normalV.ScalarProduct(lightV);
            Vec3 R = (2 * normalV.ScalarProduct(lightV)) * normalV - lightV;
            double iloczyn2 = R.ScalarProduct(normalV);


            int red, blue, green;
            (red, green, blue) = getRGB(k_d, lightColor, mappixel, iloczyn1, k_s, iloczyn2, m);


            return Color.FromArgb(Clamp(red, 0, 255), Clamp(green, 0, 255), Clamp(blue, 0, 255));
        }

        public Color CalculateInterpolation(int x, int y, Vec3 lightColor, double k_s, double k_d, int m)
        {
            if (x < 0) x = 0;
            double fullField = getField(vertices[0], vertices[1], vertices[2]);
            double Field0, Field1, Field2;
            (Field0, Field1, Field2) = getThreeFields(x, y);

            int red, green, blue;
            (red, green, blue) = getRGBfromVertices(fullField, Field0, Field1, Field2);


            return Color.FromArgb(Clamp(red, 0, 255), Clamp(green, 0, 255), Clamp(blue, 0, 255));
        }

        public Color CalculateHybrid(int x, int y, Vec3 observerVector, Vec3 lightSource, Vec3 lightColor, double k_s, double k_d, int m)
        {
            if (x < 0) x = 0;
            double fullField = getField(vertices[0], vertices[1], vertices[2]);
            double Field0, Field1, Field2;
            (Field0, Field1, Field2) = getThreeFields(x, y);

            int colorred, colorgreen, colorblue;
            (colorred, colorgreen, colorblue) = getRGBfromVerticesHybrid(fullField, Field0, Field1, Field2);

            var mappixel = Color.FromArgb(Clamp(colorred, 0, 255), Clamp(colorgreen, 0, 255), Clamp(colorblue, 0, 255));
            Vec3 normalV = Field0 / fullField * ColorAndVectorsinVertices[0].Item2 + Field1 / fullField * ColorAndVectorsinVertices[1].Item2 + Field2 / fullField * ColorAndVectorsinVertices[2].Item2;

            normalV.NormalizeVector();
            Vec3 observerV = observerVector;
            Vec3 lightV = lightSource - new Vec3(x, y, 0);
            lightV.NormalizeVector();

            double iloczyn1 = normalV.ScalarProduct(lightV);
            Vec3 R = (2 * normalV.ScalarProduct(lightV)) * normalV - lightV;
            double iloczyn2 = R.ScalarProduct(normalV);


            int red, blue, green;
            (red, green, blue) = getRGB(k_d, lightColor, mappixel, iloczyn1, k_s, iloczyn2, m);

            return Color.FromArgb(Clamp(red, 0, 255), Clamp(green, 0, 255), Clamp(blue, 0, 255));
        }


        public double getField(Vertice A, Vertice B, Vertice C)
        {
            Vec3 AB = new Vec3(B.X - A.X, B.Y - A.Y, 0);
            Vec3 AC = new Vec3(C.X - A.X, C.Y - A.Y, 0);

            double p = Math.Sqrt((Math.Pow(AB.x, 2) + Math.Pow(AB.y, 2)) * (Math.Pow(AC.x, 2) + Math.Pow(AC.y, 2)) - Math.Pow(AB.ScalarProduct(AC), 2)) / 2;
            return p;
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

        public (int, int, int) getRGB(double k_d, Vec3 lightColor, Color pixelColor, double product1, double k_s, double product2, int m)
        {
            int red = (int)(k_d * lightColor.x * pixelColor.R * product1 + k_s * lightColor.x * pixelColor.R * Math.Pow(product2, m));
            int blue = (int)(k_d * lightColor.y * pixelColor.B * product1 + k_s * lightColor.y * pixelColor.B * Math.Pow(product2, m));
            int green = (int)(k_d * lightColor.z * pixelColor.G * product2 + k_s * lightColor.z * pixelColor.G * Math.Pow(product2, m));
            return (red, green, blue);
        }

        public (int, int, int) getRGBfromVertices(double fullField, double Field0, double Field1, double Field2)
        {

            int colorred = (int)((Field0 / fullField) * ColorsInVertices[0].R + (Field1 / fullField) * ColorsInVertices[1].R + (Field2 / fullField) * ColorsInVertices[2].R);
            int colorgreen = (int)((Field0 / fullField) * ColorsInVertices[0].G + (Field1 / fullField) * ColorsInVertices[1].G + (Field2 / fullField) * ColorsInVertices[2].G);
            int colorblue = (int)((Field0 / fullField) * ColorsInVertices[0].B + (Field1 / fullField) * ColorsInVertices[1].B + (Field2 / fullField) * ColorsInVertices[2].B);
            return (colorred, colorgreen, colorblue);
        }

        public (int, int, int) getRGBfromVerticesHybrid(double fullField, double Field0, double Field1, double Field2)
        {

            int colorred = (int)((Field0 / fullField) * ColorAndVectorsinVertices[0].Item1.R + (Field1 / fullField) * ColorAndVectorsinVertices[1].Item1.R + (Field2 / fullField) * ColorAndVectorsinVertices[2].Item1.R);
            int colorgreen = (int)((Field0 / fullField) * ColorAndVectorsinVertices[0].Item1.G + (Field1 / fullField) * ColorAndVectorsinVertices[1].Item1.G + (Field2 / fullField) * ColorAndVectorsinVertices[2].Item1.G);
            int colorblue = (int)((Field0 / fullField) * ColorAndVectorsinVertices[0].Item1.B + (Field1 / fullField) * ColorAndVectorsinVertices[1].Item1.B + (Field2 / fullField) * ColorAndVectorsinVertices[2].Item1.B);
            return (colorred, colorgreen, colorblue);
        }



        public (double, double, double) getThreeFields(int x, int y)
        {
            double Field0 = getField(new Vertice(x, y), vertices[1], vertices[2]);
            double Field1 = getField(vertices[0], new Vertice(x, y), vertices[2]);
            double Field2 = getField(vertices[0], vertices[1], new Vertice(x, y));
            return (Field0, Field1, Field2);

        }
        public Vec3 getVector(int x, int y, ref Color[,] vectorMap)
        {
            var vectorpixel = vectorMap[x, y];
            Vec3 normalV = new Vec3((double)(vectorpixel.R - 127) / 127, (double)(-vectorpixel.G + 127) / 127, (double)(vectorpixel.B) / 255);
            normalV.NormalizeVector();
            return normalV;
        }

        public int Clamp(int val, int from, int to)
        {
            if (to < from) return -1;
            if (val > to) val = to;
            if (val < from) val = from;
            return val;
        }
    }


}
