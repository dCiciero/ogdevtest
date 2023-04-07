using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalogenPreTestAPI.Models;

public class ObjectModel
{
    public List<Int32>? DivBy3 { get; set; }
    public List<Int32>? DivBy5 { get; set; }
    public List<Int32>? DivBy7 { get; set; }
    public List<Int32>? EvenNums { get; set; }
    public List<Int32>? OddNums { get; set; }
    public int Mode { get; set; }
    public double Median { get; set; }
}