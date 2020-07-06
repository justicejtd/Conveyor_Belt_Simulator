using System;
using System.Collections.Generic;
using System.Windows;

namespace ProcP.Models
{
    public class Dijkstra
    {
        private List<Belt> vertices;
        private List<Belt> shortestPath;

        public List<Belt> GetShortestPath(Belt startingBelt, Belt endingBelt)
        {
            try
            {
                if (endingBelt == null)
                {
                    shortestPath = new List<Belt>();
                    throw new MyException("test");
                }
                else
                {
                    RecursivelyFindShortestPath(startingBelt, endingBelt);
                }
            }
            catch (MyException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }

            catch (Exception ex)
            {
                shortestPath = null;
                Console.WriteLine(ex.Message);
            }
            return shortestPath;
        }

        private void RecursivelyFindShortestPath(Belt startingBelt, Belt endingBelt)
        {
            try
            {
                shortestPath = new List<Belt>();
                if (endingBelt != startingBelt)
                {
                    if (endingBelt == null)
                    {
                        throw new MyException("test");
                    }
                    GetShortestPath(startingBelt, endingBelt.Parent);
                    shortestPath.Add(endingBelt);
                    Console.WriteLine("Vertex {0} weight: {1}", endingBelt.Name, endingBelt.Weight);
                }
                else
                {
                    shortestPath.Add(endingBelt);
                    Console.WriteLine("Vertex {0} weight: {1}", endingBelt.Name, endingBelt.Weight);
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Please complete path from check-in " + startingBelt + "until departure gate " + startingBelt);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void InitializeSingleSource(Belt[] vertices, Belt startV)
        {
            foreach (Belt v in vertices)
            {
                v.Weight = int.MaxValue;
                v.Parent = null;
            }
            startV.Weight = 0;
        }

        public void Relax(Belt u, Belt v, int weight)
        {
            if (v.Weight > u.Weight + weight)
            {
                v.Weight = u.Weight + weight;
                v.Parent = u;
            }
        }

        public Dijkstra(Belt[] vertices, int[][] graph, int source)
        {
            this.vertices = new List<Belt>();
            InitializeSingleSource(vertices, vertices[source]);
            //adding all vertex to priority queue
            PriorityQueue<Belt> queue = new PriorityQueue<Belt>(true);
            for (int i = 0; i < vertices.Length; i++)
            {
                queue.Enqueue(vertices[i].Weight, vertices[i]);
            }
            //treversing to all vertices
            while (queue.Count > 0)
            {
                var u = queue.Dequeue();
                this.vertices.Add(u);
                //again traversing to all vertices
                for (int v = 0; v < graph[Convert.ToInt32(u.Name)].Length; v++)
                {
                    if (graph[Convert.ToInt32(u.Name)][v] > 0)
                    {
                        Relax(u, vertices[v], graph[Convert.ToInt32(u.Name)][v]);
                        //updating priority value since distance is changed
                        queue.UpdatePriority(vertices[v], vertices[v].Weight);
                    }
                }
            }
            //this.vertices = new List<Belt>();
        }
    }
}
