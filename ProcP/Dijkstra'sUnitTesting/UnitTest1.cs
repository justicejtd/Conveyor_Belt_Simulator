using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcP.Models;
using System.Windows;
using System;

namespace Dijkstra_sUnitTesting
{
    [TestClass]
    public class UnitTest1
    {
        private List<Belt> vertices;
        private List<Belt> shortestPath;
        private int[][] matrix;

        [TestMethod]
        public void TestMethod1()
        {
            matrix = new int[14][];
            shortestPath = new List<Belt>();
            vertices = new List<Belt>();

            for (int i = 0; i < 14; i++)
            {
                this.matrix[i] = new int[14];
                for (int j = 0; j < 14; j++)
                {
                    matrix[i][j] = 0;
                }
            }

            matrix[0][1] = 1;
            matrix[1][2] = 1;
            matrix[2][3] = 1;
            matrix[3][4] = 1;
            matrix[4][5] = 1;

            //----Shortestpath
            Belt belt = new Belt(new Point(385, 405));
            belt.Name = "0";
            Belt belt1 = new Belt(new Point(435, 405));
            belt1.Name = "1";
            belt1.Parent = belt;
            Belt belt2 = new Belt(new Point(485, 405));
            belt2.Name = "2";
            belt2.Parent = belt1;
            Belt belt3 = new Belt(new Point(535, 405));
            belt3.Name = "3";
            belt3.Parent = belt2;
            Belt belt4 = new Belt(new Point(585, 405));
            belt4.Name = "4";
            belt4.Parent = belt3;
            Belt belt5 = new Belt(new Point(635, 405));
            belt5.Name = "5";
            belt5.Parent = belt4;

            //-----------second path
            Belt belt6 = new Belt(new Point(385, 355));
            belt6.Name = "6";
            belt6.Parent = belt;
            Belt belt7 = new Belt(new Point(385, 305));
            belt7.Name = "7";
            belt7.Parent = belt;
            Belt belt8 = new Belt(new Point(435, 305));
            belt8.Name = "8";
            belt8.Parent = belt;
            Belt belt9 = new Belt(new Point(485, 305));
            belt9.Name = "9";
            belt9.Parent = belt;
            Belt belt10 = new Belt(new Point(535, 305));
            belt10.Name = "10";
            belt10.Parent = belt;
            Belt belt11 = new Belt(new Point(585, 305));
            belt11.Name = "11";
            belt11.Parent = belt;
            Belt belt12 = new Belt(new Point(635, 305));
            belt12.Name = "12";
            belt12.Parent = belt;
            Belt belt13 = new Belt(new Point(635, 355));
            belt13.Name = "13";
            belt13.Parent = belt;

            shortestPath.Add(belt);
            vertices.Add(belt);
            shortestPath.Add(belt1);
            vertices.Add(belt1);
            shortestPath.Add(belt2);
            vertices.Add(belt2);
            shortestPath.Add(belt3);
            vertices.Add(belt3);
            shortestPath.Add(belt4);
            vertices.Add(belt4);
            shortestPath.Add(belt5);
            vertices.Add(belt5);
            vertices.Add(belt6);
            vertices.Add(belt7);
            vertices.Add(belt8);
            vertices.Add(belt9);
            vertices.Add(belt10);
            vertices.Add(belt11);
            vertices.Add(belt12);
            vertices.Add(belt13);

            Dijkstra dijkstra = new Dijkstra(vertices.ToArray(), matrix, 0);
            CollectionAssert.AreEqual(shortestPath, dijkstra.GetShortestPath(belt, belt5));
        }
    }
}
