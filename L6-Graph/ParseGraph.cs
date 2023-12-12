using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Linq;

namespace L6_Graph
{
	public class ParseGraph
	{
		string cvs;
        string[] l6Names;
		List<string> isolatedPeople;
		Dictionary<string, Dictionary<string, int>> allDistances;
		//List<(string, string, int)> sortedDistances;
        Dictionary<string, HashSet<string>> connectionList;
		int diameter;
		public int Diameter { get { return diameter; } }
		List<(string, string)> edgePairs;
        public ParseGraph(FormattingData data)
		{
			this.cvs = data.SubmissionsCSV;
			this.l6Names = data.L6Names;
			//connectionMatrix = fillConnectionMatrix();
            connectionList = fillConnectionList();
			isolatedPeople = findIsolatedPeople();
			allDistances = findAllDistances();
			diameter = findDiameter();
			edgePairs = findEdges();
			//foreach(string p in furthestPeopleFrom("Aarian Malhotra").Item1)
			//{
			//	Console.WriteLine(p);
			//}

		}
		private Dictionary<string, HashSet<string>> fillConnectionList()
		{
			Dictionary<string, HashSet<string>> list = new Dictionary<string, HashSet<string>>();
			foreach(string name in l6Names)
			{
				list.Add(name, new HashSet<string>());
			}

            string[] entries = cvs.Split("\n", StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < entries.Length; i++)
			{
				string[] adjacentNames = entries[i].Split(",", StringSplitOptions.RemoveEmptyEntries);
				for(int j = 1; j < adjacentNames.Length; j++)
				{
					list[adjacentNames[0]].Add(adjacentNames[j]);
                    list[adjacentNames[j]].Add(adjacentNames[0]);
                }
            }
			return list;
			
        }

		//private bool[,] fillConnectionMatrix()
		//{
		//	bool[,] matrix = new bool[l6Names.Length, l6Names.Length];
		//	string[] entries = cvs.Split("\n", StringSplitOptions.RemoveEmptyEntries);

		//	for (int i = 0; i < entries.Length; i++)
		//	{
		//		string[] names = entries[i].Split(",", StringSplitOptions.RemoveEmptyEntries);
		//		int personIndex = Array.IndexOf(l6Names, names[0]);
		//		string[] connectedPeople = names.Skip(1).ToArray();

		//		for (int j = 0; j < connectedPeople.Length; j++)
		//		{
		//			int connectedIndex = Array.IndexOf(l6Names, connectedPeople[j]);
		//			if (connectedIndex == -1)
		//			{
		//				Console.WriteLine(connectedPeople[j]);
		//				throw new Exception();
		//			}
		//			matrix[personIndex, connectedIndex] = true;
		//			matrix[connectedIndex, personIndex] = true;
		//		}
		//	}
		//	return matrix;
		//}

