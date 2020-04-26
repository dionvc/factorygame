using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class PoissonDiscDistribution
    {
        /// <summary>
        /// Gets a distribution of points using poisson disc sampling.  Samples will be at least sampleDistance apart.
        /// </summary>
        /// <param name="chunkOrigin"></param>
        /// <param name="sampleDistance"></param>
        /// <returns></returns>
        public static Vector2[] GetDistribution(float sampleDistance, long seed, int samplingMaximum)
        {
            Random r = new Random((int)seed % int.MaxValue);
            int size = (int)(((Props.chunkSize + 0.5) * Props.tileSize) / sampleDistance);
            List<int> activeList = new List<int>();
            List<Vector2> samples = new List<Vector2>();
            int[,] samplingGrid = new int[size, size];
            for(int i = 0; i < samplingGrid.Length; i++)
            {
                samplingGrid[i % size, i / size] = -1;
            }
            Vector2 init = new Vector2((float)(r.NextDouble() * size), (float)(r.NextDouble() * size));
            samples.Add(init);
            samplingGrid[(int)(init.x/sampleDistance), (int)(init.y/sampleDistance)] = 0;
            activeList.Add(0);
            while(activeList.Count > 0)
            {
                //Get a sample from active list.
                int activeIndex = activeList.Count - 1;
                Vector2 activeSample = samples[activeList[activeList.Count - 1]];
                //Generate samples.
                //Check each sample for validity:
                //is sampling grid cell -1, and then is it less than r distance away from other samples
                for (int i = 0; i < samplingMaximum; i++)
                {
                    int randomAngle = r.Next(0, 360);
                    float randomRadius = (float)(r.NextDouble()) * sampleDistance + sampleDistance;
                    Vector2 newSample = (new Vector2(randomRadius, randomAngle, false)).Add(activeSample);
                    if(newSample.x < 0)
                    {
                        newSample.x = 32;
                    }
                    if(newSample.y < 0)
                    {
                        newSample.y = 32;
                    }
                    int sampleX = (int)(newSample.x/sampleDistance);
                    int sampleY = (int)(newSample.y/sampleDistance);
                    if (sampleX >= 0 && sampleY >= 0 && sampleX < size && sampleY < size && samplingGrid[sampleX, sampleY] == -1)
                    {
                        bool allowed = true;
                        for(int j = sampleX - 1; j <= sampleX + 1; j++)
                        {
                            for(int k = sampleY - 1; k < sampleY +1; k++)
                            {
                                if(j < 0 || k < 0 || j >= size || k >= size || samplingGrid[j, k] == -1)
                                {
                                    continue;
                                }
                                float distance = samples[samplingGrid[j, k]].GetDistance(newSample);
                                if (distance < sampleDistance)
                                {
                                    allowed = false;
                                    break;
                                }
                            }
                            if(allowed == false)
                            {
                                break;
                            }
                        }
                        if (allowed == true)
                        {
                            samples.Add(newSample);
                            samplingGrid[sampleX, sampleY] = samples.Count - 1;
                            activeList.Add(samples.Count - 1);
                        }
                    }
                }

                //Check if current active sample can be removed from activelist.
                int activeX = (int)(activeSample.x/sampleDistance);
                int activeY = (int)(activeSample.y/sampleDistance);
                bool removable = true;
                for (int j = activeX - 1; j <= activeX + 1; j++)
                {
                    for (int k = activeY - 1; k < activeY + 1; k++)
                    {
                        if(j < 0 || k < 0 || j >= size || k >= size || samplingGrid[j, k] == -1)
                        {
                            removable = false;
                        }
                    }
                }
                if(removable == true || removable == false)
                {
                    activeList.RemoveAt(activeIndex);
                }
                //proceed until size*size samples have been generated.
            }
            return samples.ToArray();
        }
    }
}