		public void displayConnectedNames(string name)
		{
			if (!l6Names.Contains(name))
			{
				Console.WriteLine("That name cannot be found...");
				return;
			}
			foreach(string n in connectionList[name])
			{
				Console.WriteLine(n);
			}
		}
		public void displayMostConnected()
		{
			int max = connectionList.MaxBy(x => x.Value.Count).Value.Count;
			for (int i = max; i >= max - 2; i--)
			{
				foreach (KeyValuePair<string, HashSet<string>> kvp in connectionList)
				{
					if (kvp.Value.Count == i)
					{
						Console.WriteLine($"{kvp.Key} ({kvp.Value.Count})");
					}
				}
				if (i != max - 2)
				{
					Console.WriteLine("--------------------");
                }
			}
		}
		private List<string> findIsolatedPeople()
		{
			List<string> people = new List<string>();
			for (int i = 0; i < l6Names.Length; i++)
			{

				
				if (connectionList[l6Names[i]].Count == 0)
				{
					people.Add(l6Names[i]);
				}
				
			}
			return people;
		}
		private Dictionary<string, int> shortestPath(string source)
		{
			if (isolatedPeople.Contains(source))
			{
				return impossiblePaths(source);
			}
			int nodeCount = connectionList.Count;
			Dictionary<string, int> distances = new Dictionary<string, int>();
			foreach(string name in l6Names)
			{
                distances.Add(name, int.MaxValue);
			}
			List<string> visited = new List<string>();
			distances[source] = 0;

			for (int i = 0; i < nodeCount - 1; i++)
			{
				string u = minDistance(distances, visited, l6Names, isolatedPeople);
				visited.Add(u);

				if (connectionList.ContainsKey(u))
				{
					foreach (string edge in connectionList[u])
					{
						string v = edge;
						if (!visited.Contains(v) && distances[u] != int.MaxValue && distances[u] + 1 < distances[v])
						{
							distances[v] = distances[u] + 1;
						}
					}
				}
			}
			return distances;
		}
        private string minDistance(Dictionary<string, int> distances, List<string> visited, string[] l6Names, List<string> isolatedPeople)
        {
            int min = int.MaxValue;
            string minName = "";

            foreach(string v in l6Names)
            {
                if (!isolatedPeople.Contains(v) && !visited.Contains(v) && distances[v] <= min)
                {
                    min = distances[v];
                    minName = v;
                }
            }

            return minName;
        }
		private Dictionary<string,int> impossiblePaths(string name)
		{
			Dictionary<string, int> d = new Dictionary<string, int>();
			foreach(string p in l6Names)
			{
				if (p == name)
				{
					d.Add(p, 0);
				}
				else
				{
					d.Add(p, int.MaxValue);
				}
			}
			return d;
		}
		public void displayEveryoneDistance(string name)
		{
			string end = "--------------------\n";
			Dictionary<string, int> d = allDistances[name];
            foreach (KeyValuePair<string, int> i in d.OrderByDescending(x => x.Value))
            {
				if (i.Value == int.MaxValue)
				{
					end+= i.Key + " (No Link)\n";
				}
				else
				{
                    Console.WriteLine($"{i.Key} ({i.Value})");
                }
            }
            Console.WriteLine(end);
        }
		public void displayDistance(string start, string end)
		{
			int d = allDistances[start][end];
			if (d == int.MaxValue)
			{
                Console.WriteLine($"{start} - {end} (No Link)");
            }
			else
			{
				Console.WriteLine($"{start} - {end} ({d})");
			}
		}
		public void displayIsolated()
		{
			foreach(string n in isolatedPeople)
			{
				Console.WriteLine(n);
			}
		}
		private Dictionary<string, Dictionary<string, int>> findAllDistances()
		{
			Dictionary<string, Dictionary<string, int>> allD = new Dictionary<string, Dictionary<string, int>>();
			foreach(string p in l6Names)
			{
				if (isolatedPeople.Contains(p))
				{
					allD.Add(p, impossiblePaths(p));
				}
				else
				{
					allD.Add(p, shortestPath(p));
				}
			}
			return allD;
		}
        //      private List<(string, string, int)> sortDistances()
        //{
        //	List<(string, string, int)> distances = new List<(string, string, int)>();
        //          foreach (KeyValuePair< string, Dictionary<string, int> > i in allDistances)
        //	{
        //              foreach (KeyValuePair < string, int> j in i.Value)
        //		{
        //			(string, string, int) record = (i.Key, j.Key, j.Value);
        //                  (string, string, int) eq = (j.Key, i.Key, j.Value);
        //			if (!distances.Contains(record) && !distances.Contains(eq))
        //			{
        //				distances.Add(record);
        //			}
        //              }

        //          }
        //	distances.OrderByDescending(4)
        //	return distances;
        //      }

        private int findDiameter()
		{
			int diameter = 0;
			foreach(string n in l6Names)
			{
				int d = furthestPeopleFrom(n).Item2;
                if (d > diameter)
				{
					diameter = d;
				}
			}
			return diameter;
		}

		private List<(string, string)> findEdges()
		{
			List<(string, string)> pairs = new List<(string, string)>();

            foreach (string n in l6Names)
			{
				(List<string>, int) ds = furthestPeopleFrom(n);
				if(ds.Item2 == diameter)
				{
					foreach(string m in ds.Item1)
					{
						if(!pairs.Contains((n,m)) && !pairs.Contains((m, n)))
						{
							pairs.Add((n, m));
						}
					}
				}
            }
			return pairs;
		}

        private (List<string>,int) furthestPeopleFrom(string name)
		{
			int maxD = -1;
			List<string> furthestPeople = new List<string>();
			foreach(KeyValuePair<string, int> d in allDistances[name].OrderByDescending(x=>x.Value))
			{
                if (d.Value != int.MaxValue)
				{
					if (maxD == -1 || d.Value == maxD) //first/heighest bracket of distance
					{
						maxD = d.Value;
						furthestPeople.Add(d.Key);
					}
					else
					{
						break;
                    }
				}
			}
			return (furthestPeople,maxD);
		}
		public void displayEdgePairs()
		{
            foreach ((string, string) p in edgePairs)
            {
                Console.WriteLine($"{p.Item1} - {p.Item2} ({diameter})");
            }
        }
		

    }

}

